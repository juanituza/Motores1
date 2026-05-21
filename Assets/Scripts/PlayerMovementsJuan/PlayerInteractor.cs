using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float _interactRange = 13f;
    [SerializeField] private LayerMask _interactableLayer;

    // Se llama cuando haces click (porque la acción se llama Interact)
    void OnInteract(InputValue value)
    {
        // Solo ejecutamos la lógica cuando se presiona el botón
        if (value.isPressed)
        {
            // Lanzamos el rayo desde el centro de la pantalla
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