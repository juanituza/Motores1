using UnityEngine;

public class LaundryTrigger : MonoBehaviour
{
    public WindowBreakEvent windowEvent; // Se pone la ventana
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
       
        Debug.Log("Algo tocó el cubo del lavadero: " + other.gameObject.name + " (Tag: " + other.tag + ")");

        if (!hasTriggered && other.CompareTag("Player"))
        {
            Debug.Log("El Jugador pisó la trampa. Disparando evento");
            hasTriggered = true;

            if (windowEvent != null)
            {
                windowEvent.TriggerWindowEvent();
            }
            else
            {
                Debug.LogError("¡ERROR! Te olvidaste de arrastrar la ventana al casillero Window Event del cubo.");
            }
        }
    }
}