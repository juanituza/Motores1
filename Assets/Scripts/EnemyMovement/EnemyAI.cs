using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState { Patrolling, Investigating, Chasing, Cooldown }
    public EnemyState currentState = EnemyState.Patrolling;

    private NavMeshAgent agent;
    public Transform player;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4.5f;
    public List<Transform> patrolWaypoints;
    private int currentWaypointIndex = 0;
    private bool movingForward = true;
    private bool isWaitingAtWaypoint = false;
    public float minWaitTime = 2f;
    public float maxWaitTime = 2f;
    public float visionRange = 10f;
    public float visionAngle = 60f;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;

    public float hearingRange = 5f;
    private Vector3 noisePosition;

    public float attackDistance = 1.5f;
    public float cooldownTime = 2.5f;
    private bool isSustainingCooldown = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (patrolWaypoints.Count > 0)
            agent.destination = patrolWaypoints[currentWaypointIndex].position;
    }

    void Update()
    {
        if (isSustainingCooldown) return;
        EvaluateVision();

        if (currentState != EnemyState.Chasing)
        {
            HearingDetection();
        }
        switch (currentState)
        {
            case EnemyState.Patrolling:
                PatrolBehavior();
                break;

            case EnemyState.Investigating:
                InvestigationBehavior();
                break;

            case EnemyState.Chasing:
                ChaseBehavior();
                break;
        }

        ControlAnimations();
    }

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
                    if (currentState != EnemyState.Chasing)
                    {
                        TriggerChase();
                    }
                    return; 
                }
            }
        }

        if (currentState == EnemyState.Chasing && distanceToPlayer > visionRange)
        {
            ReturnToPatrol();
        }
    }

    void HearingDetection()
    {
        if (currentState == EnemyState.Chasing) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        bool playerIsMakingNoise = false;
        if (Input.GetKey(KeyCode.LeftShift) && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
        {
            playerIsMakingNoise = true;
        }

        if (distanceToPlayer <= hearingRange && playerIsMakingNoise)
        {
            TriggerChase();
        }
    }
    void TriggerChase()
    {
        if (isWaitingAtWaypoint)
        {
            StopAllCoroutines();
            isWaitingAtWaypoint = false;
        }

        agent.isStopped = false;
        agent.speed = chaseSpeed;
        currentState = EnemyState.Chasing;
    }

    void TriggerInvestigation()
    {
        if (isWaitingAtWaypoint)
        {
            StopAllCoroutines();
            isWaitingAtWaypoint = false;
        }
        agent.isStopped = false;
        agent.speed = patrolSpeed;
        currentState = EnemyState.Investigating;
        agent.destination = noisePosition;
    }

    void PatrolBehavior()
    {
        if (isWaitingAtWaypoint) return;
        agent.speed = patrolSpeed;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            StartCoroutine(WaitAndInvestigateWaypoint());
        }
    }

    void InvestigationBehavior()
    {
        agent.speed = patrolSpeed;
        if (!agent.pathPending && agent.remainingDistance < 0.6f)
        {
            StartCoroutine(FinishInvestigation());
        }
    }

    IEnumerator FinishInvestigation()
    {
        agent.isStopped = true;
        yield return new WaitForSeconds(2.5f);
        if (currentState == EnemyState.Investigating)
        {
            agent.isStopped = false;
            ReturnToPatrol();
        }
    }

    void ChaseBehavior()
    {
        agent.speed = chaseSpeed; 
        agent.destination = player.position; 

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackDistance)
        {
            StartCoroutine(PerformAttack());
        }
    }

    IEnumerator PerformAttack()
    {
        isSustainingCooldown = true;
        currentState = EnemyState.Cooldown;
        agent.isStopped = true;

        Debug.Log("hit");
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeHit(transform.position);
        }


        yield return new WaitForSeconds(cooldownTime);

        agent.isStopped = false;
        isSustainingCooldown = false;
        ReturnToPatrol();
    }

    void ReturnToPatrol()
    {
        currentState = EnemyState.Patrolling;
        if (patrolWaypoints.Count > 0)
        {
            agent.destination = patrolWaypoints[currentWaypointIndex].position;
        }
    }

    IEnumerator WaitAndInvestigateWaypoint()
    {
        isWaitingAtWaypoint = true;
        agent.isStopped = true;
        yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));

        if (currentState != EnemyState.Patrolling)
        {
            isWaitingAtWaypoint = false;
            yield break;
        }

        CalculateNextIndex();
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
            if (currentWaypointIndex >= patrolWaypoints.Count)
            {
                movingForward = false;
                currentWaypointIndex = patrolWaypoints.Count - 2;
            }
        }
        else
        {
            currentWaypointIndex--;
            if (currentWaypointIndex < 0)
            {
                movingForward = true;
                currentWaypointIndex = 1;
            }
        }
    }

    void ControlAnimations()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            float targetSpeedForAnimator = 0f;

            if (currentState == EnemyState.Patrolling || currentState == EnemyState.Investigating)
            {
                targetSpeedForAnimator = (agent.isStopped) ? 0f : patrolSpeed; 
            }
            else if (currentState == EnemyState.Chasing)
            {
                targetSpeedForAnimator = chaseSpeed; 
            }

            float currentAnimatorSpeed = anim.GetFloat("currentSpeed");
            float smoothedSpeed = Mathf.MoveTowards(currentAnimatorSpeed, targetSpeedForAnimator, Time.deltaTime * 8f); 

            anim.SetFloat("currentSpeed", smoothedSpeed);
            anim.SetBool("isWaiting", agent.isStopped && currentState != EnemyState.Cooldown);
            anim.SetBool("isCoolingDown", isSustainingCooldown);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hearingRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }
}