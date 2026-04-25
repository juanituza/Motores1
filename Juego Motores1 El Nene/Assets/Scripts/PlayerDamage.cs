using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerDamage : MonoBehaviour
{
    [Header("UI de Daño")]
    [SerializeField] private GameObject damageCanvas;
    [SerializeField] private float flashDuration;


    private void OnCollisionEnter(Collision collision)
    {
        // Esto imprimirá un mensaje en la consola de Unity cada vez que toques ALGO
        Debug.Log("He tocado algo llamado: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("¡Es un enemigo! Iniciando flash rojo...");
            StopAllCoroutines();
            StartCoroutine(ShowDamageFlash());
        }
    }

    IEnumerator ShowDamageFlash()
    {
        damageCanvas.SetActive(true);

        yield return new WaitForSeconds(flashDuration);

        damageCanvas.SetActive(false);
    }
}
