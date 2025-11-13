using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance { get; set; }

    [Header("UI Elements")]
    public Text healthText;
    public Text caloriesText;
    public Text hydrationText;
    public GameObject gameOverPanel;

    [Header("Stats")]
    public float currentHealth;
    public float maxHealth = 100f;
    public float currentCalories;
    public float maxCalories = 2000f;
    public float currentHydrationPercent;
    public float maxHydrationPercent = 100f;
    
    [Header("Natural Decay Rate (Per Second)")]
    public float caloriesIdleDecayRate = 0.1f; 
    public float caloriesRunningDecayRate = 1.0f; 
    public float hydrationDecayRate = 1.0f; 

    [Header("Trạng thái Môi trường")]
    public bool isGettingWet = false; 
    
    [Range(0, 100)]
    public float currentWetness = 0f;
    public float maxWetness = 100f;
    
    public float wettingRate = 5f; 
    public float dryingRate = 1f;

    private PlayerMovement playerMovement;
    private bool isDead = false;

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

        currentHealth = maxHealth;
        currentCalories = maxCalories;
        currentHydrationPercent = maxHydrationPercent;

        playerMovement = GetComponent<PlayerMovement>();
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    private void Update()
    {
        // Nếu đã chết thì không làm gì cả
        if (isDead) return;

        // --- Logic giảm chỉ số (giữ nguyên) ---
        if (isGettingWet)
        {
            currentWetness = Mathf.Min(currentWetness + wettingRate * Time.deltaTime, maxWetness);
        }
        else
        {
            currentWetness = Mathf.Max(currentWetness - dryingRate * Time.deltaTime, 0);
        }

        if (currentWetness > 50)
        {
        }

        if (currentCalories > 0)
        {
            if (playerMovement != null && playerMovement.isMoving)
            {
                currentCalories -= caloriesRunningDecayRate * Time.deltaTime;
            }
            else
            {
                currentCalories -= caloriesIdleDecayRate * Time.deltaTime;
            }
        }
        else
        {
            currentCalories = 0;
            TakeDamage(0.1f * Time.deltaTime);
        }

        if (currentHydrationPercent > 0)
        {
            currentHydrationPercent -= hydrationDecayRate * Time.deltaTime;
        }
        else
        {
            currentHydrationPercent = 0;
            TakeDamage(0.2f * Time.deltaTime);
        }
        if (isGettingWet)
        {
            currentWetness = Mathf.Min(currentWetness + wettingRate * Time.deltaTime, maxWetness);
        }
        else
        {
            currentWetness = Mathf.Max(currentWetness - dryingRate * Time.deltaTime, 0);
        }

        if (currentWetness > 50)
        {
        }

        if (currentCalories > 0)
        {
            if (playerMovement != null && playerMovement.isMoving)
            {
                currentCalories -= caloriesRunningDecayRate * Time.deltaTime;
            }
            else
            {
                currentCalories -= caloriesIdleDecayRate * Time.deltaTime;
            }
        }
        else
        {
            currentCalories = 0;
            TakeDamage(0.1f * Time.deltaTime); 
        }

        if (currentHydrationPercent > 0)
        {
            currentHydrationPercent -= hydrationDecayRate * Time.deltaTime;
        }
        else
        {
            currentHydrationPercent = 0;
            TakeDamage(0.2f * Time.deltaTime); 
        }
        

        if (healthText != null)
        {
            healthText.text = Mathf.RoundToInt(currentHealth) + " / " + maxHealth;
        }

        if (caloriesText != null)
        {
            caloriesText.text = Mathf.RoundToInt(currentCalories) + " / " + maxCalories;
        }

        if (hydrationText != null)
        {
            hydrationText.text = Mathf.RoundToInt(currentHydrationPercent) + " %";
        }
    }
    
    public void setHealth(float newHealth)
    {
        currentHealth = newHealth;
    }
     
    public void setCalories(float newCalories)
    {
        currentCalories = newCalories;
    }
 
    public void setHydration(float newHydration)
    {
        currentHydrationPercent = newHydration;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0); 

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return; // Đảm bảo chỉ chạy 1 lần

        isDead = true;
        Debug.Log("Player has died.");

        if (SoundManager.Instance != null && SoundManager.Instance.startingZoneBGMusic != null)
        {
            SoundManager.Instance.startingZoneBGMusic.Stop();
        }
        // Hiện Panel Game Over
        if (gameOverPanel != null)
        {
             gameOverPanel.SetActive(true);
        }

        // Dừng thời gian
        Time.timeScale = 0f;

        // Mở khóa chuột
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Tắt các UI khác (nếu cần, giống PauseMenu)
        if (PauseMenu.Instance != null)
        {
            if(PauseMenu.Instance.playerHUDPanel != null) PauseMenu.Instance.playerHUDPanel.SetActive(false);
            if(PauseMenu.Instance.quickSlotsPanel != null) PauseMenu.Instance.quickSlotsPanel.SetActive(false);
        }
        // Tắt SelectionManager
        if (SelectionManager.instance != null)
        {
            SelectionManager.instance.DisableSelection();
            SelectionManager.instance.GetComponent<SelectionManager>().enabled = false;
        }
    }

    // --- Thêm các hàm cho nút trên GameOver_Panel ---
    public void RestartGame()
    {
        Time.timeScale = 1f; // Khởi động lại thời gian trước khi load scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Load lại scene hiện tại
    }

    public void LoadMainMenu()
    {
         Time.timeScale = 1f;
         // Giả sử bạn có biến lưu tên scene Main Menu
         // SceneManager.LoadScene("MainMenu");
         Debug.LogWarning("Chưa có hàm LoadMainMenu!"); // Nhắc nhở nếu chưa có
         // Hoặc dùng lại hàm của PauseMenu nếu có thể truy cập
         if(PauseMenu.Instance != null)
         {
             PauseMenu.Instance.LoadMainMenu();
         }
    }
}