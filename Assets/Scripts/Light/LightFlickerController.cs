using UnityEngine;
using System.Collections;

public class LightFlickerController : MonoBehaviour
{
    [Header("Luces a parpadear")]
    public Light[] lightsToFlicker;

    [Header("Configuración del parpadeo")]
    public float minOnTime = 0.05f;
    public float maxOnTime = 0.3f;
    public float minOffTime = 0.05f;
    public float maxOffTime = 0.2f;

    private bool isFlickering = false;
    private Coroutine[] flickerCoroutines;

    public void StartFlickering()
    {
        if (isFlickering) return;
        isFlickering = true;

        flickerCoroutines = new Coroutine[lightsToFlicker.Length];
        for (int i = 0; i < lightsToFlicker.Length; i++)
        {
            if (lightsToFlicker[i] != null)
            {
                lightsToFlicker[i].enabled = true; // Asegurate que estén encendidas antes
                flickerCoroutines[i] = StartCoroutine(FlickerLight(lightsToFlicker[i]));
            }
        }
    }

    public void StopFlickering()
    {
        isFlickering = false;

        if (flickerCoroutines != null)
        {
            for (int i = 0; i < flickerCoroutines.Length; i++)
            {
                if (flickerCoroutines[i] != null)
                    StopCoroutine(flickerCoroutines[i]);
            }
        }

        // Apagar todas al terminar
        foreach (Light l in lightsToFlicker)
        {
            if (l != null) l.enabled = false;
        }
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