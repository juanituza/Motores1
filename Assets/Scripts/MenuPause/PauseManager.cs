using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseManager : MonoBehaviour
{
    [Header("Interfaz de Usuario")]
    [SerializeField] private GameObject pauseMenuCanvas;

    [Header("Navegación")]
    [SerializeField] private string mainMenuSceneName = "Menu_Title";

    [Header("Audio")]
    [SerializeField] private AudioSource pauseAudio;

    [Header("Control a Congelar")] // se congela la camara asi no se mueve en el menu
    [SerializeField] private MonoBehaviour cameraMovementScript;

    private bool isPaused = false;

    void Start()
    {
        pauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused)
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenuCanvas.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (pauseAudio != null)
        {
            pauseAudio.Play();
        }

        if (cameraMovementScript != null)
        {
            cameraMovementScript.enabled = false;
        }

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ResumeGame()
    {
        pauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (pauseAudio != null)
        {
            pauseAudio.Stop();
        }
        // se reactiva camara
        if (cameraMovementScript != null)
        {
            cameraMovementScript.enabled = true;
        }
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}