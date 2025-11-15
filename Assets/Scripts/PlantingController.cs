using UnityEngine;

public class PlantingController : MonoBehaviour
{
    // Đổi tên thành seedlingPrefab để rõ nghĩa hơn
    public GameObject seedlingPrefab;   // Kéo Prefab "PlantPrefab" MỚI của bạn vào đây
    public LayerMask groundLayer;       // Vẫn là layer "Ground"
    
    [Header("Planting Requirement")]
    public string requiredSeedName = "Seed"; // Đây là tên item hạt giống cần có

    private Camera mainCamera;
    private InventorySystem inventorySystem; // Tham chiếu đến hệ thống kho đồ

    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        inventorySystem = InventorySystem.Instance;
    }

    void Update()
    {
        // Khi bấm "E", chúng ta sẽ "Tương tác"
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }

    void TryInteract()
    {
        // Kiểm tra xem đã gán prefab cây non chưa
        if (seedlingPrefab == null)
        {
            Debug.LogError("LỖI: Seedling Prefab chưa được gán trong PlantingController!");
            return; 
        }

        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        
        // Bắn tia Raycast, không giới hạn layer
        if (Physics.Raycast(ray, out RaycastHit hit, 100f)) 
        {
            // TRƯỜNG HỢP 1: Bắn trúng ĐẤT (Layer "Ground")
            // (Chúng ta dùng Layer thay vì Tag cho đất)
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                PlantSeed(hit.point);
            }
            // TRƯỜNG HỢP 2: Bắn trúng CÂY (Tag "Plant")
            else if (hit.collider.CompareTag("Plant"))
            {
                // Thử lấy script PlantGrowth từ vật thể bắn trúng
                PlantGrowth plant = hit.collider.GetComponent<PlantGrowth>();
                
                // Nếu có script VÀ cây đã sẵn sàng thu hoạch
                if (plant != null && plant.IsHarvestable())
                {
                    Harvest(plant);
                }
                // (Tùy chọn: Thêm âm thanh "chưa lớn" nếu plant.IsHarvestable() == false)
            }
        }
    }

    void PlantSeed(Vector3 position)
    {
        // Tạo ra cây non tại vị trí bắn trúng
        Instantiate(seedlingPrefab, position, Quaternion.identity);
        Debug.Log("Đã trồng cây non!");
    }

    void Harvest(PlantGrowth plantToHarvest)
    {
        Debug.Log("Đã thu hoạch!");
        
        // NƠI ĐỂ THÊM ĐIỂM SỐ/TÀI NGUYÊN
        // Ví dụ: FindObjectOfType<GameManager>().AddScore(10);
        
        // Hủy đối tượng cây đã thu hoạch
        Destroy(plantToHarvest.gameObject);
    }
}