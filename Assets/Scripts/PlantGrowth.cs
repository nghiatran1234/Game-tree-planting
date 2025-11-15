using UnityEngine;

public class PlantGrowth : MonoBehaviour
{
    public float growthTime = 10.0f; // Thời gian (giây) để cây lớn
    
    // Các mô hình 3D cho từng trạng thái
    public GameObject seedlingModel; // Mô hình cây non (ví dụ: cục đất, mầm cây)
    public GameObject grownModel;    // Mô hình cây trưởng thành (để thu hoạch)

    [Header("Harvest Settings")]
    public string harvestItemName = "Wood"; 
    
    // MỚI: Gán số lượng sẽ nhận được
    public int harvestAmount = 1;

    private float currentGrowthTimer;
    private bool isHarvestable = false;

    void Start()
    {
        // Bắt đầu là một cây non
        currentGrowthTimer = growthTime;
        isHarvestable = false;
        
        // Hiển thị cây non, ẩn cây trưởng thành
        if (seedlingModel != null) seedlingModel.SetActive(true);
        if (grownModel != null) grownModel.SetActive(false);

        // QUAN TRỌNG: Đảm bảo prefab này có tag "Plant"
        // để GameManager/FloodManager có thể phá hủy nó
        this.gameObject.tag = "Plant"; 
    }

    void Update()
    {
        // Nếu đã sẵn sàng thu hoạch thì không cần làm gì nữa
        if (isHarvestable) return; 

        // Đếm ngược thời gian
        currentGrowthTimer -= Time.deltaTime;

        if (currentGrowthTimer <= 0)
        {
            // Hết giờ, cây đã lớn!
            BecomeHarvestable();
        }
    }

    void BecomeHarvestable()
    {
        isHarvestable = true;
        
        // Ẩn cây non, hiển thị cây trưởng thành
        if (seedlingModel != null) seedlingModel.SetActive(false);
        if (grownModel != null) grownModel.SetActive(true);

        // (Tùy chọn: Thêm hiệu ứng âm thanh/hạt)
        Debug.Log("Một cây đã sẵn sàng để thu hoạch!");
    }

    // Một hàm công khai để script khác kiểm tra
    public bool IsHarvestable()
    {
        return isHarvestable;
    }
}