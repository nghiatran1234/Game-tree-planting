using UnityEngine;
using System.Collections.Generic;
using System.IO; 
using System; 

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; set; }
    
    public static bool shouldLoadGame = false;

    public Transform playerTransform;
    public Transform cameraTransform;

    private string saveFilePath;
    public int currentSaveSlot = 1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        UpdateSaveFilePath();
    }
    private void UpdateSaveFilePath()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "savegame_" + currentSaveSlot + ".json");
        Debug.Log("Current save file path: " + saveFilePath);
    }

    // Hàm mới để MainMenu gọi khi chọn slot
    public void SetCurrentSaveSlot(int slotNumber)
    {
        currentSaveSlot = slotNumber;
        UpdateSaveFilePath();
    }

    private void Start()
    {
        if (playerTransform == null)
        {
            PlayerMovement pm = FindObjectOfType<PlayerMovement>();
            if (pm != null) playerTransform = pm.transform;
        }
        if (cameraTransform == null)
        {
            MouseMovement mm = FindObjectOfType<MouseMovement>();
            if (mm != null) cameraTransform = mm.transform;
        }

        if (shouldLoadGame)
        {
            LoadGame();
            shouldLoadGame = false; 
        }
    }

    public void SaveGame()
    {
        UpdateSaveFilePath();
        Debug.Log("Saving game to slot " + currentSaveSlot + "...");
        GameData data = new GameData();

        GatherPlayerData(data);
        GatherTransformData(data);
        GatherInventoryData(data);
        GatherEnvironmentData(data);
        GatherWorldData(data);

        string jsonData = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, jsonData);

        Debug.Log("Save successful to: " + saveFilePath);
    }

    public void LoadGame()
    {
        UpdateSaveFilePath();
        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("Save file not found for slot " + currentSaveSlot + ": " + saveFilePath);
            return;
        }

        Debug.Log("Loading game from slot " + currentSaveSlot + "...");
        string jsonData = File.ReadAllText(saveFilePath);
        GameData data = JsonUtility.FromJson<GameData>(jsonData);

        // --- Logic Xóa/Khôi phục giữ nguyên ---
        ClearOldWorldData();
        ClearOldInventoryData();
        RestorePlayerData(data);
        RestoreTransformData(data);
        RestoreInventoryData(data);
        RestoreEnvironmentData(data);
        RestoreWorldData(data);
        // --- ---

        Debug.Log("Load successful!");
    }

    private void GatherPlayerData(GameData data)
    {
        if (PlayerState.Instance == null) return;
        data.currentHealth = PlayerState.Instance.currentHealth;
        data.currentCalories = PlayerState.Instance.currentCalories;
        data.currentHydration = PlayerState.Instance.currentHydrationPercent;
        data.currentWetness = PlayerState.Instance.currentWetness;
    }

    private void GatherTransformData(GameData data)
    {
        data.playerPosX = playerTransform.position.x;
        data.playerPosY = playerTransform.position.y;
        data.playerPosZ = playerTransform.position.z;
        data.playerRotX = playerTransform.rotation.x;
        data.playerRotY = playerTransform.rotation.y;
        data.playerRotZ = playerTransform.rotation.z;
        data.playerRotW = playerTransform.rotation.w;

        data.cameraRotX = cameraTransform.localRotation.x;
        data.cameraRotY = cameraTransform.localRotation.y;
        data.cameraRotZ = cameraTransform.localRotation.z;
        data.cameraRotW = cameraTransform.localRotation.w;
    }

    private void GatherInventoryData(GameData data)
    {
        if (InventorySystem.Instance == null) return;
        InventorySystem.Instance.ReCalculateList(); 
        data.inventoryItems = new List<string>(InventorySystem.Instance.itemList);

        data.quickSlotItems = new List<string>();
        foreach (GameObject slot in EquipSystem.Instance.quickSlotsList)
        {
            if (slot.transform.childCount > 0)
            {
                data.quickSlotItems.Add(slot.transform.GetChild(0).GetComponent<InventoryItem>().itemID);
            }
            else
            {
                data.quickSlotItems.Add("null"); 
            }
        }
    }

    private void GatherEnvironmentData(GameData data)
    {
        if (EnvironmentManager.Instance == null) return;
        data.dayCounter = EnvironmentManager.Instance.dayCounter;
        data.currentSeason = EnvironmentManager.Instance.currentSeason.ToString();
        data.currentWeather = EnvironmentManager.Instance.currentWeather.ToString();
    }

    private void GatherWorldData(GameData data)
    {
        data.placedItems = new List<WorldItemData>();
        foreach (SaveTrigger item in FindObjectsOfType<SaveTrigger>())
        {
            WorldItemData itemData = new WorldItemData(
                item.itemName,
                item.transform.position,
                item.transform.rotation
            );

            StorageBox box = item.GetComponent<StorageBox>();
            if (box != null)
            {
                itemData.items = new List<string>(box.items);
            }

            data.placedItems.Add(itemData);
        }
    }

    private void RestorePlayerData(GameData data)
    {
        if (PlayerState.Instance == null) return;
        PlayerState.Instance.currentHealth = data.currentHealth;
        PlayerState.Instance.currentCalories = data.currentCalories;
        PlayerState.Instance.currentHydrationPercent = data.currentHydration;
        PlayerState.Instance.currentWetness = data.currentWetness;
    }

    private void RestoreTransformData(GameData data)
    {
        CharacterController cc = playerTransform.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        playerTransform.position = new Vector3(data.playerPosX, data.playerPosY, data.playerPosZ);
        playerTransform.rotation = new Quaternion(data.playerRotX, data.playerRotY, data.playerRotZ, data.playerRotW);
        cameraTransform.localRotation = new Quaternion(data.cameraRotX, data.cameraRotY, data.cameraRotZ, data.cameraRotW);

        MouseMovement mm = cameraTransform.GetComponent<MouseMovement>();
        if (mm != null)
        {
            mm.SetRotation(cameraTransform.localEulerAngles.x, playerTransform.eulerAngles.y);
        }

        if (cc != null) cc.enabled = true;
    }

    private void ClearOldInventoryData()
    {
        if (InventorySystem.Instance == null) return;
        foreach (GameObject slot in InventorySystem.Instance.slotList)
        {
            if (slot.transform.childCount > 0)
            {
                Destroy(slot.transform.GetChild(0).gameObject);
            }
        }
        foreach (GameObject slot in EquipSystem.Instance.quickSlotsList)
        {
            if (slot.transform.childCount > 0)
            {
                Destroy(slot.transform.GetChild(0).gameObject);
            }
        }
        InventorySystem.Instance.itemList.Clear();
    }

    private void RestoreInventoryData(GameData data)
    {
        if (InventorySystem.Instance == null || EquipSystem.Instance == null)
        {
             Debug.LogError("InventorySystem or EquipSystem Instance is null during RestoreInventoryData!");
             return;
        }

        foreach (string item in data.inventoryItems)
        {
            if (!string.IsNullOrEmpty(item))
            {
                InventorySystem.Instance.AddToInventory(item);
            }
        }

        if (EquipSystem.Instance.quickSlotsList == null || EquipSystem.Instance.quickSlotsList.Count == 0)
        {
             Debug.LogWarning("EquipSystem quickSlotsList was not populated. Attempting to populate now...");
             EquipSystem.Instance.PopulateSlotList();

             if (EquipSystem.Instance.quickSlotsList == null || EquipSystem.Instance.quickSlotsList.Count == 0)
             {
                  Debug.LogError("Failed to populate EquipSystem quickSlotsList even after manual call!");
                  InventorySystem.Instance.ReCalculateList();
                  if (CraftingSystem.Instance != null) CraftingSystem.Instance.RefreshNeededItems();
                  return;
             }
        }

        int slotsToRestore = Mathf.Min(data.quickSlotItems.Count, EquipSystem.Instance.quickSlotsList.Count);
        Debug.Log($"Restoring {slotsToRestore} quick slots. Saved: {data.quickSlotItems.Count}, Current: {EquipSystem.Instance.quickSlotsList.Count}");

        for (int i = 0; i < slotsToRestore; i++)
        {
            if (i < data.quickSlotItems.Count && data.quickSlotItems[i] != "null" && !string.IsNullOrEmpty(data.quickSlotItems[i]))
            {
                InventorySystem.ItemDefinition itemDef = InventorySystem.Instance.itemDatabase.Find(def => def.itemName == data.quickSlotItems[i]);
                if (itemDef != null)
                {
                     if (i < EquipSystem.Instance.quickSlotsList.Count && EquipSystem.Instance.quickSlotsList[i] != null)
                     {
                         foreach (Transform child in EquipSystem.Instance.quickSlotsList[i].transform)
                         {
                             Destroy(child.gameObject);
                         }

                         GameObject itemUI = Instantiate(itemDef.itemPrefab, EquipSystem.Instance.quickSlotsList[i].transform);
                         InventoryItem inventoryItemScript = itemUI.GetComponent<InventoryItem>();
                         if (inventoryItemScript != null)
                         {
                            inventoryItemScript.isInsideQuickSlot = true;
                            inventoryItemScript.itemID = itemDef.itemName;
                         }
                         else
                         {
                             Debug.LogError($"Item Prefab UI for '{itemDef.itemName}' is missing InventoryItem script!");
                             Destroy(itemUI);
                         }
                     }
                     else
                     {
                         Debug.LogError($"Quick slot index {i} is invalid or the slot GameObject is null in EquipSystem!");
                     }
                }
                 else
                 {
                     Debug.LogWarning($"Could not find item definition in Inventory Database for quick slot item: '{data.quickSlotItems[i]}'");
                 }
            }
             else if (i >= data.quickSlotItems.Count)
            {
                 Debug.LogWarning($"Save data quickSlotItems has fewer items ({data.quickSlotItems.Count}) than current slots ({EquipSystem.Instance.quickSlotsList.Count}). Index {i} ignored.");
            }
        }

        InventorySystem.Instance.ReCalculateList();

        if (CraftingSystem.Instance != null)
        {
            CraftingSystem.Instance.RefreshNeededItems();
        }
    }

    private void RestoreEnvironmentData(GameData data)
    {
        if (EnvironmentManager.Instance == null) return;
        EnvironmentManager.Instance.dayCounter = data.dayCounter;
        EnvironmentManager.Instance.currentSeason = (EnvironmentManager.Season)Enum.Parse(typeof(EnvironmentManager.Season), data.currentSeason);
        EnvironmentManager.Instance.currentWeather = (EnvironmentManager.Weather)Enum.Parse(typeof(EnvironmentManager.Weather), data.currentWeather);
    }

    private void ClearOldWorldData()
    {
        foreach (SaveTrigger item in FindObjectsOfType<SaveTrigger>())
        {
            Destroy(item.gameObject);
        }
    }

    private void RestoreWorldData(GameData data)
    {
        if (ConstructionManager.Instance == null) return;
        foreach (WorldItemData itemData in data.placedItems)
        {
            ConstructionManager.ConstructionItem itemDef = ConstructionManager.Instance.constructionDatabase.Find(item => item.name == itemData.itemName);
            
            if (itemDef != null)
            {
                GameObject prefab = itemDef.itemPrefab;
                Vector3 pos = new Vector3(itemData.posX, itemData.posY, itemData.posZ);
                Quaternion rot = new Quaternion(itemData.rotX, itemData.rotY, itemData.rotZ, itemData.rotW);

                GameObject placedItem = Instantiate(prefab, pos, rot);
                
                placedItem.AddComponent<SaveTrigger>().itemName = itemData.itemName;
                
                Constructable constructScript = placedItem.GetComponent<Constructable>();
                if(constructScript != null)
                {
                    constructScript.enabled = false;
                    constructScript.solidCollider.enabled = true;
                }

                StorageBox box = placedItem.GetComponent<StorageBox>();
                if (box != null)
                {
                    box.items = new List<string>(itemData.items);
                }
            }
        }
    }
}