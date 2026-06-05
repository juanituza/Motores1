using LightMaster;
using UnityEngine;

public class LastSwitchTrigger : MonoBehaviour
{
    [Header("Referencia al MainPowerSwitch")]
    public MainPowerSwitch mainPowerSwitch;

    [Header("Cantidad de switches para activar el flicker")]
    public int switchesRequiredToFlicker = 4;

    private LightSwitchInput[] allSwitches;
    private bool triggered = false;

    void Start()
    {
        // Busca TODOS los objetos con LightSwitchInput en la escena automáticamente
        allSwitches = FindObjectsByType<LightSwitchInput>(FindObjectsSortMode.None);
        Debug.Log($"Switches encontrados: {allSwitches.Length}");
    }

    void Update()
    {
        if (triggered) return;

        int activeCount = 0;
        foreach (var sw in allSwitches)
        {
            if (sw != null && sw.isLightOn)
                activeCount++;
        }

        if (activeCount >= switchesRequiredToFlicker)
        {
            triggered = true;
            Debug.Log($"{activeCount} switches encendidos → StartFlickering");
            mainPowerSwitch.StartFlickering();
        }
    }
}