using UnityEngine;

public class LampVisual : MonoBehaviour
{
    private Light pointLight;

    public Renderer lampRenderer;

    public Material shadeOn;
    public Material shadeOff;

    public Material bulbOn;
    public Material bulbOff;

    void Start()
    {
        pointLight = GetComponentInChildren<Light>();
    }

    void Update()
    {
        Material[] mats = lampRenderer.materials;

        if (pointLight.enabled)
        {
            mats[1] = bulbOn;
            mats[2] = shadeOn;
        }
        else
        {
            mats[1] = bulbOff;
            mats[2] = shadeOff;
        }

        lampRenderer.materials = mats;
    }
}