using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Input Settings")]
    [Tooltip("Assign your 'Raycast Interaction' action here")]
    [SerializeField] private InputActionReference interactAction;

    [Header("Raycast Settings")]
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Camera playerCamera;

    private void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

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

    private void FixedUpdate()
    {
        if (playerCamera == null || HUDManager.Instance == null)
            return;

        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0)
        );

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
        {
            bool hasInteraction =
                hit.collider.GetComponentInParent<LightSwitchInput>() != null ||
                hit.collider.GetComponentInParent<MainPowerSwitch>() != null ||
                hit.collider.GetComponent<IInteractable>() != null;

            HUDManager.Instance.SetInteractIcon(hasInteraction);
            return;
        }

        HUDManager.Instance.SetInteractIcon(false);
    }

    private void PerformInteraction(InputAction.CallbackContext context)
    {
        if (playerCamera == null)
        {
            Debug.LogWarning("PlayerInteractor: Missing camera reference!");
            return;
        }

        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0)
        );

        Debug.DrawRay(ray.origin, ray.direction * interactRange, Color.red, 2f);

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
        {
            Debug.Log("Ray hit: " + hit.collider.name);

            // LIGHT SWITCH
            LightSwitchInput lightSwitch =
                hit.collider.GetComponentInParent<LightSwitchInput>();

            if (lightSwitch != null)
            {
                lightSwitch.Interact();
                return;
            }

            // MAIN POWER SWITCH
            MainPowerSwitch mainPower =
                hit.collider.GetComponentInParent<MainPowerSwitch>();

            if (mainPower != null)
            {
                mainPower.Interact();
                return;
            }

            // GENERIC INTERACTABLE
            IInteractable interactable =
                hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }
}