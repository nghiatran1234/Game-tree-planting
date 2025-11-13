using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroController : MonoBehaviour
{
    public float delay = 3.0f; // Thời gian hiển thị (giây)
    public string nextSceneName = "MainMenu"; // Tên Scene tiếp theo

    [SerializeField] private VideoPlayer videoPlayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        videoPlayer.loopPointReached += VideoPlayer_loopPointReached;
        Invoke("LoadNextScene", delay);
    }
    private void VideoPlayer_loopPointReached(VideoPlayer source)
    {

        SceneManager.LoadScene(nextSceneName);
    }

}