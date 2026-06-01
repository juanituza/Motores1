using UnityEngine;
using UnityEngine.AI; 
using System.Collections;

public class EnemyDoorInteractor : MonoBehaviour
{
    [SerializeField] private float _interactRange = 3f;
    [SerializeField] private LayerMask _interactableLayer;

    
    [SerializeField] private Transform rayOrigin;

    [Header("Waiting time")]
    [SerializeField] private float coolDown = 2f;
    [SerializeField] private float waitingForTheDoor = 1.2f;
    private float _nextInteractTime = 0f;

    private NavMeshAgent agent;
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }



    void Update()
    {
        
        if (Time.time < _nextInteractTime) return;

        
        Vector3 origin = rayOrigin != null ? rayOrigin.position : transform.position;
        Vector3 direction = transform.forward;

       
        Debug.DrawRay(origin, direction * _interactRange, Color.magenta);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, _interactRange, _interactableLayer))
        {
            
            SystemDoor puerta = hit.collider.GetComponent<SystemDoor>();

            
            if (puerta != null)
            {
                puerta.Interact();

                _nextInteractTime = Time.time + coolDown;
                StartCoroutine(PauseAgent());

            }
        }
    }
    private IEnumerator PauseAgent() // frena al enemigo para que no choque con la puerta
    {
        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero; 

            
            yield return new WaitForSeconds(waitingForTheDoor);
            agent.isStopped = false;
        }
    }
}