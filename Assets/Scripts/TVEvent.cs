using UnityEngine;
using UnityEngine.Video; // Necesario para controlar videos

public class TVEvent : MonoBehaviour, IInteractable
{
    [Header("Componentes")]
    [Tooltip("El componente VideoPlayer de la TV")]
    [SerializeField] private VideoPlayer tvVideoPlayer;

    [Header("Configuración del Evento")]
    [Tooltip("¿Querés que el nene piense algo al prenderla?")]
    [SerializeField] private string thoughtText = "¿De dónde viene esta transmisión...?";
    [SerializeField] private bool turnOffAfterPlay = true;

    private bool alreadyUsed = false;

    private void Start()
    {
        // Nos aseguramos de que arranque apagada
        if (tvVideoPlayer != null)
        {
            tvVideoPlayer.Stop();

            // Suscribimos un evento para saber cuándo termina el video
            if (turnOffAfterPlay)
            {
                tvVideoPlayer.loopPointReached += EndVideo;
            }
        }
    }

    public void Interact()
    {
        if (alreadyUsed) return;

        if (tvVideoPlayer != null)
        {
            tvVideoPlayer.Play();

            if (HUDManager.Instance != null && !string.IsNullOrEmpty(thoughtText))
            {
                HUDManager.Instance.ShowEphemeralText(thoughtText, 3f);
            }

            alreadyUsed = true;

            // Apagamos la interacción para no spamear
            GetComponent<Collider>().enabled = false;
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }

    // Este método se dispara solo cuando el video llega a su último fotograma
    private void EndVideo(VideoPlayer vp)
    {
        vp.Stop();
        // Acá podrías disparar otro evento si quisieras, como un apagón o un sonido de estática
    }
}
