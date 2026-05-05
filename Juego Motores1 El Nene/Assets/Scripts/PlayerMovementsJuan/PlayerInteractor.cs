using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float _interactRange = 13f;
    [SerializeField] private LayerMask _interactableLayer;

   
    void OnInteract(InputValue value)
    {
        // Logica usada cuando se presiona la tecla
        if (value.isPressed)
        {
           
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            if (Physics.Raycast(ray, out RaycastHit hit, _interactRange, _interactableLayer))
            {
                Debug.Log("Click en: " + hit.collider.name);

                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }
    }
}