using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Configuración de Input")]
    [Tooltip("Arrastrá acá tu acción 'Raycast Interaction' desde el archivo .inputactions")]
    [SerializeField] private InputActionReference interactAction;

    [Header("Configuración del Raycast")]
    [SerializeField] private float interactRange = 3f; // Distancia máxima para interactuar (3 metros es realista)
    [SerializeField] private LayerMask interactableLayer; // Asegurate de asignar la capa de las puertas/notas acá
    [SerializeField] private Camera playerCamera; // Referencia directa a la cámara para evitar errores de tags

    private void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.action.Enable();
            interactAction.action.performed += PerformInteraction;
        }
    }

    private void OnDisable()
    {
       
        if (interactAction != null)
        {
            interactAction.action.performed -= PerformInteraction;
            interactAction.action.Disable();
        }
    }

    private void PerformInteraction(InputAction.CallbackContext context)
    {
        if (playerCamera == null)
        {
            Debug.LogWarning("PlayerInteractor: ¡Te olvidaste de asignar la cámara en el Inspector!");
            return;
        }

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        // DIBUJO DE DEBUG
        Debug.DrawRay(ray.origin, ray.direction * interactRange, Color.red, 2f);

        // Disparamos el Raycas
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
        {
            Debug.Log("El rayo impactó contra: " + hit.collider.name);

            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }
}