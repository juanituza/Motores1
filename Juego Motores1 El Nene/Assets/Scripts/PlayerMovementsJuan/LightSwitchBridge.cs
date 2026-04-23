using LightMaster;
using UnityEngine;

// Este script conecta tu sistema de Raycast con Light Master
public class LightSwitchBridge : MonoBehaviour, IInteractable
{
    private LightAction _lightAction;

    void Start()
    {
        // Obtiene la referencia al componente del paquete
         _lightAction = GetComponent<LightAction>(); 
    }

    public void Interact()
    {
        if (_lightAction != null)
        {
          
             _lightAction.PerformAction(); 
        }
    }
}