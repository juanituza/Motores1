using Unity.Cinemachine; 
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameManager : MonoBehaviour
{
    // Usamos un Singleton básico para que sea el administrador único
    public static NewGameManager Instance { get; private set; }

    [Header("Configuración de Spawn")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("Referencias de Escena")]
    [SerializeField] private CinemachineCamera virtualCamera;

    [Header("Transición Final")]
    [Tooltip("El nombre exacto de tu escena de cierre")]
    [SerializeField] private string endingSceneName = "Final";


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private bool isEndingTriggered = false;
    private void Start()
    {
        SpawnPlayer();
    }
    // Esta es la función que dejaron separada. 
    // Cuando decidan el evento (ej: abrir la puerta principal), ese objeto llamará a GameManager.Instance.TriggerEnding()
    public void TriggerEnding()
    {
        // Evitamos que el evento se dispare dos veces por error
        if (isEndingTriggered) return;

        isEndingTriggered = true;

        Debug.Log("¡Evento final disparado! Cargando escena de cierre...");

        // Carga la escena final
        SceneManager.LoadScene(endingSceneName);
    }
    private void SpawnPlayer()
    {
        // 1. Instanciamos al nene en la posición y rotación del SpawnPoint
        GameObject playerInstance = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);

        // 2. Reconectamos la cámara Cinemachine
        if (virtualCamera != null)
        {
            // Buscamos el objeto "CameraTarget" que está adentro del Player instanciado
            Transform camTarget = playerInstance.transform.Find("CameraTarget");

            if (camTarget != null)
            {
                virtualCamera.Follow = camTarget;
                // virtualCamera.LookAt = camTarget; // Descomentá esta línea si también usabas el LookAt
            }
            else
            {
                Debug.LogWarning("GameManager: No se encontró el objeto 'CameraTarget' dentro del Player.");
            }
        }
    }
}