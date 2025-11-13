using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; set; }

    public GameObject ItemInfoUI;
    public GameObject inventoryScreenUI;
    public GameObject deletionAlertUI;
    public List<GameObject> slotList = new List<GameObject>();
    // Flattened list of inventory item IDs (one entry per unit) used by save/load
    public List<string> itemList = new List<string>();

    [System.Serializable]
    public class ItemDefinition
    {
        public string itemName;
        public GameObject itemPrefab;
        public int maxStackSize = 64;
    }
    public List<ItemDefinition> itemDatabase;

    public bool isOpen;

    public GameObject pickupAlert;
    public Text pickupName;
    public Image pickupImage;

    [Header("Trash UI")]
    public GameObject trashIcon;
    public Sprite trashOpenSprite;
    public Sprite trashClosedSprite;
    private Image trashImageComponent;

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
        if (trashIcon != null)
        {
            trashImageComponent = trashIcon.GetComponent<Image>();
        }
    }

    void Start()
    {
        isOpen = false;
        PopulateSlotList();
        Cursor.visible = false;
        if (trashIcon != null) trashIcon.SetActive(false);
        if (trashImageComponent != null) trashImageComponent.sprite = trashClosedSprite;
    }

    private void PopulateSlotList()
    {
        slotList.Clear();
        foreach (Transform child in inventoryScreenUI.transform)
        {
            if (child.CompareTag("Slot"))
            {
                slotList.Add(child.gameObject);
            }
        }
    }

    // Recalculate a flattened representation of items (one string per unit quantity)
    public void ReCalculateList()
    {
        itemList.Clear();

        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount > 0)
            {
                InventoryItem itemScript = slot.transform.GetChild(0).GetComponent<InventoryItem>();
                if (itemScript != null)
                {
                    for (int i = 0; i < itemScript.quantity; i++)
                    {
                        itemList.Add(itemScript.itemID);
                    }
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && !isOpen && !PauseMenu.Instance.isPaused && (StorageManager.Instance == null || !StorageManager.Instance.isOpen))
        {
            OpenInventory();
        }
        else if (Input.GetKeyDown(KeyCode.I) && isOpen && !PauseMenu.Instance.isPaused)
        {
            CloseInventory();
        }
    }

    public void OpenInventory()
    {
         inventoryScreenUI.SetActive(true);
         Cursor.lockState = CursorLockMode.None;
         Cursor.visible = true;
         if (SelectionManager.instance != null)
         {
             SelectionManager.instance.DisableSelection();
             if(SelectionManager.instance.GetComponent<SelectionManager>() != null)
                 SelectionManager.instance.GetComponent<SelectionManager>().enabled = false;
         }
         if (trashIcon != null) trashIcon.SetActive(true);
         isOpen = true;
         if (CraftingSystem.Instance != null) CraftingSystem.Instance.RefreshNeededItems();
    }

    public void CloseInventory()
    {
        inventoryScreenUI.SetActive(false);
        if (ShouldLockCursor())
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (SelectionManager.instance != null)
            {
                 SelectionManager.instance.EnableSelection();
                 if(SelectionManager.instance.GetComponent<SelectionManager>() != null)
                    SelectionManager.instance.GetComponent<SelectionManager>().enabled = true;
            }
        }
        if (trashIcon != null) trashIcon.SetActive(false);
        if (ItemInfoUI != null) ItemInfoUI.SetActive(false);
        isOpen = false;
    }

    private bool ShouldLockCursor()
    {
        bool craftingOpen = CraftingSystem.Instance != null && CraftingSystem.Instance.isOpen;
        bool storageOpen = StorageManager.Instance != null && StorageManager.Instance.isOpen;
        bool dialogueOpen = DialogueManager.Instance != null && DialogueManager.Instance.isDialogueActive;
        bool pauseOpen = PauseMenu.Instance != null && PauseMenu.Instance.isPaused;

        return !craftingOpen && !storageOpen && !dialogueOpen && !pauseOpen;
    }


    public void AddToInventory(string itemName, int quantityToAdd = 1)
    {
        ItemDefinition itemDef = itemDatabase.Find(item => item.itemName == itemName);
        if (itemDef == null || itemDef.itemPrefab == null)
        {
            Debug.LogError($"Không tìm thấy item hoặc prefab tên: {itemName} trong Database!");
            return;
        }

        if (SoundManager.Instance != null && SoundManager.Instance.pickUpItemSound != null)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.pickUpItemSound);
        }

        int remainingQuantity = quantityToAdd;

        foreach (GameObject slot in slotList)
        {
            if (remainingQuantity <= 0) break;

            if (slot.transform.childCount > 0)
            {
                GameObject itemObj = slot.transform.GetChild(0).gameObject;
                InventoryItem itemScript = itemObj.GetComponent<InventoryItem>();

                if (itemScript != null && itemScript.itemID == itemName && itemScript.quantity < itemDef.maxStackSize)
                {
                    int canAdd = itemDef.maxStackSize - itemScript.quantity;
                    int amountToAdd = Mathf.Min(remainingQuantity, canAdd);

                    itemScript.quantity += amountToAdd;
                    itemScript.UpdateQuantityText();
                    remainingQuantity -= amountToAdd;
                }
            }
        }

        while (remainingQuantity > 0)
        {
            GameObject emptySlot = FindEmptySlot();
            if (emptySlot == null)
            {
                Debug.Log("Inventory đã đầy, không thể thêm hết!");
                TriggerPiclkupPopup(itemName, itemDef.itemPrefab.GetComponent<Image>().sprite);
                if (CraftingSystem.Instance != null) CraftingSystem.Instance.RefreshNeededItems();
                return;
            }

            GameObject newItemObj = Instantiate(itemDef.itemPrefab, emptySlot.transform);
            newItemObj.name = itemName + "(Clone)";
            InventoryItem newItemScript = newItemObj.GetComponent<InventoryItem>();

            int amountInNewStack = Mathf.Min(remainingQuantity, itemDef.maxStackSize);

            if (newItemScript != null)
            {
                newItemScript.itemID = itemName;
                newItemScript.quantity = amountInNewStack;
                newItemScript.maxStackSize = itemDef.maxStackSize;
                newItemScript.UpdateQuantityText();
            }
            remainingQuantity -= amountInNewStack;
        }

        TriggerPiclkupPopup(itemName, itemDef.itemPrefab.GetComponent<Image>().sprite);

        if (CraftingSystem.Instance != null)
        {
            CraftingSystem.Instance.RefreshNeededItems();
        }
    }


    public void RemoveItem(string nameToRemove, int amountToRemove)
    {
        int remainingToRemove = amountToRemove;

        for (int i = slotList.Count - 1; i >= 0; i--)
        {
            if (remainingToRemove <= 0) break;

            GameObject slot = slotList[i];
            if (slot.transform.childCount > 0)
            {
                GameObject itemObj = slot.transform.GetChild(0).gameObject;
                InventoryItem itemScript = itemObj.GetComponent<InventoryItem>();

                bool nameMatch = (itemScript != null && itemScript.itemID == nameToRemove);

                if (nameMatch)
                {
                    int amountInSlot = itemScript.quantity;
                    int amountToRemoveFromSlot = Mathf.Min(remainingToRemove, amountInSlot);

                    itemScript.quantity -= amountToRemoveFromSlot;
                    remainingToRemove -= amountToRemoveFromSlot;

                    if (itemScript.quantity <= 0)
                    {
                        Destroy(itemObj);
                    }
                    else
                    {
                        itemScript.UpdateQuantityText();
                    }
                }
            }
        }

        if (remainingToRemove > 0)
        {
            Debug.LogWarning($"Không thể xóa đủ {amountToRemove} {nameToRemove}. Chỉ xóa được {amountToRemove - remainingToRemove}.");
        }

        if (CraftingSystem.Instance != null)
        {
            CraftingSystem.Instance.RefreshNeededItems();
        }
        if (StorageManager.Instance != null && StorageManager.Instance.isOpen)
        {
             StorageManager.Instance.ReCalculateList();
        }
    }


    public int CountItem(string itemName)
    {
        int totalCount = 0;
        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount > 0)
            {
                InventoryItem itemScript = slot.transform.GetChild(0).GetComponent<InventoryItem>();
                if (itemScript != null && itemScript.itemID == itemName)
                {
                    totalCount += itemScript.quantity;
                }
            }
        }
        return totalCount;
    }


    void TriggerPiclkupPopup(string itemName, Sprite itemSprite)
    {
       if (pickupAlert != null && pickupName != null && pickupImage != null)
       {
            pickupAlert.SetActive(true);
            pickupName.text = itemName;
            pickupImage.sprite = itemSprite;
            StopCoroutine("HidePopupAfterDelay");
            StartCoroutine(HidePopupAfterDelay(2f));
       }
    }

    private IEnumerator HidePopupAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (pickupAlert != null) pickupAlert.SetActive(false);
    }

    public void PopupDelay()
    {
        if (pickupAlert != null) pickupAlert.SetActive(false);
    }

    private GameObject FindEmptySlot()
    {
        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount == 0)
            {
                return slot;
            }
        }
        return null;
    }

    public bool CheckSlotAvailable(int emptyMeeded)
    {
        int emptySlot = 0;
        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount <= 0)
            {
                emptySlot += 1;
            }
        }
        return (emptySlot >= emptyMeeded);
    }

    private GameObject itemPendingDeletion;

    public void ShowDeletionAlert(GameObject itemToDrop)
    {
        if (itemToDrop != null)
        {
            itemPendingDeletion = itemToDrop;
            if (deletionAlertUI != null) deletionAlertUI.SetActive(true);
            if (trashImageComponent != null) trashImageComponent.sprite = trashOpenSprite;
        }
    }

    public void ConfirmDeletion()
    {
        if (itemPendingDeletion != null)
        {
            // --- LOGIC XÓA STACK ---
            InventoryItem itemScript = itemPendingDeletion.GetComponent<InventoryItem>();
            if (itemScript != null)
            {
                // Gọi RemoveItem để xóa toàn bộ stack
                RemoveItem(itemScript.itemID, itemScript.quantity);
            }
            else
            {
                // Trường hợp dự phòng nếu không có script
                Destroy(itemPendingDeletion);
            }
            // --- HẾT LOGIC XÓA STACK ---

            itemPendingDeletion = null;
            if (deletionAlertUI != null) deletionAlertUI.SetActive(false);
            if (trashImageComponent != null) trashImageComponent.sprite = trashClosedSprite;

            // RemoveItem đã gọi RefreshNeededItems và ReCalculateList rồi
        }
    }


    public void CancelDeletion()
    {
        if (itemPendingDeletion != null)
        {
            DragDrop dd = itemPendingDeletion.GetComponent<DragDrop>();
            if (dd != null && dd.startParent != null) // Thêm kiểm tra startParent
            {
                itemPendingDeletion.transform.SetParent(dd.startParent);
                itemPendingDeletion.transform.position = dd.startPosition;
            }

            itemPendingDeletion = null;
            if (deletionAlertUI != null) deletionAlertUI.SetActive(false);
            if (trashImageComponent != null) trashImageComponent.sprite = trashClosedSprite;
        }
    }
}