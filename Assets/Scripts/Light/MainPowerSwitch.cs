using LightMaster;
using UnityEngine;

public class MainPowerSwitch : MonoBehaviour
{
    [Header("Emergency OFF Actions")]
    public LightAction[] emergencyOffLights;

    private SwitchAnimator switchAnimator;

    void Start()
    {
        switchAnimator = GetComponent<SwitchAnimator>();
    }

    public void Interact()
    {
        // Toggle electricity
        EnergyManager.TogglePower();

        bool currentState = EnergyManager.powerEnabled;

        Debug.Log("Electricity Enabled: " + currentState);

        // Animate switch
        if (switchAnimator != null)
        {
            switchAnimator.ToggleSwitch();
        }

        // ONLY when electricity goes OFF
        if (currentState == false)
        {
            Debug.Log("Power OFF -> Turning off all lights");

            foreach (LightAction action in emergencyOffLights)
            {
                if (action != null)
                {
                    action.PerformAction();
                }
            }
        }
    }
}