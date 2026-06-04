using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class WindowBreakEvent : MonoBehaviour
{
    [Header("Componentes de la Ventana")]
   [SerializeField] private AudioSource breakAudio;  
    [SerializeField] private MeshRenderer windowMesh; // el vidrio

    [Header("Aparición del Zombie")]
    public GameObject zombieEnemy; 
    public Transform zombieSpawnPoint; 

    public void TriggerWindowEvent()
    {
        
        if (breakAudio != null) breakAudio.Play();

        // Activa zombie
        if (zombieEnemy != null && zombieSpawnPoint != null)
        {
            zombieEnemy.SetActive(true); 

            NavMeshAgent agent = zombieEnemy.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                
                agent.Warp(zombieSpawnPoint.position);
            }
        }

        StartCoroutine(HideWindowRoutine());
    }

    private IEnumerator HideWindowRoutine()
    {
        yield return new WaitForSeconds(2f);
        if (windowMesh != null) windowMesh.enabled = false;
    }
}
