using LightMaster;
using UnityEngine;

public class MainPowerSwitch : MonoBehaviour
{
    [Header("Emergency OFF Actions")]
    public Light[] houseLights;
    public LightSwitchInput[] houseSwitches;

    [Header("Audio")]
    public AudioSource blackoutAudio;

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

            if (blackoutAudio != null)
            {
                blackoutAudio.Play();
            }

            foreach (Light lightSource in houseLights)
            {
                if (lightSource != null)
                {
                    lightSource.enabled = false;
                }
            }
            foreach (LightSwitchInput lightSwitch in houseSwitches)
            {
                if (lightSwitch != null)
                {
                    lightSwitch.ForceTurnOff();
                }
            }
        }

    }

    // Podés agregar esto debajo de tu método Interact()
    public void ForceBlackout()
    {
        if (EnergyManager.powerEnabled == true)
        {
            Interact(); // Solo baja la palanca si la luz estaba prendida
        }
    }
}