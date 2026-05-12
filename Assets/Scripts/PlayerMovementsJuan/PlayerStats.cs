using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public void TakeDamage()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerHit();
        }
    }
}