using UnityEngine;
using UnityEngine.InputSystem;

public class ControladorNino : MonoBehaviour
{
    private CharacterController controller;
    private Vector2 inputMovimiento;
    private Vector3 direccion;

    public float velocidad = 100f;
    public float velocidadCorrer = 150f;
    public float gravedad = -70f;
    public float fuerzaSalto = 10f;
    private float velocidadVertical;
    private bool estaCorriendo;

    [Header("Interacción")]
    [SerializeField] private float _interactRange = 5f;
    [SerializeField] private LayerMask _interactableLayer;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public void Moverse(InputAction.CallbackContext context)
    {
        inputMovimiento = context.ReadValue<Vector2>();
    }

    public void AlCorrer(InputAction.CallbackContext context)
    {
        if (context.performed) estaCorriendo = true;
        if (context.canceled) estaCorriendo = false;
    }

    void Update()
    {
        float velocidadActual = estaCorriendo ? velocidadCorrer : velocidad;

        direccion = transform.right * inputMovimiento.x + transform.forward * inputMovimiento.y;
        controller.Move(direccion * velocidadActual * Time.deltaTime);

        if (controller.isGrounded)
        {
            if (velocidadVertical < 0)
            {
                velocidadVertical = -50f; 
            }
        }
        else
        {
            velocidadVertical += gravedad * Time.deltaTime;
        }
        // -------------------------------------

        Vector3 movimientoCaida = new Vector3(0, velocidadVertical, 0);
        controller.Move(movimientoCaida * Time.deltaTime);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            Debug.DrawRay(ray.origin, ray.direction * _interactRange, Color.red, 2f);

            if (Physics.Raycast(ray, out RaycastHit hit, _interactRange, _interactableLayer))
            {
                Debug.Log("¡Golpeé a: " + hit.collider.name + "!");

                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
            else
            {
                Debug.Log("El rayo no chocó con nada interactuable.");
            }
        }
    }
    public void Saltar(InputAction.CallbackContext context)
    {
        if (context.performed && controller.isGrounded)
        {
            velocidadVertical = Mathf.Sqrt(fuerzaSalto * -2f * gravedad);
        }
    }
}