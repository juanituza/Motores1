using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject pauseMenuCanvas;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pauseOpenSound;

    private bool isPaused = false;

    void Start()
    {
        if (pauseMenuCanvas != null) pauseMenuCanvas.SetActive(false);
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

       
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (audioSource != null && pauseOpenSound != null)
        {
            audioSource.clip = pauseOpenSound; 
            audioSource.Play();
        }

    }

    public void ResumeGame()
    {
        pauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (audioSource != null)
        {
            audioSource.Stop(); 

        }

    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu_Title");
    }

 
}