using UnityEngine;

public class CamaraSeguimiento : MonoBehaviour
{
    [Header("Configuración de Seguimiento")]
    public Transform objetivo; 
    public float suavizado = 0.125f;

    [Header("Posición Relativa (Offset)")]
    
    public Vector3 separacion = new Vector3(0, 2.5f, -5f);

    void LateUpdate()
    {
        if (objetivo == null) return;

       
        Vector3 posicionDeseada = objetivo.position + separacion;

        // suavizado (Lerp) para que no sea un movimiento rigido
        Vector3 posicionSuavizada = Vector3.Lerp(transform.position, posicionDeseada, suavizado);

        transform.position = posicionSuavizada;

        // camara apunta al jugador dando inclinacion
        transform.LookAt(objetivo.position + Vector3.up * 1.5f);
    }
}