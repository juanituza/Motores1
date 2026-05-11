using UnityEngine;

public class LightSetup : MonoBehaviour
{
    public bool startOff = true;

    void Awake()
    {
        if (startOff)
        {
            var light = GetComponent<Light>();
            if (light != null) light.enabled = false;
        }
    }
}