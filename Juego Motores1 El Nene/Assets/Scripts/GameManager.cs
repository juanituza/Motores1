using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cambiar de escena
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Configuración de Daño")]
    [SerializeField] private int gameplaySceneIndex = 2;
    [SerializeField] private string _gameOverSceneName = "GameOver"; // Nombre de tu escena de derrota

    [SerializeField] private int _hitsReceived = 0; // Contador de golpes

    private void Awake()
    {
        // Singleton: permite que el enemigo llame al Manager fácilmente
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

    }

    // Esta es la función que el enemigo debe llamar
    public void OnPlayerHit()
    {
        _hitsReceived++;
        Debug.Log("Golpes recibidos: " + _hitsReceived);

        if (_hitsReceived >= 4)
        {
            // Segundo toque: Fin del juego
            GameOver();
        }
    }

    public void GameOver()
    {
        Debug.Log("Cargando Game Over...");
        SceneManager.LoadScene(gameplaySceneIndex);
    }
}