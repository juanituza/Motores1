using LightMaster;
using UnityEngine;

public class LightSwitchBridge : MonoBehaviour, IInteractable
{
    private LightAction _lightAction;
    private bool _hasBeenActivated = false; // Paso 2.1: Control para no sumar de más

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

            // --- PASO 2: AVISAR AL GAMEMANAGER ---
            if (!_hasBeenActivated)
            {
                _hasBeenActivated = true; // Marcamos que este interruptor ya aportó a la victoria

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.RegisterLightOn();
                }
                else
                {
                    Debug.LogError("¡Rami, no encontré el GameManager en la escena!");
                }
            }
            // -------------------------------------
        }
    }
}