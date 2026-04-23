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

        if (controller.isGrounded && velocidadVertical < 0)
        {
            velocidadVertical = -2f;
        }
        velocidadVertical += gravedad * Time.deltaTime;

        Vector3 movimientoCaida = new Vector3(0, velocidadVertical, 0);
        controller.Move(movimientoCaida * Time.deltaTime);
    }
}