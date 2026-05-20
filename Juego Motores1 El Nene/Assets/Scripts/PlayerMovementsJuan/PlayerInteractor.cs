using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float _interactRange = 4f; 
    [SerializeField] private LayerMask _interactableLayer;

    
    public void OnInteract(InputAction.CallbackContext context)
    {
       
        if (context.performed)
        {
            
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            
            Debug.DrawRay(ray.origin, ray.direction * _interactRange, Color.red, 2f);

            if (Physics.Raycast(ray, out RaycastHit hit, _interactRange, _interactableLayer))
            {
                
                Debug.Log("El rayo golpeó: " + hit.collider.name);

                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }
    }
}