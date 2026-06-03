using UnityEngine;

public class EnergyManager : MonoBehaviour
{
    public static bool powerEnabled = true;

    public static void TogglePower()
    {
        powerEnabled = !powerEnabled;
    }
}