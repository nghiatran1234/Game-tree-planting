using UnityEngine;
using UnityEngine.UI;

public class ResourceHealthBar : MonoBehaviour
{
    private Slider slider;
    private GlobalState globalState;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        globalState = FindObjectOfType<GlobalState>();
    }

    private void Update()
    {
        if (globalState == null || slider == null)
        {
            // Tắt thanh máu nếu không có global state
            if(slider != null && slider.gameObject.activeInHierarchy)
            {
                slider.gameObject.SetActive(false);
            }
            return;
        }

        float currentHealth = globalState.resourceHealth;
        float maxHealth = globalState.resourceMaxHealth;

        // Chỉ hiển thị thanh máu nếu có tài nguyên
        bool shouldBeActive = maxHealth > 0;
        if (slider.gameObject.activeInHierarchy != shouldBeActive)
        {
            slider.gameObject.SetActive(shouldBeActive);
        }

        if (shouldBeActive)
        {
            float fillvalue = currentHealth / maxHealth;
            slider.value = fillvalue;
        }
    }
}