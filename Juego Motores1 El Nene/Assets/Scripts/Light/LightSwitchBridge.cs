using LightMaster;
using UnityEngine;

public class LightSwitchBridge : MonoBehaviour, IInteractable
{
    private LightAction _lightAction;
    private bool _hasBeenActivated = false;

    void Start()
    {
        _lightAction = GetComponent<LightAction>();

        if (_lightAction == null)
        {
            Debug.LogError($"[Bridge] LightAction missing on {gameObject.name}");
        }
    }

    public void Interact()
    {
        if (_lightAction != null)
        {
            var controller = _lightAction.GetComponent<LightController>();
            if (controller != null)
            {
                controller.Interact();
            }

            _lightAction.PerformAction();
            Debug.Log($"[Bridge] Action performed on {gameObject.name}");

        
            if (!_hasBeenActivated)
            {
                _hasBeenActivated = true;

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.RegisterLightOn();
                }
                else
                {
                    Debug.LogError("No se encontro la escena");
                }
            }
            
        }
    }
}