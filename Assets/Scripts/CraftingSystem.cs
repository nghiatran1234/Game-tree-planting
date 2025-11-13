using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingSystem : MonoBehaviour
{
    public GameObject craftingUI;
    public GameObject toolScreenUI, survivalScreenUI, refineScreenUI, constructionScreenUI;

    //Category Buttons
    Button toolsButton, survivalButton, refineButton, constructionButton;

    // --- Tool Items ---
    Button AxeButton;
    Text Req1, Req2;

    // --- Refine Items ---
    Button craftPlankButton;
    Text PlankReq1;

    // --- Construction ---
    Button craftFoundationButton;
    Button craftWallButton;
    Text FoundationReq1;
    Text WallReq1;

    public bool isOpen;

    public Blueprint AxeBlueprint;
    public Blueprint PlankBlueprint;
    public Blueprint FoundationBlueprint;
    public Blueprint WallBlueprint;
    public Blueprint BowBlueprint;
    public Blueprint ArrowBlueprint;

    public List<string> inventoryItemList = new List<string>();
    public static CraftingSystem Instance { get; set; }

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

    void Start()
    {
        isOpen = false;

        // --- Gán các nút Category ---
        toolsButton = craftingUI.transform.Find("ToolsButton").GetComponent<Button>();
        toolsButton.onClick.AddListener(delegate { OpenToolsCategory(); });

        survivalButton = craftingUI.transform.Find("SurvivalButton").GetComponent<Button>();
        survivalButton.onClick.AddListener(delegate { OpenSurvivalCategory(); });

        refineButton = craftingUI.transform.Find("RefineButton").GetComponent<Button>();
        refineButton.onClick.AddListener(delegate { OpenRefineCategory(); });

        constructionButton = craftingUI.transform.Find("ConstructionButton").GetComponent<Button>();
        constructionButton.onClick.AddListener(delegate { OpenConstructionCategory(); });

        // --- Gán item trong Tool Screen ---
        Req1 = toolScreenUI.transform.Find("Axe").transform.Find("Req1").GetComponent<Text>();
        Req2 = toolScreenUI.transform.Find("Axe").transform.Find("Req2").GetComponent<Text>();
        AxeButton = toolScreenUI.transform.Find("Axe").transform.Find("Button").GetComponent<Button>();
        AxeButton.onClick.AddListener(delegate { CraftAnyItem(AxeBlueprint); });

        // --- Refine Screen ---
        PlankReq1 = refineScreenUI.transform.Find("Plank").transform.Find("Req1").GetComponent<Text>();
        craftPlankButton = refineScreenUI.transform.Find("Plank").transform.Find("Button").GetComponent<Button>();
        craftPlankButton.onClick.AddListener(delegate { CraftAnyItem(PlankBlueprint); });

        // --- Construction Screen ---
        FoundationReq1 = constructionScreenUI.transform.Find("Foundation").transform.Find("Req1").GetComponent<Text>();
        craftFoundationButton = constructionScreenUI.transform.Find("Foundation").transform.Find("Button").GetComponent<Button>();
        craftFoundationButton.onClick.AddListener(delegate { CraftAnyItem(FoundationBlueprint); });

        WallReq1 = constructionScreenUI.transform.Find("Wall").transform.Find("Req1").GetComponent<Text>();
        craftWallButton = constructionScreenUI.transform.Find("Wall").transform.Find("Button").GetComponent<Button>();
        craftWallButton.onClick.AddListener(delegate { CraftAnyItem(WallBlueprint); });
    }

    void OpenToolsCategory()
    {
        craftingUI.SetActive(false);
        toolScreenUI.SetActive(true);
    }

    void OpenSurvivalCategory()
    {
        craftingUI.SetActive(false);
        toolScreenUI.SetActive(false);
        survivalScreenUI.SetActive(true);
    }

    void OpenRefineCategory()
    {
        craftingUI.SetActive(false);
        toolScreenUI.SetActive(false);
        survivalScreenUI.SetActive(false);
        refineScreenUI.SetActive(true);
    }

    void OpenConstructionCategory()
    {
        craftingUI.SetActive(false);
        toolScreenUI.SetActive(false);
        survivalScreenUI.SetActive(false);
        refineScreenUI.SetActive(false);
        constructionScreenUI.SetActive(true);
    }

    // ✅ ĐÃ SỬA: thêm đúng số lượng, log đầy đủ
    void CraftAnyItem(Blueprint blueprintToCraft)
    {
        if (blueprintToCraft == null)
        {
            Debug.LogError("❌ Blueprint bị null! Kiểm tra slot trong Inspector.");
            return;
        }

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySound(SoundManager.Instance.craftingSound);

        StartCoroutine(craftedDelayForSound(blueprintToCraft));

        if (InventorySystem.Instance == null)
        {
            Debug.LogError(" InventorySystem.Instance = null! Không thể xóa nguyên liệu.");
            return;
        }

        // Trừ nguyên liệu
        if (blueprintToCraft.nunOfRequirements == 1)
        {
            InventorySystem.Instance.RemoveItem(blueprintToCraft.Req1, blueprintToCraft.Req1Amount);
        }
        else if (blueprintToCraft.nunOfRequirements == 2)
        {
            InventorySystem.Instance.RemoveItem(blueprintToCraft.Req1, blueprintToCraft.Req1Amount);
            InventorySystem.Instance.RemoveItem(blueprintToCraft.Req2, blueprintToCraft.Req2Amount);
        }

        StartCoroutine(calculate());
    }

    IEnumerator craftedDelayForSound(Blueprint blueprintToCraft)
    {
        yield return new WaitForSeconds(1f);

        if (InventorySystem.Instance == null)
        {
            Debug.LogError(" InventorySystem.Instance = null! Không thể thêm item mới.");
            yield break;
        }

        if (string.IsNullOrEmpty(blueprintToCraft.itemName))
        {
            Debug.LogError(" Blueprint không có itemName hợp lệ!");
            yield break;
        }

        if (blueprintToCraft.numberOfProducedItems <= 0)
        {
            Debug.LogWarning($"⚠ Blueprint {blueprintToCraft.itemName} có số lượng sản xuất = 0. Không thêm item.");
            yield break;
        }

        Debug.Log($" Craft thành công: {blueprintToCraft.itemName} x{blueprintToCraft.numberOfProducedItems}");

        // ✅ Thêm đúng số lượng vào Inventory
        InventorySystem.Instance.AddToInventory(blueprintToCraft.itemName, blueprintToCraft.numberOfProducedItems);

        InventorySystem.Instance.ReCalculateList();
        RefreshNeededItems();
    }

    public IEnumerator calculate()
    {
        yield return 0;
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.ReCalculateList();
            RefreshNeededItems();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && !isOpen && !PauseMenu.Instance.isPaused)
        {
            craftingUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (SelectionManager.instance != null)
            {
                SelectionManager.instance.DisableSelection();
                SelectionManager.instance.GetComponent<SelectionManager>().enabled = false;
            }

            isOpen = true;
        }
        else if (Input.GetKeyDown(KeyCode.C) && isOpen && !PauseMenu.Instance.isPaused)
        {
            craftingUI.SetActive(false);
            toolScreenUI.SetActive(false);
            survivalScreenUI.SetActive(false);
            refineScreenUI.SetActive(false);
            constructionScreenUI.SetActive(false);

            if (InventorySystem.Instance != null && !InventorySystem.Instance.isOpen)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                if (SelectionManager.instance != null)
                {
                    SelectionManager.instance.EnableSelection();
                    SelectionManager.instance.GetComponent<SelectionManager>().enabled = true;
                }
            }

            isOpen = false;
        }
    }

    public void RefreshNeededItems()
    {
        if (InventorySystem.Instance == null) return;

        int stone_Count = InventorySystem.Instance.CountItem("Stone");
        int stick_Count = InventorySystem.Instance.CountItem("Stick");
        int log_Count = InventorySystem.Instance.CountItem("Log");
        int plank_Count = InventorySystem.Instance.CountItem("Plank");

        //Axe
        if (AxeBlueprint != null)
        {
            if (Req1 != null) Req1.text = AxeBlueprint.Req1Amount + " " + AxeBlueprint.Req1 + "[ " + stone_Count + "]";
            if (Req2 != null) Req2.text = AxeBlueprint.Req2Amount + " " + AxeBlueprint.Req2 + "[ " + stick_Count + "]";

            if (AxeButton != null)
            {
                if (stone_Count >= AxeBlueprint.Req1Amount && stick_Count >= AxeBlueprint.Req2Amount && InventorySystem.Instance.CheckSlotAvailable(AxeBlueprint.numberOfProducedItems))
                    AxeButton.gameObject.SetActive(true);
                else
                    AxeButton.gameObject.SetActive(false);
            }
        }

        //Plank
        if (PlankBlueprint != null)
        {
            if (PlankReq1 != null) PlankReq1.text = PlankBlueprint.Req1Amount + " " + PlankBlueprint.Req1 + "[ " + log_Count + "]";

            if (craftPlankButton != null)
            {
                if (log_Count >= PlankBlueprint.Req1Amount && InventorySystem.Instance.CheckSlotAvailable(PlankBlueprint.numberOfProducedItems))
                    craftPlankButton.gameObject.SetActive(true);
                else
                    craftPlankButton.gameObject.SetActive(false);
            }
        }

        //Foundation
        if (FoundationBlueprint != null)
        {
            if (FoundationReq1 != null) FoundationReq1.text = FoundationBlueprint.Req1Amount + " " + FoundationBlueprint.Req1 + "[ " + plank_Count + "]";

            if (craftFoundationButton != null)
            {
                if (plank_Count >= FoundationBlueprint.Req1Amount && InventorySystem.Instance.CheckSlotAvailable(FoundationBlueprint.numberOfProducedItems))
                    craftFoundationButton.gameObject.SetActive(true);
                else
                    craftFoundationButton.gameObject.SetActive(false);
            }
        }

        //Wall
        if (WallBlueprint != null)
        {
            if (WallReq1 != null) WallReq1.text = WallBlueprint.Req1Amount + " " + WallBlueprint.Req1 + "[ " + plank_Count + "]";

            if (craftWallButton != null)
            {
                if (plank_Count >= WallBlueprint.Req1Amount && InventorySystem.Instance.CheckSlotAvailable(WallBlueprint.numberOfProducedItems))
                    craftWallButton.gameObject.SetActive(true);
                else
                    craftWallButton.gameObject.SetActive(false);
            }
        }
    }
}
