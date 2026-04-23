using UnityEngine;

public class Selected : MonoBehaviour
{
    [SerializeField] private float distance = 3f;
  
    [SerializeField] private LayerMask capaInteractuable;

    private void Update()
    {
        Vector3 rayOrigin = transform.position + new Vector3(0, 1.5f, 0);
        Vector3 rayDirection = transform.TransformDirection(Vector3.forward);

        Debug.DrawRay(rayOrigin, rayDirection * distance, Color.red);

        RaycastHit hit;

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, distance, capaInteractuable))
        {
            Debug.Log("El rayo está chocando contra: " + hit.collider.gameObject.name);

            if (hit.collider.CompareTag("Door"))
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    SystemDoor doorScript = hit.collider.GetComponent<SystemDoor>();
                    if (doorScript != null)
                    {
                        doorScript.Interact();
                    }
                }
            }
        }
    }
}