using UnityEngine;

public class NoteUnlocker : MonoBehaviour, IInteractable
{
    [Header("Conexión del Nivel")]
    [Tooltip("Arrastrá acá TODAS las puertas que querés que se desbloqueen")]
    // Al agregar [], le decimos a Unity que esto ahora es una lista
    [SerializeField] private Transform[] doorTransformsToUnlock;

    [Header("Textos de Historia y Misión")]
    [SerializeField] private string noteThought = "Esto habla sobre el pasillo... Debería intentar abrir la puerta ahora.";
    [SerializeField] private string nextMission = "Investiga el pasillo oscuro";

    private bool alreadyRead = false;

    public void Interact()
    {
        if (alreadyRead) return;

        // 1. DESBLOQUEAMOS TODAS LAS PUERTAS DE LA LISTA
        if (doorTransformsToUnlock != null && doorTransformsToUnlock.Length > 0)
        {
            // El bucle foreach pasa por cada Transform que hayas puesto en el Inspector
            foreach (Transform doorTransform in doorTransformsToUnlock)
            {
                if (doorTransform != null)
                {
                    DoorController door = doorTransform.GetComponentInChildren<DoorController>();

                    if (door != null)
                    {
                        door.UnlockDoor();
                    }
                    else
                    {
                        Debug.LogWarning($"El objeto {doorTransform.name} no tiene un DoorController asignado.");
                    }
                }
            }
        }

        // 2. Textos del HUD
        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.ShowEphemeralText(noteThought, 4f);
            HUDManager.Instance.UpdateMissionAnimated(nextMission);
        }

        alreadyRead = true;

        // 3. Nos apagamos
        GetComponent<Collider>().enabled = false;
        gameObject.layer = LayerMask.NameToLayer("Default");
    }
}