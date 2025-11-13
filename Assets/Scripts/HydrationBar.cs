using UnityEngine;
using UnityEngine.UI;

public class HydrationBar : MonoBehaviour
{
    private Slider slider;
    public Text hydrationCount;

    public PlayerState playerState; // Kéo Player (có script PlayerState) vào đây

    private float currentHydration, maxHydration;
    
    void Awake()
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

        currentHydration = playerState.currentHydrationPercent;
        maxHydration = playerState.maxHydrationPercent;

        float fillvalue = 0;
        if (maxHydration > 0)
        {
            fillvalue = currentHydration / maxHydration;
        }
        
        slider.value = fillvalue;
        
        if (hydrationCount != null)
        {
            hydrationCount.text = currentHydration + " % " ;
        }
    }
}