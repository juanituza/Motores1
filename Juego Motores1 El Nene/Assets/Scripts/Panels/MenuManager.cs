using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    [SerializeField] private int gameplaySceneIndex = 1;
    [SerializeField] private GameObject howToPlayPanel; // el panel de instrucciones va aca

    [Header("Configuración de Sonido")]
    [SerializeField] private AudioSource stepsAudioSource; // el AudioSource de los pasos va aca
    [SerializeField] private AudioClip footstepsClip;
    [SerializeField] private float interval = 5f;

    [Header("Configuración de Sonido - Llanto")]
    [SerializeField] private AudioSource cryingAudioSource; 
    [SerializeField] private AudioClip childCryingClip;
    [SerializeField] private float minWait = 5f;
    [SerializeField] private float maxWait = 10f;

    void Start()
    {
        if (stepsAudioSource != null && footstepsClip != null)
        {
            StartCoroutine(PlayFootstepsRoutine());
        }

        // Corrutina del llanto (ritmo aleatorio)
        if (cryingAudioSource != null && childCryingClip != null)
        {
            StartCoroutine(PlayChildCryingRoutine());
        }
    }

    public void StartGame()
    {
        // Carga la escena por su número en el Build Settings
        SceneManager.LoadScene(gameplaySceneIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo...");
        Application.Quit();
    }

    public void HowToPlay()
            {
        
        Debug.Log("Mostrando instrucciones...");
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(true);
        }
    }

    public void CloseHowToPlay()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }
    }

    IEnumerator PlayFootstepsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval); // Espera la cantidad inter segundos
            stepsAudioSource.PlayOneShot(footstepsClip);

           
        }

    }
    IEnumerator PlayChildCryingRoutine()
    {
        while (true)
        {
            // Genera un tiempo aleatorio entre el rango definido
            float randomTime = Random.Range(minWait, maxWait);
            yield return new WaitForSeconds(randomTime);

            cryingAudioSource.PlayOneShot(childCryingClip);
        }
    }
}