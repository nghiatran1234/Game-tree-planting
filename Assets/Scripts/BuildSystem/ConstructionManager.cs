using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionManager : MonoBehaviour
{
    public static ConstructionManager Instance { get; set; }

    public GameObject itemToBeConstructed;
    public bool inConstructionMode = false;
    public GameObject constructionHoldingSpot;

    public bool isValidPlacement;

    public bool selectingAGhost;
    public GameObject selectedGhost;

    public Material ghostSelectedMat;
    public Material ghostSemiTransparentMat;
    public Material ghostFullTransparentMat;

    [Header("UI")]
    public GameObject constructionModeUI; // Kéo Panel "ConstructionMode_UI" vào đây
    public Text constructionItemNameText; // Kéo Text "ItemName_Text" vào đây
    
    private string currentItemName; // Dùng để biết trừ item nào

    public List<GameObject> allGhostsInExistence = new List<GameObject>();

    [System.Serializable]
    public class ConstructionItem
    {
        public string name;
        public GameObject itemPrefab;
    }
    public List<ConstructionItem> constructionDatabase;

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

    public void ActivateConstructionPlacement(string itemToConstruct)
    {
        ConstructionItem itemDef = constructionDatabase.Find(item => item.name == itemToConstruct);
        if (itemDef == null || itemDef.itemPrefab == null)
        {
            Debug.LogError("Không tìm thấy construction item: " + itemToConstruct);
            return;
        }


        GameObject item = Instantiate(itemDef.itemPrefab);
        item.name = itemToConstruct;

        item.transform.SetParent(constructionHoldingSpot.transform, false);
        itemToBeConstructed = item;
        itemToBeConstructed.gameObject.tag = "activeConstructable";

        constructionModeUI.SetActive(true); // Hiện UI
        constructionItemNameText.text = itemDef.name; 
        currentItemName = itemDef.name.Replace("_Model", ""); 

        Constructable constructableScript = itemToBeConstructed.GetComponent<Constructable>();
        if (constructableScript != null && constructableScript.solidCollider != null)
        {
            constructableScript.solidCollider.enabled = false;
        }


        inConstructionMode = true;
    }

    private void GetAllGhosts(GameObject itemToBeConstructed)
    {
        if (itemToBeConstructed == null) return;
        Constructable constructableScript = itemToBeConstructed.GetComponent<Constructable>();
        if (constructableScript == null) return;

        List<GameObject> ghostlist = constructableScript.ghostList;
        if (ghostlist == null) return;

        foreach (GameObject ghost in ghostlist)
        {
            if (ghost != null)
            {
                allGhostsInExistence.Add(ghost);
            }
        }
    }

    private void PerformGhostDeletionScan()
    {
        List<GameObject> ghostsToDestroy = new List<GameObject>();

        foreach (GameObject ghost in allGhostsInExistence)
        {
            if (ghost == null) continue;

            GhostItem ghostItem = ghost.GetComponent<GhostItem>();
            if (ghostItem == null || ghostItem.hasSamePosition) continue;

            foreach (GameObject ghostX in allGhostsInExistence)
            {
                if (ghostX == null || ghost.gameObject == ghostX.gameObject) continue;
                
                if (XPositionToAccurateFloat(ghost) == XPositionToAccurateFloat(ghostX) && ZPositionToAccurateFloat(ghost) == ZPositionToAccurateFloat(ghostX))
                {
                    GhostItem ghostXItem = ghostX.GetComponent<GhostItem>();
                    if (ghostXItem != null)
                    {
                        ghostXItem.hasSamePosition = true; 
                        ghostsToDestroy.Add(ghostX); 
                        break; 
                    }
                }
            }
        }

        foreach (GameObject ghost in ghostsToDestroy)
        {
            if (ghost != null)
            {
                allGhostsInExistence.Remove(ghost);
                Destroy(ghost);
            }
        }
    }

    private float XPositionToAccurateFloat(GameObject ghost)
    {
        if (ghost == null) return 0;
        Vector3 targetPosition = ghost.gameObject.transform.position;
        return Mathf.Round(targetPosition.x * 100f) / 100f;
    }

    private float ZPositionToAccurateFloat(GameObject ghost)
    {
        if (ghost == null) return 0;
        Vector3 targetPosition = ghost.gameObject.transform.position;
        return Mathf.Round(targetPosition.z * 100f) / 100f;
    }

    private void Update()
    {
        if (itemToBeConstructed != null && inConstructionMode)
        {
            if (CheckValidConstructionPosition())
            {
                isValidPlacement = true;
                itemToBeConstructed.GetComponent<Constructable>().SetValidColor();
            }
            else
            {
                isValidPlacement = false;
                itemToBeConstructed.GetComponent<Constructable>().SetInvalidColor();
            }

            if (Camera.main == null) return;

            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var selectionTransform = hit.transform;
                if (selectionTransform.gameObject.CompareTag("ghost"))
                {
                    itemToBeConstructed.SetActive(false);
                    selectingAGhost = true;
                    selectedGhost = selectionTransform.gameObject;
                }
                else
                {
                    // Di chuyển vật phẩm preview đến vị trí con trỏ chuột
                    itemToBeConstructed.transform.position = hit.point;

                    itemToBeConstructed.SetActive(true);
                    selectingAGhost = false;
                }
            }

            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            
            // Tốc độ xoay (bạn có thể điều chỉnh số 100f này)
            float rotationSpeed = 100.0f; 

            // Tính toán góc xoay dựa trên tốc độ lăn chuột
            float rotationAmount = scrollInput * rotationSpeed * Time.deltaTime;

            // Áp dụng xoay (luôn xoay quanh trục Y)
            itemToBeConstructed.transform.Rotate(Vector3.right, rotationAmount);
        }
        
        if (Input.GetMouseButtonDown(1) && inConstructionMode)
        {
            CancelPlacement(); // Gọi hàm hủy
            return; // Dừng hàm Update tại đây
        }

        if (Input.GetMouseButtonDown(0) && inConstructionMode)
        {
            if (isValidPlacement && !selectingAGhost)
            {
                PlaceItemFreeStyle();
            }

            if (selectingAGhost && selectedGhost != null)
            {
                PlaceItemInGhostPosition(selectedGhost);
            }
        }
    }

    private void PlaceItemInGhostPosition(GameObject copyOfGhost)
    {
        Vector3 ghostPosition = copyOfGhost.transform.position;
        Quaternion ghostRotation = copyOfGhost.transform.rotation;

        selectedGhost.gameObject.SetActive(false);

        itemToBeConstructed.gameObject.SetActive(true);
        itemToBeConstructed.transform.SetParent(null, true); 

        itemToBeConstructed.transform.position = ghostPosition;
        itemToBeConstructed.transform.rotation = ghostRotation;

        Constructable constructableScript = itemToBeConstructed.GetComponent<Constructable>();
        if (constructableScript != null)
        {
            constructableScript.ExtractGhostMembers();
            constructableScript.SetDefaultColor();
            if (constructableScript.solidCollider != null)
            {
                constructableScript.solidCollider.enabled = true;
            }
        }
        
        itemToBeConstructed.tag = "placedFoundation";

        GetAllGhosts(itemToBeConstructed);
        PerformGhostDeletionScan();
        
        SaveTrigger trigger = itemToBeConstructed.AddComponent<SaveTrigger>();
        trigger.itemName = currentItemName.Replace("_Model", "");

        itemToBeConstructed = null;
        inConstructionMode = false;

        constructionModeUI.SetActive(false); // Ẩn UI
        if (InventorySystem.Instance != null)
        {
            // Trừ item khỏi túi đồ
            InventorySystem.Instance.RemoveItem(currentItemName, 1);
            InventorySystem.Instance.ReCalculateList();
        }
        if (CraftingSystem.Instance != null)
        {
            CraftingSystem.Instance.RefreshNeededItems();
        }

        itemToBeConstructed = null;
        inConstructionMode = false;
    }

    private void PlaceItemFreeStyle()
    {
        itemToBeConstructed.transform.SetParent(null, true);

        Constructable constructableScript = itemToBeConstructed.GetComponent<Constructable>();
        if (constructableScript != null)
        {
            constructableScript.ExtractGhostMembers();
            constructableScript.SetDefaultColor();
            constructableScript.enabled = false;
            if (constructableScript.solidCollider != null)
            {
                constructableScript.solidCollider.enabled = true;
            }
        }

        itemToBeConstructed.tag = "placedFoundation";

        GetAllGhosts(itemToBeConstructed);
        PerformGhostDeletionScan();

        SaveTrigger trigger = itemToBeConstructed.AddComponent<SaveTrigger>();
        trigger.itemName = currentItemName.Replace("_Model", "");

        itemToBeConstructed = null;
        inConstructionMode = false;

        constructionModeUI.SetActive(false); // Ẩn UI
        if (InventorySystem.Instance != null)
        {
            // Trừ item khỏi túi đồ
            InventorySystem.Instance.RemoveItem(currentItemName, 1);
            InventorySystem.Instance.ReCalculateList();
        }
        if (CraftingSystem.Instance != null)
        {
            CraftingSystem.Instance.RefreshNeededItems();
        }

        itemToBeConstructed = null;
        inConstructionMode = false;
    }

    private bool CheckValidConstructionPosition()
    {
        if (itemToBeConstructed != null)
        {
            Constructable constructableScript = itemToBeConstructed.GetComponent<Constructable>();
            if (constructableScript != null)
            {
                return constructableScript.isValidToBeBuilt;
            }
        }
        return false;
    }

    public void CancelPlacement()
    {
        if (itemToBeConstructed != null)
        {
            Destroy(itemToBeConstructed);
        }

        inConstructionMode = false;
        itemToBeConstructed = null;
        constructionModeUI.SetActive(false); // Ẩn UI

        // (Tùy chọn) Bật lại SelectionManager nếu bạn có
        if (SelectionManager.instance != null)
        {
            SelectionManager.instance.EnableSelection();
            SelectionManager.instance.GetComponent<SelectionManager>().enabled = true;
        }
    }
}