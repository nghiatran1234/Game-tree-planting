using UnityEngine;
using UnityEngine.UI;

public class CaloriesBar : MonoBehaviour
{
    private Slider slider;
    public Text caloriesCount;

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

        float currentCalories = playerState.currentCalories;
        float maxCalories = playerState.maxCalories;

        float fillvalue = 0;
        if (maxCalories > 0)
        {
            fillvalue = currentCalories / maxCalories;
        }

        slider.value = fillvalue;
        
        if (caloriesCount != null)
        {
            caloriesCount.text = currentCalories + " / " + maxCalories;
        }
    }
}