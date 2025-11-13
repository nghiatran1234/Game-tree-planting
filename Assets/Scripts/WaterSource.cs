using UnityEngine;

public class WaterSource : MonoBehaviour
{
    public bool playerInRange;
    public float hydrationAmount = 30f; // Lượng nước phục hồi

    void Update()
    {
        // Kiểm tra xem người chơi có đang nhìn, ở gần và nhấn E không
        if (SelectionManager.instance != null && PlayerState.Instance != null &&
            Input.GetKeyDown(KeyCode.E) && playerInRange &&
            SelectionManager.instance.onTarget &&
            SelectionManager.instance.seclectedObject == gameObject)
        {
            DrinkWater();
        }
    }

    private void DrinkWater()
    {
        // Phục hồi nước bằng script PlayerState
        PlayerState.Instance.setHydration(
            Mathf.Min(PlayerState.Instance.currentHydrationPercent + hydrationAmount,
                      PlayerState.Instance.maxHydrationPercent)
        );

        // Tùy chọn: Chơi âm thanh uống nước
        // if (SoundManager.Instance != null && SoundManager.Instance.drinkSound != null)
        // {
        //     SoundManager.Instance.PlaySound(SoundManager.Instance.drinkSound);
        // }

        Debug.Log("Người chơi đã uống nước. Nước đã được phục hồi.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}