using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VictoryVideoController : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    [Header("Scene Routing")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += ReturnToMenu;
        }
    }

    void ReturnToMenu(VideoPlayer vp)
    {
        videoPlayer.loopPointReached -= ReturnToMenu;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}