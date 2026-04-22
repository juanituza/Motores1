using UnityEngine;

public class CamaraSeguimiento : MonoBehaviour
{
    [Header("Configuración de Seguimiento")]
    public Transform objetivo; // Arrastra tu cápsula aquí
    public float suavizado = 0.125f;

    [Header("Posición Relativa (Offset)")]
    // Estos valores son para que se vea como en tu imagen
    public Vector3 separacion = new Vector3(0, 2.5f, -5f);

    void LateUpdate()
    {
        if (objetivo == null) return;

        // Calculamos la posición deseada sumando la separación a la posición del jugador
        Vector3 posicionDeseada = objetivo.position + separacion;

        // Aplicamos el suavizado (Lerp) para que no sea un movimiento rígido
        Vector3 posicionSuavizada = Vector3.Lerp(transform.position, posicionDeseada, suavizado);

        transform.position = posicionSuavizada;

        // Esto hace que la cámara siempre apunte al jugador, dándole esa inclinación
        transform.LookAt(objetivo.position + Vector3.up * 1.5f);
    }
}