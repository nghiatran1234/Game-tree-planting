using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Slider slider;
    public Text healthCount;

    public PlayerState playerState; // Kéo Player (có script PlayerState) vào đây

    private void Awake()
    {
        slider = GetComponent<Slider>();
        
        // Nếu không kéo thả thì tự tìm
        if (playerState == null)
        {
            playerState = FindObjectOfType<PlayerState>();
        }
    }
    
    void Update()
    {
        if (playerState == null || slider == null) return;

        float currentHealth = playerState.currentHealth;
        float maxHealth = playerState.maxHealth;

        float fillvalue = 0;
        if (maxHealth > 0)
        {
            fillvalue = currentHealth / maxHealth;
        }
        
        slider.value = fillvalue;

        if (healthCount != null)
        {
            healthCount.text = currentHealth + " / " + maxHealth;
        }
    }
}