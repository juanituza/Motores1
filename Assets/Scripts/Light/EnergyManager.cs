using UnityEngine;

public class EnergyManager : MonoBehaviour
{
    public static bool powerEnabled = false;

    public static void TogglePower()
    {
        powerEnabled = !powerEnabled;
    }
}