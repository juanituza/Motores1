using UnityEngine;
using UnityEngine.AI;

public class FinalSwitch : MonoBehaviour
{
    [Header("Referencias del Final")]
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private Transform finalSpawnPoint;

    private bool interactuado = false;

    public void ActivarFinal()
    {
        if (interactuado) return;
        interactuado = true;

        LightSwitchInput luzCompañero = GetComponent<LightSwitchInput>();
        if (luzCompañero == null) luzCompañero = GetComponentInParent<LightSwitchInput>();
        if (luzCompañero == null) luzCompañero = GetComponentInChildren<LightSwitchInput>();

        if (luzCompañero != null)
        {
            luzCompañero.Interact();
        }

        if (zombiePrefab != null && finalSpawnPoint != null)
        {
            NavMeshAgent zombieAgent = zombiePrefab.GetComponent<NavMeshAgent>();

            if (zombieAgent != null)
            {
                zombieAgent.enabled = false;
                zombiePrefab.transform.position = finalSpawnPoint.position;
                zombiePrefab.transform.rotation = finalSpawnPoint.rotation;
                zombieAgent.enabled = true;
                zombieAgent.Warp(finalSpawnPoint.position);
            }
            else
            {
                zombiePrefab.transform.position = finalSpawnPoint.position;
                zombiePrefab.transform.rotation = finalSpawnPoint.rotation;
            }
        }

        EnemyAI zombieAI = zombiePrefab.GetComponent<EnemyAI>();
        if (zombieAI != null)
        {
            zombieAI.modoFinalImplacable = true;
            zombieAI.currentState = EnemyAI.EnemyState.Chasing;

            UnityEngine.AI.NavMeshAgent zombieAgent = zombiePrefab.GetComponent<UnityEngine.AI.NavMeshAgent>();

            if (zombieAgent != null)
            {
                zombieAgent.speed = 5.5f;
            }
        }
    }
}