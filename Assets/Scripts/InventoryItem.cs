using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
// using TMPro; // Uncomment if using TextMeshPro

public class InventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerClickHandler
{
    public bool isTrashable;

    private GameObject inventoryItemInfoUI;
    private Text inventoryItemInfoUI_itemName;
    private Text inventoryItemInfoUI_itemDescription;
    private Text inventoryItemInfoUI_itemFunctionality;

    private GameObject storageItemInfoUI;
    private Text storageItemInfoUI_itemName;
    private Text storageItemInfoUI_itemDescription;
    private Text storageItemInfoUI_itemFunctionality;


    public string itemID;
    public string thisName;
    public string thisDescription;
    public string thisFunctionality;

    [Header("Stacking")]
    public int quantity = 1;
    public int maxStackSize = 64;
    public GameObject quantityTextPrefab; // Kéo prefab ItemQuantityText vào đây
    private Text quantityText;

    private GameObject itemPendingConsumption;
    public bool isConsumable;
    public float healthEffect;
    public float caloriesEffect;
    public float hydrationEffect;

    public bool isEquipable;
    private GameObject itemPendingEquipping;
    public bool isInsideQuickSlot;

    public bool isSelected;

    public bool isUseable;
    public GameObject itemPendingToUsed;

    private void Start()
    {
        if (InventorySystem.Instance != null && InventorySystem.Instance.ItemInfoUI != null)
        {
            inventoryItemInfoUI = InventorySystem.Instance.ItemInfoUI;
            inventoryItemInfoUI_itemName = inventoryItemInfoUI.transform.Find("ItemName")?.GetComponent<Text>();
            inventoryItemInfoUI_itemDescription = inventoryItemInfoUI.transform.Find("ItemDescription")?.GetComponent<Text>();
            inventoryItemInfoUI_itemFunctionality = inventoryItemInfoUI.transform.Find("ItemFunctionality")?.GetComponent<Text>();
        }

        if (StorageManager.Instance != null && StorageManager.Instance.storageItemInfoUI != null)
        {
             storageItemInfoUI = StorageManager.Instance.storageItemInfoUI;
             storageItemInfoUI_itemName = StorageManager.Instance.storageItemName;
             storageItemInfoUI_itemDescription = StorageManager.Instance.storageItemDescription;
             storageItemInfoUI_itemFunctionality = StorageManager.Instance.storageItemFunctionality;
        }

        // Tạo text hiển thị số lượng
        GameObject quantityObj = Instantiate(quantityTextPrefab, transform);
        quantityText = quantityObj.GetComponent<Text>();
        
        UpdateQuantityText();
    }

    public void UpdateQuantityText()
    {
        if (quantityText != null)
        {
            if (quantity > 1)
            {
                quantityText.text = quantity.ToString();
                quantityText.gameObject.SetActive(true);
            }
            else
            {
                quantityText.text = "";
                quantityText.gameObject.SetActive(false);
            }
        }
    }


    void Update()
    {
        DragDrop dd = gameObject.GetComponent<DragDrop>();
        if (dd != null)
        {
            dd.enabled = !isSelected;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ItemSlot parentSlot = GetComponentInParent<ItemSlot>();
        if (parentSlot == null) return;

        bool isInStorageSlot = parentSlot.CompareTag("StorageSlot");

        if (isInStorageSlot && storageItemInfoUI != null)
        {
            storageItemInfoUI.SetActive(true);
            if(storageItemInfoUI_itemName != null) storageItemInfoUI_itemName.text = thisName;
            if(storageItemInfoUI_itemDescription != null) storageItemInfoUI_itemDescription.text = thisDescription;
            if(storageItemInfoUI_itemFunctionality != null) storageItemInfoUI_itemFunctionality.text = thisFunctionality;
        }
        else if (!isInStorageSlot && inventoryItemInfoUI != null)
        {
            inventoryItemInfoUI.SetActive(true);
            if(inventoryItemInfoUI_itemName != null) inventoryItemInfoUI_itemName.text = thisName;
            if(inventoryItemInfoUI_itemDescription != null) inventoryItemInfoUI_itemDescription.text = thisDescription;
            if(inventoryItemInfoUI_itemFunctionality != null) inventoryItemInfoUI_itemFunctionality.text = thisFunctionality;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (storageItemInfoUI != null)
        {
            storageItemInfoUI.SetActive(false);
        }
        if (inventoryItemInfoUI != null)
        {
            inventoryItemInfoUI.SetActive(false);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ItemSlot parentSlot = GetComponentInParent<ItemSlot>();
            // Only allow equipping from Inventory or Quick Slots, not Storage
            if (parentSlot != null && !parentSlot.CompareTag("StorageSlot"))
            {
                if (isEquipable && !isInsideQuickSlot && EquipSystem.Instance != null && !EquipSystem.Instance.CheckIfFull())
                {
                    EquipSystem.Instance.AddToQuickSlots(gameObject);
                    isInsideQuickSlot = true;
                }
            }
        }
    }

    private void UseItem()
    {
        if (inventoryItemInfoUI != null) inventoryItemInfoUI.SetActive(false);
        if (storageItemInfoUI != null) storageItemInfoUI.SetActive(false);

        if (InventorySystem.Instance != null && InventorySystem.Instance.isOpen)
        {
            InventorySystem.Instance.CloseInventory();
        }

        if (CraftingSystem.Instance != null && CraftingSystem.Instance.isOpen)
        {
            CraftingSystem.Instance.isOpen = false;
            if(CraftingSystem.Instance.craftingUI != null) CraftingSystem.Instance.craftingUI.SetActive(false);
            // Close other crafting screens if necessary
        }

        if (StorageManager.Instance != null && StorageManager.Instance.isOpen)
        {
             StorageManager.Instance.CloseStorage();
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (SelectionManager.instance != null)
        {
            SelectionManager.instance.EnableSelection();
             if(SelectionManager.instance.GetComponent<SelectionManager>() != null)
                SelectionManager.instance.GetComponent<SelectionManager>().enabled = true;
        }

        switch (itemID)
        {
            case "Foundation":
            case "Wall":
            case "StorageBox": // Add other placeables
                if (ConstructionManager.Instance != null)
                {
                    ConstructionManager.ConstructionItem itemDef = ConstructionManager.Instance.constructionDatabase.Find(item => item.name == itemID);
                     if(itemDef != null)
                     {
                         ConstructionManager.Instance.ActivateConstructionPlacement(itemDef.name);
                     } else {
                         Debug.LogError("Construction item definition not found for: " + itemID);
                     }
                }
                break;
            default:
                break;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ItemSlot parentSlot = GetComponentInParent<ItemSlot>();
            bool isInStorage = parentSlot != null && parentSlot.CompareTag("StorageSlot");

            if (isConsumable)
            {
                itemPendingConsumption = gameObject;
                consumingFunction(healthEffect, caloriesEffect, hydrationEffect);

                quantity--;
                if (quantity <= 0)
                {
                    Destroy(gameObject);
                }
                else
                {
                    UpdateQuantityText();
                }

                if (CraftingSystem.Instance != null && !isInStorage)
                    CraftingSystem.Instance.RefreshNeededItems(); // Refresh crafting if consumed from inventory
                if (StorageManager.Instance != null && isInStorage)
                    StorageManager.Instance.ReCalculateList(); // Update storage list if consumed from storage
            }

            if (isUseable)
            {
                itemPendingToUsed = gameObject;
                UseItem();
                // Note: Placeable items are usually *not* consumed on Left Click.
                // Consumption happens when ConstructionManager places the item.
                // If you have other 'Useable' items that *should* be consumed, add logic here.
                /*
                quantity--;
                if (quantity <= 0) {
                    Destroy(gameObject);
                } else {
                    UpdateQuantityText();
                }
                // Update lists
                */
            }
        }
    }


    private void consumingFunction(float healthEffect, float caloriesEffect, float hydrationEffect)
    {
        if (inventoryItemInfoUI != null) inventoryItemInfoUI.SetActive(false);
        if (storageItemInfoUI != null) storageItemInfoUI.SetActive(false);

        if (PlayerState.Instance == null) return;

        healthEffectCalculation(healthEffect);
        caloriesEffectCalculation(caloriesEffect);
        hydrationEffectCalculation(hydrationEffect);
    }

    private static void healthEffectCalculation(float healthEffect)
    {
        if (healthEffect == 0 || PlayerState.Instance == null) return;

        float healthBeforeConsumption = PlayerState.Instance.currentHealth;
        float maxHealth = PlayerState.Instance.maxHealth;
        float newHealth = Mathf.Min(healthBeforeConsumption + healthEffect, maxHealth);
        PlayerState.Instance.setHealth(newHealth);
    }

    private static void caloriesEffectCalculation(float caloriesEffect)
    {
        if (caloriesEffect == 0 || PlayerState.Instance == null) return;

        float caloriesBeforeConsumption = PlayerState.Instance.currentCalories;
        float maxCalories = PlayerState.Instance.maxCalories;
        float newCalories = Mathf.Min(caloriesBeforeConsumption + caloriesEffect, maxCalories);
        PlayerState.Instance.setCalories(newCalories);
    }

    private static void hydrationEffectCalculation(float hydrationEffect)
    {
        if (hydrationEffect == 0 || PlayerState.Instance == null) return;

        float hydrationBeforeConsumption = PlayerState.Instance.currentHydrationPercent;
        float maxHydration = PlayerState.Instance.maxHydrationPercent;
        float newHydration = Mathf.Min(hydrationBeforeConsumption + hydrationEffect, maxHydration);
        PlayerState.Instance.setHydration(newHydration);
    }
}