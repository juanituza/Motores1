using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState { Patrolling, Chasing, Cooldown }
    [Header("States")]
    public EnemyState currentState = EnemyState.Patrolling;

    [Header("Components")]
    private NavMeshAgent agent;
    public Transform player;

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4.5f;

    [Header("Patrol (Waypoints)")]
    public List<Transform> patrolWaypoints;
    private int currentWaypointIndex = 0;
    private bool movingForward = true;
    private bool isWaitingAtWaypoint = false;

    [Header("Hello Neighbor Behavior")]
    [Tooltip("Minimum time the enemy stays still investigating the waypoint")]
    public float minWaitTime = 2f;
    [Tooltip("Maximum time the enemy stays still investigating the waypoint")]
    public float maxWaitTime = 4.5f;

    [Header("Visual Detection (FOV)")]
    public float visionRange = 10f;
    [Range(0, 360)] public float visionAngle = 60f;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;

    [Header("Hearing Detection")]
    public float hearingRange = 4f;

    [Header("Attack and Cooldown")]
    public float attackDistance = 1.5f;
    public float cooldownTime = 2.5f;
    private bool isSustainingCooldown = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (patrolWaypoints.Count > 0)
        {
            // Heads straight to the first assigned waypoint on start
            agent.destination = patrolWaypoints[currentWaypointIndex].position;
        }
    }

    void Update()
    {
        if (isSustainingCooldown) return;

        HearingDetection();

        switch (currentState)
        {
            case EnemyState.Patrolling:
                PatrolBehavior();
                EvaluateVision();
                break;

            case EnemyState.Chasing:
                ChaseBehavior();
                break;
        }

        // Optional call to handle animations
        ControlAnimations();
    }

    // --- PATROL LOGIC ---
    void PatrolBehavior()
    {
        // If standing still investigating, suspend movement logic
        if (isWaitingAtWaypoint) return;

        agent.speed = patrolSpeed;

        // If reached close to current waypoint, start investigation pause
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            StartCoroutine(WaitAndInvestigateWaypoint());
        }
    }

    IEnumerator WaitAndInvestigateWaypoint()
    {
        isWaitingAtWaypoint = true;
        agent.isStopped = true; // Stop the agent

        // Select a random wait time within configured parameters
        float waitDuration = Random.Range(minWaitTime, maxWaitTime);
        yield return new WaitForSeconds(waitDuration);

        // If state changed (detected player) during wait, cancel patrol flow
        if (currentState != EnemyState.Patrolling)
        {
            isWaitingAtWaypoint = false;
            yield break;
        }

        // Calculate next waypoint in ping-pong sequence
        CalculateNextIndex();

        // Assign new destination and resume movement
        if (patrolWaypoints.Count > 0)
        {
            agent.isStopped = false;
            agent.destination = patrolWaypoints[currentWaypointIndex].position;
        }

        isWaitingAtWaypoint = false;
    }

    void CalculateNextIndex()
    {
        if (patrolWaypoints.Count <= 1) return;

        if (movingForward)
        {
            currentWaypointIndex++;
            // If exceeding list boundaries, bounce backward
            if (currentWaypointIndex >= patrolWaypoints.Count)
            {
                movingForward = false;
                currentWaypointIndex = Mathf.Max(0, patrolWaypoints.Count - 2);
            }
        }
        else
        {
            currentWaypointIndex--;
            // If dropping below 0, advance forward again
            if (currentWaypointIndex < 0)
            {
                movingForward = true;
                currentWaypointIndex = Mathf.Min(patrolWaypoints.Count - 1, 1);
            }
        }
    }

    // --- DETECTION LOGIC ---
    void EvaluateVision()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= visionRange)
        {
            if (Vector3.Angle(transform.forward, directionToPlayer) < visionAngle / 2)
            {
                if (!Physics.Raycast(transform.position + Vector3.up, directionToPlayer, distanceToPlayer, obstacleLayer))
                {
                    DetectPlayer();
                }
            }
        }
    }

    void HearingDetection()
    {
        if (currentState == EnemyState.Chasing) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= hearingRange)
        {
            DetectPlayer();
        }
    }

    void DetectPlayer()
    {
        // If waiting at a waypoint, cancel the investigation immediately
        if (isWaitingAtWaypoint)
        {
            StopAllCoroutines();
            isWaitingAtWaypoint = false;
            agent.isStopped = false;
        }

        currentState = EnemyState.Chasing;
    }

    // --- CHASE AND HIT LOGIC ---
    void ChaseBehavior()
    {
        agent.speed = chaseSpeed;
        agent.destination = player.position;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackDistance)
        {
            StartCoroutine(PerformAttack());
        }

        // If player breaks line of sight range, lose track
        if (distanceToPlayer > visionRange)
        {
            ReturnToPatrol();
        }
    }

    IEnumerator PerformAttack()
    {
        isSustainingCooldown = true;
        currentState = EnemyState.Cooldown;
        agent.isStopped = true;

        Debug.Log("HIT! The enemy struck you.");

        yield return new WaitForSeconds(cooldownTime);

        agent.isStopped = false;
        isSustainingCooldown = false;

        ReturnToPatrol();
    }

    void ReturnToPatrol()
    {
        currentState = EnemyState.Patrolling;

        // Resume route from the last configured waypoint index
        if (patrolWaypoints.Count > 0)
        {
            agent.destination = patrolWaypoints[currentWaypointIndex].position;
        }
    }

    // --- ANIMATION LOGIC ---
    void ControlAnimations()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetFloat("currentSpeed", agent.velocity.magnitude);
            anim.SetBool("isWaiting", isWaitingAtWaypoint);
            anim.SetBool("isCoolingDown", isSustainingCooldown);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Hearing Range (Blue)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hearingRange);

        // Vision Range (Red)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        // Vision Cone (Yellow)
        Vector3 fovLine1 = Quaternion.AngleAxis(visionAngle / 2, Vector3.up) * transform.forward;
        Vector3 fovLine2 = Quaternion.AngleAxis(-visionAngle / 2, Vector3.up) * transform.forward;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position + Vector3.up, fovLine1 * visionRange);
        Gizmos.DrawRay(transform.position + Vector3.up, fovLine2 * visionRange);
    }
}