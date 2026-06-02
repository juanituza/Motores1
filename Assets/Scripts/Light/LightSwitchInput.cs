using LightMaster;
using UnityEngine;

public class LightSwitchInput : MonoBehaviour
{
    public LightAction lightAction;
    public bool isLightOn = false;

    private SwitchAnimator switchAnimator;

    void Start()
    {
        switchAnimator = GetComponent<SwitchAnimator>();
    }

    public void Interact()
    {
        // Animate ALWAYS
        if (switchAnimator != null)
        {
            switchAnimator.ToggleSwitch();
        }

        // No electricity = no light
        if (!EnergyManager.powerEnabled)
        {
            Debug.Log("No electricity");
            return;
        }

        // Toggle light
        if (lightAction != null)
        {
            isLightOn = !isLightOn;
            lightAction.PerformAction();
        }
    }
    public void ForceTurnOff()
    {
        isLightOn = false;
    }
}