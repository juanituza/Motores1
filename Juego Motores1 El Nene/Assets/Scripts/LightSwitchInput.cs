using LightMaster;
using UnityEngine;

public class LightSwitchInput : MonoBehaviour
{
    public LightAction lightAction;

    [Header("Configuración de Cercanía")]
    public float distanciaMaxima = 2f; // Ajusta esto: más pequeño = más cerca tienes que estar
    public LayerMask capaInteractuable;  // Aquí seleccionarás "Interactable" en el Inspector

    void Update()
    {
        // 1. Detectar el clic del mouse
        if (Input.GetMouseButtonDown(0))
        {
            // 2. Lanzar un rayo desde el centro de la pantalla (donde apunta la cámara)
            Ray rayo = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            // 3. Verificar si el rayo toca ALGO dentro de la distancia máxima
            if (Physics.Raycast(rayo, out hit, distanciaMaxima, capaInteractuable))
            {
                // 4. Verificar si ese "algo" es ESTE interruptor
                if (hit.collider.gameObject == gameObject)
                {
                    lightAction.PerformAction();
                }
            }
        }
    }
}