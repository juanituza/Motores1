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
    private Animator anim;
    public Transform player;

    [Header("Patrol")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4.5f;
    public List<Transform> patrolWaypoints;
    private int currentWaypointIndex = 0;
    private bool movingForward = true;
    private bool isWaitingAtWaypoint = false;

    [Header("Pause")]
    public float minWaitTime = 2f;
    public float maxWaitTime = 4.5f;

    [Header("Range")]
    public float hearingRange = 5f;

    [Header("Attack")]
    public float attackDistance = 1.5f;
    public float cooldownTime = 2.5f;
    private bool isSustainingCooldown = false;

    [Header("Escape System")]
    public float maxChaseTimeWithoutNoise = 1f;
    private float currentChaseTimer = 0f;

    [Header("Stalker")]
    public List<Transform> spawnPoints;
    public float maxDistanceToTeleport = 25f;
    public float minSpawnDistance = 8f;
    private float teleportCheckTimer = 0f;
    private float timeBetweenChecks = 2f;

    [Header("Audio System")]
    public AudioSource footstepsAudioSource;
    public AudioClip footstepSound;

    void Start()
    {
        GameObject targetPlayer = GameObject.FindWithTag("Player");
        if (targetPlayer != null)
        {
            player = targetPlayer.transform;
        }

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        if (agent != null)
        {
            agent.updateRotation = false;
            agent.updatePosition = true;
        }

        if (anim != null) anim.applyRootMotion = false;

        if (patrolWaypoints.Count > 0 && agent != null && agent.enabled)
            agent.destination = patrolWaypoints[currentWaypointIndex].position;
    }

    void Update()
    {
        if (player == null)
        {
            GameObject targetPlayer = GameObject.FindWithTag("Player");
            if (targetPlayer != null) player = targetPlayer.transform;
            return;
        }

        if (isSustainingCooldown) return;

        HearingDetection();

        CheckPlayerDistanceAndTeleport();

        switch (currentState)
        {
            case EnemyState.Patrolling:
                PatrolBehavior();
                break;

            case EnemyState.Chasing:
                ChaseBehavior();
                break;
        }

        ControlAnimations();
    }

    void PatrolBehavior()
    {
        if (isWaitingAtWaypoint || patrolWaypoints.Count == 0) return;

        agent.isStopped = false;
        agent.speed = patrolSpeed;

        Vector3 nextPathPoint = agent.steeringTarget - transform.position;
        nextPathPoint.y = 0;
        if (nextPathPoint.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(nextPathPoint);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 6f);
        }

        float distanceToWaypoint = Vector3.Distance(transform.position, patrolWaypoints[currentWaypointIndex].position);

        if (!agent.pathPending && distanceToWaypoint < 0.8f)
        {
            StartCoroutine(WaitAndInvestigateWaypoint());
        }
    }

    void ChaseBehavior()
    {
        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.destination = player.position;

        Vector3 nextPathPoint = agent.steeringTarget - transform.position;
        nextPathPoint.y = 0;
        if (nextPathPoint.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(nextPathPoint);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 35f);
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool playerIsMakingNoise = Input.GetKey(KeyCode.LeftShift) && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0);

        if (!playerIsMakingNoise || distanceToPlayer > hearingRange)
        {
            currentChaseTimer += Time.deltaTime;

            if (currentChaseTimer >= maxChaseTimeWithoutNoise)
            {
                Debug.Log("El zombie ciego perdió el rastro acústico.");
                ReturnToPatrol();
                return;
            }
        }
        else
        {
            currentChaseTimer = 0f;
        }

        if (distanceToPlayer <= attackDistance)
        {
            StartCoroutine(PerformAttack());
            return;
        }

        if (distanceToPlayer > maxDistanceToTeleport / 1.5f)
        {
            ReturnToPatrol();
        }
    }

    void HearingDetection()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        bool playerIsMakingNoise = false;
        if (Input.GetKey(KeyCode.LeftShift) && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
        {
            playerIsMakingNoise = true;
        }

        if (currentState == EnemyState.Chasing)
        {
            if (distanceToPlayer <= hearingRange && playerIsMakingNoise)
            {
                currentChaseTimer = 0f;
            }
        }
        else if (currentState == EnemyState.Patrolling)
        {
            if (distanceToPlayer <= hearingRange && playerIsMakingNoise)
            {
                TriggerChase();
            }
        }
    }

    void TriggerChase()
    {
        if (isWaitingAtWaypoint)
        {
            StopAllCoroutines();
            isWaitingAtWaypoint = false;
        }
        currentState = EnemyState.Chasing;
        currentChaseTimer = 0f;
    }

    IEnumerator PerformAttack()
    {
        if (player == null) yield break;

        float realDistance = Vector3.Distance(transform.position, player.position);
        if (realDistance > attackDistance + 0.5f)
        {
            ReturnToPatrol();
            yield break;
        }

        isSustainingCooldown = true;
        currentState = EnemyState.Cooldown;

        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        if (anim != null)
        {
            anim.SetFloat("currentSpeed", 0f);
            anim.SetTrigger("hit");
        }

        yield return new WaitForSeconds(cooldownTime);

        isSustainingCooldown = false;

        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            bool playerIsRunningAway = Input.GetKey(KeyCode.LeftShift) && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0);

            if (distanceToPlayer <= hearingRange && playerIsRunningAway)
            {
                currentState = EnemyState.Chasing;
                agent.speed = chaseSpeed;
                if (anim != null) anim.SetFloat("currentSpeed", chaseSpeed);
                currentChaseTimer = 0f;
            }
            else
            {
                ReturnToPatrol();
                if (anim != null) anim.SetFloat("currentSpeed", patrolSpeed);
            }
        }
        else
        {
            ReturnToPatrol();
        }
    }

    void ReturnToPatrol()
    {
        currentState = EnemyState.Patrolling;
        currentChaseTimer = 0f;

        if (agent != null && agent.enabled)
        {
            agent.ResetPath();
            agent.velocity = Vector3.zero;
        }

        if (patrolWaypoints.Count > 0 && agent != null && agent.enabled)
        {
            agent.destination = patrolWaypoints[currentWaypointIndex].position;
        }
    }

    IEnumerator WaitAndInvestigateWaypoint()
    {
        isWaitingAtWaypoint = true;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));

        if (currentState != EnemyState.Patrolling)
        {
            isWaitingAtWaypoint = false;
            yield break;
        }

        CalculateNextIndex();
        if (patrolWaypoints.Count > 0 && agent != null && agent.enabled)
        {
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

    void CheckPlayerDistanceAndTeleport()
    {
        if (currentState == EnemyState.Chasing || currentState == EnemyState.Cooldown) return;

        teleportCheckTimer += Time.deltaTime;
        if (teleportCheckTimer >= timeBetweenChecks)
        {
            teleportCheckTimer = 0f;

            Vector3 playerPositionAtSameHeight = new Vector3(player.position.x, transform.position.y, player.position.z);
            float currentDistanceToPlayer = Vector3.Distance(transform.position, playerPositionAtSameHeight);

            if (currentDistanceToPlayer < 10f)
            {
                return;
            }

            if (currentDistanceToPlayer >= maxDistanceToTeleport)
            {
                TeleportCloserToPlayer();
            }
        }
    }

    void TeleportCloserToPlayer()
    {
        Transform bestSpawnPoint = null;
        float closestDistanceToPlayer = Mathf.Infinity;

        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint == null) continue;
            float distanceToPlayer = Vector3.Distance(spawnPoint.position, player.position);

            if (distanceToPlayer >= minSpawnDistance && distanceToPlayer < closestDistanceToPlayer)
            {
                closestDistanceToPlayer = distanceToPlayer;
                bestSpawnPoint = spawnPoint;
            }
        }

        if (bestSpawnPoint != null)
        {
            agent.enabled = false;
            transform.position = bestSpawnPoint.position;
            transform.rotation = bestSpawnPoint.rotation;
            agent.enabled = true;

            FindClosestPatrolWaypointAfterTeleport();
        }
    }

    void FindClosestPatrolWaypointAfterTeleport()
    {
        if (patrolWaypoints.Count == 0) return;
        int closestIndex = 0;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < patrolWaypoints.Count; i++)
        {
            float dist = Vector3.Distance(transform.position, patrolWaypoints[i].position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestIndex = i;
            }
        }

        currentWaypointIndex = closestIndex;
        isWaitingAtWaypoint = false;
        agent.destination = patrolWaypoints[currentWaypointIndex].position;
    }

    void ControlAnimations()
    {
        if (anim != null)
        {
            float targetSpeedForAnimator = 0f;

            if (currentState == EnemyState.Patrolling)
            {
                targetSpeedForAnimator = (isWaitingAtWaypoint) ? 0f : patrolSpeed;
            }
            else if (currentState == EnemyState.Chasing)
            {
                targetSpeedForAnimator = chaseSpeed;
            }

            float currentAnimatorSpeed = anim.GetFloat("currentSpeed");
            float smoothedSpeed = Mathf.MoveTowards(currentAnimatorSpeed, targetSpeedForAnimator, Time.deltaTime * 15f);

            anim.SetFloat("currentSpeed", smoothedSpeed);
            anim.SetBool("isWaiting", isWaitingAtWaypoint && currentState != EnemyState.Cooldown);
            anim.SetBool("isCoolingDown", isSustainingCooldown);
        }
    }

    public void PlayFootstep()
    {
        if (footstepsAudioSource != null && footstepSound != null && agent.isStopped == false)
        {
            footstepsAudioSource.PlayOneShot(footstepSound);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hearingRange);
    }
}