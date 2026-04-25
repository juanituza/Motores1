using UnityEngine;
using System.Collections;

public class PlayerDamage : MonoBehaviour
{
    [SerializeField] private GameObject damageCanvas;
    [SerializeField] private float flashDuration = 1.0f;

    private void OnTriggerEnter(Collider other)
    {
        // Esto tiene que salir ahora que el enemigo tiene Rigidbody
        Debug.Log("Trigger detectado con: " + other.name);

        if (other.CompareTag("Enemy"))
        {
            StartCoroutine(ShowDamageFlash());
        }
    }

    IEnumerator ShowDamageFlash()
    {
        if (damageCanvas != null)
        {
            damageCanvas.SetActive(true);
            yield return new WaitForSeconds(flashDuration);
            damageCanvas.SetActive(false);
        }
    }
}