using UnityEngine;
using System.Collections;

// Firmamos el contrato IInteractable para que el jugador pueda hacerle clic
public class FinalDoorTrigger : MonoBehaviour, IInteractable
{
    [Header("Configuración del Final")]
    [Tooltip("Texto que piensa el nene justo antes de que corte la escena")]
    [SerializeField] private string finalThought = "Por fin... la salida.";

    [Tooltip("Tiempo de suspenso antes de cambiar a la escena del grito")]
    [SerializeField] private float suspenseDelay = 1.5f;

    private bool alreadyTriggered = false;

    public void Interact()
    {
        // Evitamos que el jugador haga mil clics por nerviosismo
        if (alreadyTriggered) return;
        alreadyTriggered = true;

        // 1. Mostramos el pensamiento final
        if (HUDManager.Instance != null && !string.IsNullOrEmpty(finalThought))
        {
            HUDManager.Instance.ShowEphemeralText(finalThought, suspenseDelay);
        }

        // 2. Apagamos el collider para desactivar el ícono de la mano
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
        gameObject.layer = LayerMask.NameToLayer("Default");

        // 3. Iniciamos la pequeña pausa de suspenso antes del susto
        StartCoroutine(TriggerEndingRoutine());
    }

    private IEnumerator TriggerEndingRoutine()
    {
        // Pausa dramática
        yield return new WaitForSeconds(suspenseDelay);

        // Llamamos a tu GameManager para que haga el cambio de escena
        if (NewGameManager.Instance != null)
        {
            NewGameManager.Instance.TriggerEnding();
        }
        else
        {
            Debug.LogError("FinalDoorTrigger: ¡No se encontró el NewameManager.Instance en la escena!");
        }
    }
}