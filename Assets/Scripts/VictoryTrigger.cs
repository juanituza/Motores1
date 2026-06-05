using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryTrigger : MonoBehaviour
{
    [Header("Escena del Video Final")]
    [SerializeField] private string nombreEscenaVideo = "Final";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("¡Cruzaste la puerta! Cargando video cinematográfico...");

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            NewGameManager.Instance.TriggerEnding();
        }
    }
}
