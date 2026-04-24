using LightMaster;
using UnityEngine;

public class LightSwitchBridge : MonoBehaviour, IInteractable
{
    private LightAction _lightAction;

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
            // Verificamos si el Controller de la acción tiene su luz asignada
            // Esto previene el NullReference que vimos antes
            var controller = _lightAction.GetComponent<LightController>();
            if (controller != null)
            {
                // Forzamos el encendido del componente Light si el paquete se olvidó
                // o si el Awake todavía no corrió correctamente
                controller.Interact();
            }

            _lightAction.PerformAction();
            Debug.Log($"[Bridge] Action performed on {gameObject.name}");
        }
    }
}