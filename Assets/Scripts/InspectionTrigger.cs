using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class InspectionTrigger : MonoBehaviour
{
    [TextArea]
    [SerializeField] private string textToDisplay = "Parece que alguien estuvo aquí...";
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private bool onlyShowOnce = true;

    private void OnTriggerEnter(Collider other)
    {
        // Si el que entra es el jugador
        if (other.CompareTag("Player"))
        {
            HUDManager.Instance.ShowEphemeralText(textToDisplay, displayDuration);

            // Si es un pensamiento efímero, solemos apagarlo para que no se repita
            if (onlyShowOnce)
            {
                // Desactivamos el trigger para que no se vuelva a activar
                GetComponent<Collider>().enabled = false;
                this.enabled = false; // También desactivamos este script para evitar futuras llamadas
            }
        }
    }
}