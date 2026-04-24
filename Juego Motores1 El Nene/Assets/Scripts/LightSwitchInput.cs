using LightMaster;
using UnityEngine;

public class LightSwitchInput : MonoBehaviour
{
    public LightAction lightAction;
    public float maxDistance = 2f;
    public LayerMask interactableLayer;

    private bool _hasBeenCounted = false;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxDistance, interactableLayer))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    ExecuteSwitchAction();
                }
            }
        }
    }

    private void ExecuteSwitchAction()
    {
        if (lightAction != null)
        {
            lightAction.PerformAction();
            if (!_hasBeenCounted)
            {
                _hasBeenCounted = true;

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.RegisterLightOn();
                }

                Debug.Log("Light registered for victory condition!");
            }
        }
    }
}