using LightMaster;
using UnityEngine;

public class LightSwitchInput : MonoBehaviour
{
    public LightAction lightAction;

    [Header("Configuración de Cercanía")]
    public float distanciaMaxima = 2f; 
    public LayerMask capaInteractuable;  

    void Update()
    {
       
        if (Input.GetMouseButtonDown(0))
        {
            
            Ray rayo = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            
            if (Physics.Raycast(rayo, out hit, distanciaMaxima, capaInteractuable))
            {
               
                if (hit.collider.gameObject == gameObject)
                {
                    lightAction.PerformAction();
                }
            }
        }
    }
}