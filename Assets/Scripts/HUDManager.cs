using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [Header("Crosshair")]
    [SerializeField] private Image crosshairImage;
    [SerializeField] private Sprite dotSprite;  // El punto gris
    [SerializeField] private Sprite handSprite; // La mano

    [Header("Stamina")]
    [SerializeField] private Image staminaBar; // Imagen de tipo "Filled"

    [Header("Textos")]
    [SerializeField] private TextMeshProUGUI ephemeralText;
    [SerializeField] private TextMeshProUGUI missionText;

    [Header("Eventos Especiales")]
    [SerializeField] private TextMeshProUGUI timerText; // Arrastrá un nuevo texto de tu Canvas acá
    // TODO: Implementar UI de batería cuando tengamos la linterna
    // [SerializeField] private Image batteryBar; 

    private void Awake()
    {
        // Configuramos el Singleton para llamarlo desde cualquier script
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Limpiamos los textos al arrancar
        ephemeralText.text = "";
        ephemeralText.canvasRenderer.SetAlpha(0f);

        timerText.text = "";
    }

    // --- MÉTODOS PARA LLAMAR DESDE OTROS SCRIPTS ---

    public void UpdateStamina(float fillAmount)
    {
        staminaBar.fillAmount = fillAmount;
    }

    public void SetInteractIcon(bool canInteract)
    {
        crosshairImage.sprite = canInteract ? handSprite : dotSprite;
        // Le damos un poco más de opacidad a la mano para que resalte
        Color c = crosshairImage.color;
        c.a = canInteract ? 0.8f : 0.3f;
        crosshairImage.color = c;
    }

    public void ShowEphemeralText(string message, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FadeTextRoutine(message, duration));
    }

    public void UpdateMission(string missionInfo)
    {
        missionText.text = missionInfo;
    }

    public void ClearMission()
    {
        missionText.text = "";
    }

    private IEnumerator FadeTextRoutine(string message, float duration)
    {
        ephemeralText.text = message;
        ephemeralText.CrossFadeAlpha(1f, 0.5f, false); // Aparece suave

        yield return new WaitForSeconds(duration);

        ephemeralText.CrossFadeAlpha(0f, 1f, false);   // Desaparece suave
    }
    // Llamamos a este método desde cualquier objeto para lanzar la animación
    public void UpdateMissionAnimated(string newMission)
    {
        // Detenemos cualquier transición previa por seguridad
        StopCoroutine(nameof(MissionTransitionRoutine));
        StartCoroutine(MissionTransitionRoutine(newMission));
    }

    private IEnumerator MissionTransitionRoutine(string newMission)
    {
        // 1. Tachamos la misión actual (Usando Rich Text de TextMeshPro)
        string currentText = missionText.text;

        // Evitamos tachar si la misión anterior estaba vacía
        if (!string.IsNullOrEmpty(currentText))
        {
            missionText.text = $"<s>{currentText}</s>";
            // 2. Esperamos 2 segundos para que el jugador vea que la completó
            yield return new WaitForSeconds(2f);
        }

        // 3. Desvanecemos el texto viejo suavemente
        missionText.CrossFadeAlpha(0f, 0.5f, false);
        yield return new WaitForSeconds(0.5f);

        // 4. Escribimos la nueva misión y la hacemos aparecer suavemente
        missionText.text = newMission;
        missionText.CrossFadeAlpha(1f, 0.5f, false);
    }

    public void UpdateTimerDisplay(int seconds)
    {
        // Formato para que se vea como reloj digital (ej: 00:15)
        timerText.text = $"00:{seconds:D2}";
    }

    public void HideTimerDisplay()
    {
        timerText.text = "";
    }
}
