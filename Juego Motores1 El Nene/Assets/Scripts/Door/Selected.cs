using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class Selected : MonoBehaviour
{
    [SerializeField] private float distance = 3f;
    private void Update()
    {
        // 1. Levantamos el origen del rayo para que no salga desde los pies. 
        // Ajusta el "1.5f" según la altura de tu personaje (pecho/cabeza)
        Vector3 rayOrigin = transform.position + new Vector3(0, 1.5f, 0);
        Vector3 rayDirection = transform.TransformDirection(Vector3.forward);

        // 2. DIBUJAMOS EL RAYO: Esto creará una línea roja en la pestaña "Scene" de Unity
        Debug.DrawRay(rayOrigin, rayDirection * distance, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, distance))
        {
            // Esto te dirá exactamente contra qué pared o collider está chocando el rayo
            Debug.Log("El rayo está chocando contra: " + hit.collider.gameObject.name);

            if (hit.collider.CompareTag("Door"))
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    hit.collider.GetComponent<SystemDoor>().ChangeDoorState();
                }
            }
        }
    }

}
