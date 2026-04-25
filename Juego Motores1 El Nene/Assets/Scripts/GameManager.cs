using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Configuración de Daño")]
    [SerializeField] private int gameplaySceneIndex = 2;
    [SerializeField] private string _gameOverSceneName = "GameOver";
    [SerializeField] private int _hitsReceived = 0;

    [Header("Configuración de Victoria")]
    [SerializeField] private int _totalLightsToWin = 2; // Cantidad de luces para ganar
    [SerializeField] private string _victorySceneName = "Victory"; // Nombre de la escena de victoria
    private int _lightsOnCount = 0; // Contador de luces prendidas

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // --- LÓGICA DE VICTORIA ---
    // Este es el método que deben llamar tus interruptores
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

    // --- LÓGICA DE DAÑO (Sin modificar tu efecto) ---
    public void OnPlayerHit()
    {
        _hitsReceived++;
        Debug.Log("Golpes recibidos: " + _hitsReceived);

        // Mantenemos tu lógica de que al segundo (o cuarto según tu script anterior) se termina
        if (_hitsReceived >= 2)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        Debug.Log("Cargando Game Over...");
        // Usamos el índice que tenías configurado
        SceneManager.LoadScene(gameplaySceneIndex);
    }
}