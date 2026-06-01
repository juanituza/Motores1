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

        Debug.Log("Main Power: " + currentState);

        // Animate thermal switch
        if (switchAnimator != null)
        {
            switchAnimator.ToggleSwitch();
        }

        // Turn OFF all lights only when power goes OFF
        if (!currentState)
        {
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