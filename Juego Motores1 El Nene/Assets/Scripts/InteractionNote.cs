using UnityEngine;

public class InteractionNote : MonoBehaviour
{
    [Header("Configuración de UI")]
    [SerializeField] private GameObject interactionPrompt; // El objeto "Press E to Interact"
    [SerializeField] private GameObject noteCanvas;       // El Canvas con la imagen de la nota

    private bool isPlayerInRange = false;
    private bool isNoteOpen = false;

    void Start()
    {
        // Nos aseguramos de que todo empiece oculto
        if (interactionPrompt) interactionPrompt.SetActive(false);
        if (noteCanvas) noteCanvas.SetActive(false);
    }

    void Update()
    {
        // Si el jugador está en rango y presiona E
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ToggleNote();
        }
    }

    private void ToggleNote()
    {
        isNoteOpen = !isNoteOpen;
        noteCanvas.SetActive(isNoteOpen);

        // Si abrimos la nota, ocultamos el mensaje de "Press E" para que no moleste
        if (isNoteOpen)
        {
            interactionPrompt.SetActive(false);
        }
        else
        {
            interactionPrompt.SetActive(true);
        }
    }

    // Detección del Sphere Collider (isTrigger activado)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            interactionPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            isNoteOpen = false;

            // Al salir, ocultamos todo por seguridad
            interactionPrompt.SetActive(false);
            noteCanvas.SetActive(false);
        }
    }
}
