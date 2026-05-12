using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

public class IAEnemigo : MonoBehaviour
{
    private NavMeshAgent _agent;

    [SerializeField] private Transform player;
    private PlayerStats _playerStats;
    [SerializeField] private List<Transform> wayPoints;
    [SerializeField] private float patrolWaitTime = 2f;
    private int _currentWayPointIndex = 0;
    private bool _isWaiting = false;
    private bool _isAttacking = true;

    [SerializeField] private float normalSpeed = 2.5f;
    [SerializeField] private float _damageCooldown = 2f;
    private float _lastDamageTime;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;

        if (player != null) _playerStats = player.GetComponent<PlayerStats>();
    }

    private void Start()
    {
        if (wayPoints.Count > 0) UpdatePatrolDestination();
    }

    private void Update()
    {
        if (_isAttacking) return;
        if (!_isWaiting) Patrol();

        ApplyManualRotation();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Time.time > _lastDamageTime + _damageCooldown)
            {
                if (_playerStats != null)
                {
                    _playerStats.TakeDamage();
                    _lastDamageTime = Time.time;
                    Debug.Log("Player hit by enemy!");
                }
            }
        }
    }

    private void ApplyManualRotation()
    {
        Vector3 rotationDirection = _agent.velocity;
        if (_isAttacking) rotationDirection = (player.position - transform.position);

        if (rotationDirection.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(rotationDirection.x, 0, rotationDirection.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    private void Patrol()
    {
        _agent.isStopped = false;
        _agent.speed = normalSpeed;
        if (!_agent.pathPending && _agent.remainingDistance < 0.5f) StartCoroutine(WaitAtPoint());
    }

    private IEnumerator WaitAtPoint()
    {
        _isWaiting = true;
        _agent.isStopped = true;
        yield return new WaitForSeconds(patrolWaitTime);
        _currentWayPointIndex = (_currentWayPointIndex + 1) % wayPoints.Count;
        UpdatePatrolDestination();
        _agent.isStopped = false;
        _isWaiting = false;
    }

    private void UpdatePatrolDestination()
    {
        _agent.SetDestination(wayPoints[_currentWayPointIndex].position);
    }
}