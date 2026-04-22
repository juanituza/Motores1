using UnityEngine;
using UnityEngine.InputSystem;

public class MyPlayerMovement : MonoBehaviour
{
    [SerializeField] private float _currentSpeed = 5f;

    void Update()
    {
        // 1. Verificación de seguridad del nuevo Input System
        if (Keyboard.current == null) return;

        // 2. Leemos los valores del teclado
        float h = 0; // Horizontal (A/D)
        float v = 0; // Vertical (W/S)

        if (Keyboard.current.wKey.isPressed) v = 1;
        if (Keyboard.current.sKey.isPressed) v = -1;
        if (Keyboard.current.aKey.isPressed) h = -1;
        if (Keyboard.current.dKey.isPressed) h = 1;

        // 3. Encapsulamiento del movimiento
        // 'h' mueve en el eje X (izquierda/derecha)
        // 'v' mueve en el eje Z (adelante/atrás)
        Vector3 move = new Vector3(h, 0, v).normalized;

        // 4. Aplicación del movimiento
        // Usamos Space.World para que se mueva respecto al mundo 
        // o Space.Self si querés que respete la orientación actual del objeto.
        transform.Translate(move * _currentSpeed * Time.deltaTime, Space.World);

        // 5. Cálculo de magnitud usando tu método
        float magnitude = CalculateMagnitude(move);
    }

    // Tu método para el cálculo de magnitudes
    public float CalculateMagnitude(Vector3 vector)
    {
        return vector.magnitude;
    }
}