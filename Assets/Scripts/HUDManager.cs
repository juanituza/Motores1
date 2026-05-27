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
}
