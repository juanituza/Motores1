using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject _damageOverlay; // Asignar en el Inspector el Canvas con la imagen roja
    [SerializeField] private string _gameOverSceneName = "GameOver";

    private int _hitsReceived = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (_damageOverlay != null)
        {
            _damageOverlay.SetActive(false);
        }
    }

    public void OnPlayerHit()
    {
        _hitsReceived++;
        Debug.Log("Hit Recibido. Total: " + _hitsReceived);

        if (_hitsReceived == 1)
        {
            StartCoroutine(ShowDamageEffect());
        }
        else if (_hitsReceived >= 2)
        {
            GameOver();
        }
    }

    private IEnumerator ShowDamageEffect()
    {
        if (_damageOverlay != null)
        {
            _damageOverlay.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            _damageOverlay.SetActive(false);
        }
    }
   
    public void GameOver()
    {
        Debug.Log("GameOver: Loading " + _gameOverSceneName);
        SceneManager.LoadScene("GameOver");
    }
}