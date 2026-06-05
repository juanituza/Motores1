using LightMaster;
using UnityEngine;
using System.Collections;

public class MainPowerSwitch : MonoBehaviour
{
    [Header("Emergency OFF Actions")]
    public Light[] houseLights;
    public LightSwitchInput[] houseSwitches;

    [Header("Audio")]
    public AudioSource blackoutAudio;

    [Header("Flicker Settings")]
    public float minOnTime = 0.05f;
    public float maxOnTime = 0.3f;
    public float minOffTime = 0.05f;
    public float maxOffTime = 0.2f;

    private SwitchAnimator switchAnimator;
    private Coroutine[] flickerCoroutines;
    private bool isFlickering = false;

    void Start()
    {
        switchAnimator = GetComponent<SwitchAnimator>();
    }

    public void Interact()
    {
        EnergyManager.TogglePower();
        bool currentState = EnergyManager.powerEnabled;
        Debug.Log("Electricity Enabled: " + currentState);

        if (switchAnimator != null)
            switchAnimator.ToggleSwitch();

        if (currentState == false)
        {
            Debug.Log("Power OFF -> Turning off all lights");

            if (blackoutAudio != null)
                blackoutAudio.Play();

            foreach (Light lightSource in houseLights)
                if (lightSource != null)
                    lightSource.enabled = false;

            foreach (LightSwitchInput lightSwitch in houseSwitches)
                if (lightSwitch != null)
                    lightSwitch.ForceTurnOff();
        }
    }

    public void ForceBlackout()
    {
        if (EnergyManager.powerEnabled == true)
        {
            StopFlickering();
            Interact();
        }
    }

    public void StartFlickering()
    {
        if (isFlickering) return;
        isFlickering = true;

        flickerCoroutines = new Coroutine[houseLights.Length];
        for (int i = 0; i < houseLights.Length; i++)
        {
            if (houseLights[i] != null)
                flickerCoroutines[i] = StartCoroutine(FlickerLight(houseLights[i]));
        }
    }

    public void StopFlickering()
    {
        isFlickering = false;

        if (flickerCoroutines != null)
            foreach (var c in flickerCoroutines)
                if (c != null) StopCoroutine(c);
    }

    private IEnumerator FlickerLight(Light light)
    {
        while (isFlickering)
        {
            light.enabled = true;
            yield return new WaitForSeconds(Random.Range(minOnTime, maxOnTime));
            light.enabled = false;
            yield return new WaitForSeconds(Random.Range(minOffTime, maxOffTime));
        }
    }
}