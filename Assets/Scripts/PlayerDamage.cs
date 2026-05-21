using UnityEngine;
using System.Collections;

public class PlayerDamage : MonoBehaviour
{
    [SerializeField] private GameObject DamageEffect;
    [SerializeField] private float flashDuration = 1.0f;

    private void OnTriggerEnter(Collider other)
    {
        // Esto tiene que salir ahora que el enemigo tiene Rigidbody
        Debug.Log("Trigger detectado con: " + other.name);

        if (other.CompareTag("Enemy"))
        {
            GameManager.Instance.OnPlayerHit();
            StartCoroutine(ShowDamageFlash());
        }
    }

    IEnumerator ShowDamageFlash()
    {
        if (DamageEffect != null)
        {
            DamageEffect.SetActive(true);
            yield return new WaitForSeconds(flashDuration);
            DamageEffect.SetActive(false);
        }
    }
}