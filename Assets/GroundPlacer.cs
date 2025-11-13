using UnityEngine;

// Phần Script chính, gắn vào GameObject
public class GroundPlacer : MonoBehaviour
{
    [Tooltip("Độ cao cộng thêm so với mặt đất (ví dụ: 0.1 để tránh xuyên đất)")]
    public float yOffset = 0.05f;

    [Tooltip("LayerMask của mặt đất")]
    public LayerMask groundLayerMask = 1; // Mặc định là Layer "Default"

    // THÊM HÀM START NÀY VÀO ĐÂY
    void Start()
    {
        PlaceOnGround(); // Tự động chạy khi game bắt đầu
    }

    // THÊM HÀM NÀY VÀO ĐÂY (hoặc giữ lại nếu đã có)
    public void PlaceOnGround()
    {
        Transform objectTransform = transform;
        // Bắt đầu bắn tia từ vị trí hiện tại của vật thể, cộng thêm 1 đơn vị Y
        Vector3 origin = objectTransform.position + Vector3.up * 1.0f;
        RaycastHit hit;

        // Bắn tia Raycast thẳng xuống dưới
        if (Physics.Raycast(origin, Vector3.down, out hit, Mathf.Infinity, groundLayerMask))
        {
            // Nếu tia trúng mặt đất
            Vector3 targetPosition = hit.point; // Lấy điểm va chạm
            targetPosition.y += yOffset; // Cộng thêm độ cao offset
            objectTransform.position = targetPosition; // Di chuyển vật thể
            // Debug.Log(gameObject.name + " auto placed on ground at Y = " + targetPosition.y);
        }
        else
        {
            // Nếu không tìm thấy mặt đất bên dưới
            Debug.LogWarning("Không tìm thấy mặt đất bên dưới: " + gameObject.name + " để tự động hạ xuống.");
        }
    }
}
