using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorageManager : MonoBehaviour
{
    public static StorageManager Instance { get; set; }

    public GameObject storageUI;
    public List<GameObject> slotList = new List<GameObject>();
    public StorageBox currentOpenBox;

    public bool isOpen;

    [Header("Storage Item Info UI")]
    public GameObject storageItemInfoUI;
    public Text storageItemName;
    public Text storageItemDescription;
    public Text storageItemFunctionality;

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
        isOpen = false;
        PopulateSlotList();
        if (storageItemInfoUI != null) storageItemInfoUI.SetActive(false);
    }

    private void PopulateSlotList()
    {
        foreach (Transform child in storageUI.transform)
        {
            if (child.CompareTag("Slot"))
            {
                slotList.Add(child.gameObject);
            }
        }
    }

    public void OpenStorage(StorageBox boxToOpen)
    {
        currentOpenBox = boxToOpen;
        storageUI.SetActive(true);
        isOpen = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (SelectionManager.instance != null)
        {
            SelectionManager.instance.DisableSelection();
            SelectionManager.instance.GetComponent<SelectionManager>().enabled = false;
        }

        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.inventoryScreenUI.SetActive(true);
            InventorySystem.Instance.isOpen = true;
            if (InventorySystem.Instance.ItemInfoUI != null)
            {
                InventorySystem.Instance.ItemInfoUI.SetActive(false);
            }
        }

        if (storageItemInfoUI != null) storageItemInfoUI.SetActive(false);

        PopulateStorageUI();
    }

    private void PopulateStorageUI()
    {
        if (currentOpenBox == null || InventorySystem.Instance == null) return;

        foreach (GameObject slot in slotList)
        {
            foreach (Transform child in slot.transform)
            {
                Destroy(child.gameObject);
            }
        }

        foreach (string itemName in currentOpenBox.items)
        {
            GameObject emptySlot = FindEmptySlot();
            if (emptySlot == null)
            {
                Debug.Log("Storage UI đã đầy!");
                break;
            }

            InventorySystem.ItemDefinition itemDef = InventorySystem.Instance.itemDatabase.Find(item => item.itemName == itemName);
            if (itemDef != null)
            {
                GameObject itemToAdd = Instantiate(itemDef.itemPrefab, emptySlot.transform);
                itemToAdd.name = itemName + "(Clone)";
                InventoryItem itemScript = itemToAdd.GetComponent<InventoryItem>();
                if (itemScript != null)
                {
                    itemScript.itemID = itemName;
                }
            }
        }
    }

    public void CloseStorage()
    {
        ReCalculateList();

        currentOpenBox = null;
        storageUI.SetActive(false);
        isOpen = false;

        if (storageItemInfoUI != null) storageItemInfoUI.SetActive(false);

        if (InventorySystem.Instance != null)
        {
            if (InventorySystem.Instance.ItemInfoUI != null)
            {
                InventorySystem.Instance.ItemInfoUI.SetActive(false);
            }
            InventorySystem.Instance.inventoryScreenUI.SetActive(false);
            InventorySystem.Instance.isOpen = false;
        }


        if (PauseMenu.Instance != null && !PauseMenu.Instance.isPaused &&
            DialogueManager.Instance != null && !DialogueManager.Instance.isDialogueActive)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (SelectionManager.instance != null)
            {
                SelectionManager.instance.EnableSelection();
                SelectionManager.instance.GetComponent<SelectionManager>().enabled = true;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) && isOpen && !PauseMenu.Instance.isPaused)
        {
            CloseStorage();
        }
    }

    public void ReCalculateList()
    {
        if (currentOpenBox == null) return;

        currentOpenBox.items.Clear();

        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount > 0)
            {
                GameObject itemObj = slot.transform.GetChild(0).gameObject;
                InventoryItem itemScript = itemObj.GetComponent<InventoryItem>();

                if (itemScript != null && !string.IsNullOrEmpty(itemScript.itemID))
                {
                    currentOpenBox.items.Add(itemScript.itemID);
                }
            }
        }
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
}