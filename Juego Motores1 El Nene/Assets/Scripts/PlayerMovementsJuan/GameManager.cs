using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private int _totalLightsToWin = 2;
    [SerializeField] private string _victorySceneName = "Victory";
   

    [SerializeField] private GameObject _damageOverlay;
    [SerializeField] private string _gameOverSceneName = "GameOver";


    private int _lightsOnCount = 0;
    private int _hitsReceived = 0;

    public void RegisterLightOn()
    {
        _lightsOnCount++;
        Debug.Log("Lights on: " + _lightsOnCount + " / " + _totalLightsToWin);

        if (_lightsOnCount >= _totalLightsToWin)
        {
            WinGame();
        }
    }

    private void WinGame()
    {
        Debug.Log("Victory! All lights are on.");
        SceneManager.LoadScene(_victorySceneName);
    }
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
        Debug.Log("Hit received. Total: " + _hitsReceived);

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
        Debug.Log("GameOver: Loading scene " + _gameOverSceneName);
        SceneManager.LoadScene(_gameOverSceneName);
    }
}