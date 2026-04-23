using UnityEngine;
using UnityEngine.InputSystem;

public class ControladorNino : MonoBehaviour
{
    private CharacterController controller;
    private Vector2 inputMovimiento;
    private Vector3 direccion;

    public float velocidad = 4f;
    public float velocidadCorrer = 8f;
    public float gravedad = -9.81f;
    private float velocidadVertical;

    private bool estaCorriendo;


    // ... tus variables de movimiento de siempre ...

    [Header("Interacción")]
    [SerializeField] private float _interactRange = 5f;
    [SerializeField] private LayerMask _interactableLayer;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }



    // ESTE ES EL NUEVO MÉTODO QUE TENÉS QUE AGREGAR
  

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

        if (controller.isGrounded && velocidadVertical < 0)
        {
            velocidadVertical = -2f;
        }
        velocidadVertical += gravedad * Time.deltaTime;

        Vector3 movimientoCaida = new Vector3(0, velocidadVertical, 0);
        controller.Move(movimientoCaida * Time.deltaTime);
    }


      public void OnInteract(InputAction.CallbackContext context)
    {
        // Solo se ejecuta cuando presionás el botón (performed)
        if (context.performed)
        {
            // El rayo sale desde el centro de la pantalla
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            // Dibujamos el rayo en la Scene para que vos veas si llega al interruptor
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
}