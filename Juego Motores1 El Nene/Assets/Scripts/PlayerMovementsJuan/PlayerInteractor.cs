using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float _interactRange = 3f;
    [SerializeField] private LayerMask _interactableLayer;

    // Se llama automáticamente cuando presionas "E" (si la acción se llama Interact)
    void OnInteract(InputValue value)
    {
        if (value.isPressed)
        {
            float range = 3f;
            // Lanzamos el rayo desde el centro de la cámara (o del jugador) hacia adelante
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, range, _interactableLayer))
            {
                // 1. Mensaje de prueba para ver en la Consola
                Debug.Log("¡Rayo chocó con: " + hit.collider.name + "!");

                // 2. Intentamos obtener el script (SOLO UNA VEZ)
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();

                // 3. Si lo encontramos, ejecutamos la acción
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }

        }
    }
}