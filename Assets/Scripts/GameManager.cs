using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private int gameplaySceneIndex = 2;
    [SerializeField] private string _gameOverSceneName = "GameOver";
    [SerializeField] private int _hitsReceived = 0;

    [SerializeField] private int _totalLightsToWin = 2; 
    [SerializeField] private string _victorySceneName = "Victory";
    private int _lightsOnCount = 0; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterLightOn()
    {
        _lightsOnCount++;
        Debug.Log("Luces prendidas: " + _lightsOnCount + " / " + _totalLightsToWin);

        if (_lightsOnCount >= _totalLightsToWin)
        {
            WinGame();
        }
    }

    private void WinGame()
    {
        Debug.Log("¡Victoria! Cargando escena: " + _victorySceneName);
        SceneManager.LoadScene(_victorySceneName);
    }

    public void OnPlayerHit()
    {
        _hitsReceived++;
        Debug.Log("Golpes recibidos: " + _hitsReceived);
        if (_hitsReceived == 4)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        Debug.Log("Cargando Game Over...");
        SceneManager.LoadScene(gameplaySceneIndex);
    }
}