using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipSystem : MonoBehaviour
{
    public static EquipSystem Instance { get; set; }

    public GameObject quickSlotsPanel;
    public List<GameObject> quickSlotsList = new List<GameObject>();
    public List<string> itemList = new List<string>();

    public GameObject numbersHolder;
    public int selectedNumber = -1;
    public GameObject selectedItem;
    public GameObject ToolHolder;
    public GameObject selecteditemModel;

    [System.Serializable]
    public class ToolModel
    {
        public string itemName; 
        public GameObject modelPrefab; 
    }
    public List<ToolModel> toolModelDatabase;


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

    private void Start()
    {
        PopulateSlotList();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) { SelecQuickSlot(1); }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) { SelecQuickSlot(2); }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) { SelecQuickSlot(3); }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) { SelecQuickSlot(4); }
        else if (Input.GetKeyDown(KeyCode.Alpha5)) { SelecQuickSlot(5); }
        else if (Input.GetKeyDown(KeyCode.Alpha6)) { SelecQuickSlot(6); }
        else if (Input.GetKeyDown(KeyCode.Alpha7)) { SelecQuickSlot(7); }
    }

    void SelecQuickSlot(int number)
    {
        if (checkIfSlotIsFull(number))
        {
            if (selectedNumber != number)
            {
                selectedNumber = number;

                if (selectedItem != null)
                {
                    selectedItem.gameObject.GetComponent<InventoryItem>().isSelected = false;
                }
                selectedItem = getSetlectedItem(number);
                selectedItem.GetComponent<InventoryItem>().isSelected = true;

                SetEquippedModel(selectedItem);

                foreach (Transform child in numbersHolder.transform)
                {
                    child.transform.Find("Text").GetComponent<Text>().color = Color.gray;
                }

                Text toBeChanged = numbersHolder.transform.Find("number" + number).transform.Find("Text").GetComponent<Text>();
                toBeChanged.color = Color.white;
            }
            else 
            {
                selectedNumber = -1;
                if (selectedItem != null)
                {
                    selectedItem.gameObject.GetComponent<InventoryItem>().isSelected = false;
                    selectedItem = null;
                }

                if (selecteditemModel != null)
                {
                    Destroy(selecteditemModel.gameObject);
                    selecteditemModel = null;
                }

                foreach (Transform child in numbersHolder.transform)
                {
                    child.transform.Find("Text").GetComponent<Text>().color = Color.gray;
                }
            }
        }
    }

    private void SetEquippedModel(GameObject selectedItem)
    {
        InventoryItem itemScript = selectedItem.GetComponent<InventoryItem>();
        if (SoundManager.Instance != null && SoundManager.Instance.pullaxeSound != null)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.pullaxeSound);
        }
        if (itemScript == null) return;

        string selectedItemID = itemScript.itemID;

        // Nếu item là vật xây dựng => kích hoạt chế độ xây
        if (selectedItemID == "Foundation")
        {
            ConstructionManager.Instance.ActivateConstructionPlacement("Foundation");
            return;
        }

        // Nếu là tool thì mới tìm model 3D
        ToolModel tool = toolModelDatabase.Find(t => t.itemName == selectedItemID);
        if (tool != null && tool.modelPrefab != null)
        {
            selecteditemModel = Instantiate(tool.modelPrefab,
                new Vector3(0.37f, 1.27f, 0.43f),
                Quaternion.Euler(-0.809f, -125.857f, -14.016f));
            selecteditemModel.transform.SetParent(ToolHolder.transform, false);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy model 3D hoặc không cần equip cho itemID: " + selectedItemID);
        }
    }


    GameObject getSetlectedItem(int slotNumber)
    {
        return quickSlotsList[slotNumber - 1].transform.GetChild(0).gameObject;
    }

    bool checkIfSlotIsFull(int slotNumber)
    {
        return quickSlotsList[slotNumber - 1].transform.childCount > 0;
    }

    public void PopulateSlotList()
    {
        foreach (Transform child in quickSlotsPanel.transform)
        {
            if (child.CompareTag("QuickSlot"))
            {
                quickSlotsList.Add(child.gameObject);
            }
        }
    }

    public void AddToQuickSlots(GameObject itemToEquip)
    {
        GameObject availableSlot = FindNextEmptySlot();
        if (availableSlot == null)
        {
            Debug.Log("Quick slots đã đầy!");
            return;
        }

        itemToEquip.transform.SetParent(availableSlot.transform, false);

        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.ReCalculateList();
        }
    }

    private GameObject FindNextEmptySlot()
    {
        foreach (GameObject slot in quickSlotsList)
        {
            if (slot.transform.childCount == 0)
            {
                return slot;
            }
        }
        return null; 
    }

    public bool CheckIfFull()
    {
        int counter = 0;
        foreach (GameObject slot in quickSlotsList)
        {
            if (slot.transform.childCount > 0)
            {
                counter++;
            }
        }
        return counter == 7;
    }
}