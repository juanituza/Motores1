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

    private void Awake()
    {
        // Al instanciarse el Prefab, busca la cámara de la escena automáticamente
        if (playerCamera == null)
        {
            playerCamera = Camera.main;

            if (playerCamera == null)
            {
                Debug.LogError("PlayerInteractor: ¡No se encontró ninguna Main Camera en la escena!");
            }
        }
    }
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

    public void FixedUpdate()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
        {
            // Si miramos algo interactuable, prende la mano
            if (hit.collider.GetComponent<IInteractable>() != null)
            {
                HUDManager.Instance.SetInteractIcon(true);
                return;
            }
        }
        // Si no impacta nada o no es interactuable, vuelve al puntito
        HUDManager.Instance.SetInteractIcon(false);
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