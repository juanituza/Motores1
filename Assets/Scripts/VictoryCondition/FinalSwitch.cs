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

        // 1. Prende la luz del compañero
        LightSwitchInput luzCompañero = GetComponent<LightSwitchInput>();
        if (luzCompañero == null) luzCompañero = GetComponentInParent<LightSwitchInput>();
        if (luzCompañero == null) luzCompañero = GetComponentInChildren<LightSwitchInput>();

        if (luzCompañero != null)
        {
            luzCompañero.Interact();
        }

        // 2. Teletransporte forzado del Zombie
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

        // 3. Activación de la velocidad y modo implacable de forma directa
        EnemyAI zombieAI = zombiePrefab.GetComponent<EnemyAI>();
        if (zombieAI != null)
        {
            zombieAI.modoFinalImplacable = true; // Cambia la variable directamente
            zombieAI.currentState = EnemyAI.EnemyState.Chasing; // Fuerza el estado

            // --- CORRECCIÓN CLAVE: Declaramos y buscamos el NavMeshAgent ---
            UnityEngine.AI.NavMeshAgent zombieAgent = zombiePrefab.GetComponent<UnityEngine.AI.NavMeshAgent>();

            // Le seteamos la velocidad directo al agente desde acá por si acaso
            if (zombieAgent != null)
            {
                zombieAgent.speed = 5.5f; // Podés cambiar este número fijo a mano si querés (ej: 5.5f, 6f)
            }
        }
    }
}