using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Efectos Visuales")]
    public GameObject redOverlay;

    [Header("Efectos Sonoros")]
    public AudioClip painSound;
    private AudioSource audioSource;

    [Header("Físicas de Impacto")]
    public float knockbackForce = 5f;
    private Rigidbody rb;
    private CharacterController cc;

    [Header("Sistema de Vidas")]
    public int maxHits = 3;
    private int currentHits = 0;

    // Evita recibir múltiples golpes en un solo segundo
    private bool isInvulnerable = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CharacterController>();

        audioSource = GetComponent<AudioSource>();

       
        if (redOverlay == null)
        {
            
            GameObject overlayEnEscena = GameObject.Find("RedOverlay");

            if (overlayEnEscena != null)
            {
                redOverlay = overlayEnEscena; // Lo conecta
            }
            else
            {
                Debug.LogWarning("Falta la pantalla roja: Asegurate de que el objeto en el Canvas se llame exactamente 'RedOverlay' y esté PRENDIDO antes de darle a Play.");
            }
        }
        if (redOverlay != null)
        {
            redOverlay.SetActive(false);
        }
    }

    // --- NUEVO: DETECCIÓN FÍSICA ---
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Enemy"))
        {
            TakeHit(hit.transform.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            TakeHit(other.transform.position);
        }
    }
    // -------------------------------

    public void TakeHit(Vector3 attackerPosition)
    {
        if (isInvulnerable) return; // Filtro de invulnerabilidad

        currentHits++;
        Debug.Log("Hit detectado. Vidas restantes: " + (maxHits - currentHits));

        if (audioSource != null && painSound != null)
        {
            audioSource.PlayOneShot(painSound);
        }

        if (redOverlay != null)
        {
            StartCoroutine(FlashRedScreen());
        }

        Vector3 pushDirection = (transform.position - attackerPosition).normalized;
        pushDirection.y = 0.1f;

        StartCoroutine(ApplyKnockbackForce(pushDirection));

        if (currentHits >= maxHits)
        {
            TriggerGameOver();
        }
    }

    void TriggerGameOver()
    {
        Debug.Log("Game Over");
        SceneManager.LoadScene("GameOver");
    }

    IEnumerator FlashRedScreen()
    {
        isInvulnerable = true;

        redOverlay.SetActive(true);
        yield return new WaitForSeconds(1f);
        redOverlay.SetActive(false);

        isInvulnerable = false;
    }

    IEnumerator ApplyKnockbackForce(Vector3 direction)
    {
        float timer = 0f;
        float duration = 0.2f;

        if (cc != null)
        {
            while (timer < duration)
            {
                cc.Move(direction * knockbackForce * Time.deltaTime);
                timer += Time.deltaTime;
                yield return null;
            }
        }
        else if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(direction * knockbackForce, ForceMode.Impulse);
        }
    }
}