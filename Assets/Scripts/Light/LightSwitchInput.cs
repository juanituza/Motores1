using LightMaster;
using UnityEngine;

public class LightSwitchInput : MonoBehaviour
{
    public LightAction lightAction;

    private SwitchAnimator switchAnimator;

    void Start()
    {
        switchAnimator = GetComponent<SwitchAnimator>();
    }

    public void Interact()
    {
        // ALWAYS animate switch
        if (switchAnimator != null)
        {
            switchAnimator.ToggleSwitch();
        }

        // NO electricity
        if (!EnergyManager.powerEnabled)
        {
            Debug.Log("No power available.");
            return;
        }

        // Toggle light
        if (lightAction != null)
        {
            lightAction.PerformAction();
        }
    }
}