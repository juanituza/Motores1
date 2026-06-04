using UnityEngine;
using System.Collections;

public class MicrowaveEvent : MonoBehaviour, IInteractable
{
    [Header("Referencias del Microondas")]
    [Tooltip("El punto de luz dentro del microondas")]
    [SerializeField] private Light interiorLight;
    [Tooltip("El modelo 3D del plato/comida para rotar")]
    [SerializeField] private Transform foodModel;
    [Tooltip("El sonido de funcionamiento del microondas")]
    [SerializeField] private AudioSource microwaveAudio;
    [SerializeField] private float rotationSpeed = 60f;

    [Header("Configuración del Evento")]
    [SerializeField] private float cookTime = 15f;
    [Tooltip("Arrastrá el objeto que tiene tu script MainPowerSwitch")]
    [SerializeField] private MainPowerSwitch powerSwitch;
    [Tooltip("La misión que aparece al cortarse la luz")]
    [SerializeField] private string blackoutMission = "Busca la caja de fusibles en el sótano";

    private bool isRunning = false;
    private bool alreadyUsed = false;

    private void Start()
    {
        if (interiorLight != null) interiorLight.enabled = false;
    }

    public void Interact()
    {
        // Solo se puede usar una vez y no se puede spamear
        if (alreadyUsed || isRunning) return;

        StartCoroutine(MicrowaveRoutine());
    }

    private IEnumerator MicrowaveRoutine()
    {
        isRunning = true;

        // 1. ENCENDEMOS EL MICROONDAS
        if (interiorLight != null) interiorLight.enabled = true;
        if (microwaveAudio != null) microwaveAudio.Play();

        // 2. BUCLE DEL TEMPORIZADOR
        float timer = cookTime;
        while (timer > 0)
        {
            // Rotamos el plato de comida en su eje Y
            if (foodModel != null)
            {
                foodModel.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
            }

            // Actualizamos el HUD mandando los segundos redondeados hacia arriba
            if (HUDManager.Instance != null)
            {
                HUDManager.Instance.UpdateTimerDisplay(Mathf.CeilToInt(timer));
            }

            timer -= Time.deltaTime;
            yield return null; // Esperamos al siguiente frame
        }

        // 3. FIN DEL TIEMPO - SE CORTA LA LUZ
        isRunning = false;
        alreadyUsed = true;

        // Apagamos el microondas abruptamente
        if (interiorLight != null) interiorLight.enabled = false;
        if (microwaveAudio != null) microwaveAudio.Stop();

        // Ocultamos el temporizador
        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.HideTimerDisplay();
        }

        // 4. ¡EL APAGÓN GENERAL!
        if (powerSwitch != null)
        {
            // Ejecutamos tu propio método Interact para bajar la palanca y cortar la luz
            powerSwitch.Interact();
        }

        // 5. NUEVA MISIÓN A OSCURAS
        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.UpdateMissionAnimated(blackoutMission);
        }
    }
}