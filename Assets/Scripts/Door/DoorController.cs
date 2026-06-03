using UnityEngine;
using System.Collections;

// Firmamos el contrato IInteractable para tu raycast
public class DoorController : MonoBehaviour, IInteractable
{
    [Header("Configuración de Rotación")]
    [Tooltip("Arrastrá acá el objeto vacío 'HingePivot'")]
    [SerializeField] private Transform hingePivot;
    [Tooltip("Grados de apertura (puede ser negativo ej: -90 si abre para el otro lado)")]
    [SerializeField] private float openAngle = 90f;
    [Tooltip("Velocidad de la animación")]
    [SerializeField] private float smoothSpeed = 4f;

    [Header("Estado de la Puerta")]
    [Tooltip("Si está marcada, no se abrirá hasta que un evento la desbloquee")]
    [SerializeField] private bool isLocked = true;

    [Header("Feedback al jugador")]
    [SerializeField] private string lockedText = "Está bloqueada... Parece que falta algo.";
    [SerializeField] private AudioClip lockedDoorSound;

    private bool isOpen = false;
    private bool isAnimating = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    private void Start()
    {
        if (hingePivot != null)
        {
            // Guardamos la rotación inicial como la "cerrada"
            closedRotation = hingePivot.localRotation;
            // Calculamos cuál será la rotación "abierta"
            openRotation = Quaternion.Euler(hingePivot.localEulerAngles.x, hingePivot.localEulerAngles.y + openAngle, hingePivot.localEulerAngles.z);
        }
    }

    public void Interact()
    {
        // Si se está moviendo, ignoramos el clic para evitar bugs visuales
        if (isAnimating) return;

        if (isLocked)
        {
            // Avisamos al HUD que está cerrada
            if (HUDManager.Instance != null)
            {
                HUDManager.Instance.ShowEphemeralText(lockedText, 3f);
            }
            if (lockedDoorSound != null) AudioSource.PlayClipAtPoint(lockedDoorSound, transform.position, 1f);
            return;
        }

        // Alternamos el estado y comenzamos la animación
        isOpen = !isOpen;
        StartCoroutine(AnimateDoor());
    }

    // Este método lo van a llamar las notas u otros eventos para destrabarla
    public void UnlockDoor()
    {
        isLocked = false;
        
    }

    private IEnumerator AnimateDoor()
    {
        isAnimating = true;
        Quaternion targetRotation = isOpen ? openRotation : closedRotation;

        // Rotamos suavemente hasta que el ángulo casi coincida
        while (Quaternion.Angle(hingePivot.localRotation, targetRotation) > 0.1f)
        {
            hingePivot.localRotation = Quaternion.Slerp(hingePivot.localRotation, targetRotation, Time.deltaTime * smoothSpeed);
            yield return null;
        }

        // Forzamos la posición final exacta para evitar desajustes milimétricos
        hingePivot.localRotation = targetRotation;
        isAnimating = false;
    }
}