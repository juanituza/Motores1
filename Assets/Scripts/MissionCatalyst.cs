using UnityEngine;

// Firmamos el contrato IInteractable para que el Raycast del jugador lo detecte
public class MissionCatalyst : MonoBehaviour, IInteractable
{
    [Header("Configuración de Misión")]
    [Tooltip("El texto de la nueva misión que va a aparecer")]
    [SerializeField] private string nextMission = "Encuentra una salida...";  

    [Tooltip("Desactiva este script tras usarlo para no repetir la animación")]
    [SerializeField] private bool disableAfterTrigger = true;

    // Este es el método obligatorio de nuestra interfaz
    public void Interact()
    {
        // 1. Lanzamos la animación del tachado en el HUD
        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.UpdateMissionAnimated(nextMission);

        }

        // 2. Nos auto-apagamos para que el jugador no pueda spamear el botón
        if (disableAfterTrigger)
        {
            this.enabled = false;
            // Nota: Podrías también cambiarle el layer al objeto si querés 
            // que la mano del cursor deje de aparecer al mirarlo.
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }
}