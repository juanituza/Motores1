using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public GameObject redOverlay;
    public float knockbackForce = 5f;
    private Rigidbody rb;
    private CharacterController cc;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CharacterController>();

        if (redOverlay != null) redOverlay.SetActive(false);
    }
    public void TakeHit(Vector3 attackerPosition)
    {
        if (redOverlay != null)
        {
            StartCoroutine(FlashRedScreen());
        }

        Vector3 pushDirection = (transform.position - attackerPosition).normalized;
        pushDirection.y = 0.1f;

        StartCoroutine(ApplyKnockbackForce(pushDirection));
    }

    IEnumerator FlashRedScreen()
    {
        redOverlay.SetActive(true);
        yield return new WaitForSeconds(1f);
        redOverlay.SetActive(false);
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