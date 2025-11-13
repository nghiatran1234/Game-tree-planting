using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Settings")]
    public string gameSceneName = "SampleScene";

    [Header("Menu Panels")]
    public GameObject mainPanel;
    public GameObject settingsPanel;

    [Header("Audio")]
    public AudioSource audioSource;       // Gắn AudioSource từ Inspector
    public AudioClip clickSound;          // Âm thanh click
    

    // --- ÂM THANH CHUNG ---
    public void PlayClickSound()
    {
        if (audioSource && clickSound)
            audioSource.PlayOneShot(clickSound);
    }

    // --- CÁC NÚT MENU ---
    public void StartGame()
    {
        PlayClickSound();
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenSettings()
    {
        PlayClickSound();
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        PlayClickSound();
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void LoadGame()
    {
        PlayClickSound();
        SaveLoadManager.shouldLoadGame = true;
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        PlayClickSound();
        Debug.Log("QUIT GAME!");
        Application.Quit();
    }
}
