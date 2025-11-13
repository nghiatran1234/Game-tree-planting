using UnityEngine;
using UnityEngine.SceneManagement; // Cần để quay về Main Menu

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; set; }

    public GameObject pauseMenuUI; 
    public bool isPaused;
    public string mainMenuSceneName = "MainMenu";

    [Header("UI khác cần ẩn")]
    public GameObject playerHUDPanel;
    public GameObject quickSlotsPanel;
    public GameObject questTrackerPanel;
    public GameObject chopHolder;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; 
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playerHUDPanel != null) playerHUDPanel.SetActive(false);
        if (quickSlotsPanel != null) quickSlotsPanel.SetActive(false);
        if (questTrackerPanel != null) questTrackerPanel.SetActive(false);
        if (chopHolder != null) chopHolder.SetActive(false);

        if (SelectionManager.instance != null)
        {
            SelectionManager.instance.DisableSelection();
            SelectionManager.instance.GetComponent<SelectionManager>().enabled = false;
        }

        if (InventorySystem.Instance != null && InventorySystem.Instance.isOpen)
        {
            InventorySystem.Instance.inventoryScreenUI.SetActive(false);
            InventorySystem.Instance.isOpen = false;
        }
        if (CraftingSystem.Instance != null && CraftingSystem.Instance.isOpen)
        {
            CraftingSystem.Instance.craftingUI.SetActive(false);
            CraftingSystem.Instance.isOpen = false;
        }
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; 
        isPaused = false;
        
        if (playerHUDPanel != null) playerHUDPanel.SetActive(true);
        if (quickSlotsPanel != null) quickSlotsPanel.SetActive(true);
        

        if ((InventorySystem.Instance != null && !InventorySystem.Instance.isOpen) &&
            (CraftingSystem.Instance != null && !CraftingSystem.Instance.isOpen))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            if (SelectionManager.instance != null)
            {
                SelectionManager.instance.EnableSelection();
                SelectionManager.instance.GetComponent<SelectionManager>().enabled = true;
            }
        }
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT GAME!");
        Application.Quit();
    }
}