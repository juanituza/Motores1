using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class WindowBreakEvent : MonoBehaviour
{
    [Header("Window Components")]
    public AudioSource breakAudio;
    public MeshRenderer windowMesh;

    [Header("Zombie Spawn")]
    public GameObject zombieEnemy;
    public Transform zombieSpawnPoint;

    [Header("Player Effects")]
    public AudioClip playerAgitationClip;


    public void TriggerWindowEvent()
    {
        float delayDelVidrio = 0f;

        if (breakAudio != null)
        {
            breakAudio.Play();
            if (breakAudio.clip != null) delayDelVidrio = breakAudio.clip.length;
        }

        if (zombieEnemy != null && zombieSpawnPoint != null)
        {
            zombieEnemy.SetActive(true);

            NavMeshAgent agent = zombieEnemy.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.enabled = false; 
                zombieEnemy.transform.position = zombieSpawnPoint.position;
                zombieEnemy.transform.rotation = zombieSpawnPoint.rotation;
                agent.enabled = true;

                agent.Warp(zombieSpawnPoint.position); 
            }

            
            EnemyAI zombieAI = zombieEnemy.GetComponent<EnemyAI>();
            if (zombieAI != null)
            {
                zombieAI.currentState = EnemyAI.EnemyState.Patrolling;
                zombieAI.modoFinalImplacable = false;
            }
        }

        StartCoroutine(PlayPlayerAgitationRoutine(delayDelVidrio));
        UnlockAllDoors();

        StartCoroutine(HideWindowRoutine());
    }

    private IEnumerator PlayPlayerAgitationRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (playerAgitationClip == null) yield break;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            AudioSource playerAudio = player.GetComponent<AudioSource>();
            if (playerAudio == null)
            {
                playerAudio = player.AddComponent<AudioSource>();
            }

            playerAudio.PlayOneShot(playerAgitationClip);
        }
    }

    private void UnlockAllDoors()
    {
        DoorController[] allDoors = FindObjectsByType<DoorController>(FindObjectsSortMode.None);

        foreach (DoorController door in allDoors)
        {
            door.UnlockDoor();
        }

        Debug.Log("Event triggered: Unlocked " + allDoors.Length + " doors.");
    }

    private IEnumerator HideWindowRoutine()
    {
        yield return new WaitForSeconds(2f);
        if (windowMesh != null) windowMesh.enabled = false;
    }
}