using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private Transform slotsParent;
    [SerializeField] private GameObject inventoryPanel;

    [Header("Gold UI")]
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI goldTextInventory;

    private InventorySlot_UI[] slots;

    // ==============================
    // Unity Lifecycle
    // ==============================
    private void Awake()
    {
        SetupSingleton();
    }

    private void Start()
    {
        CacheSlots();
        SubscribeEvents();

        inventoryPanel.SetActive(false);
        RefreshUI();
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    private void Update()
    {
        HandleInput();
    }

    // ==============================
    // Setup
    // ==============================
    private void SetupSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void CacheSlots()
    {
        slots = slotsParent.GetComponentsInChildren<InventorySlot_UI>();
    }

    private void SubscribeEvents()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged += RefreshUI;
    }

    private void UnsubscribeEvents()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged -= RefreshUI;
    }

    // ==============================
    // Input
    // ==============================
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    // ==============================
    // UI Update
    // ==============================
    private void RefreshUI()
    {
        UpdateSlots();
        UpdateGold();
    }

    private void UpdateSlots()
    {
        if (InventoryManager.Instance == null) return;

        List<InventorySlot> inventory = InventoryManager.Instance.inventory;

        if (slots == null || slots.Length == 0)
        {
            Debug.LogError("Chưa tìm thấy Slot UI nào! Hãy kiểm tra Slots Parent.");
            return;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.Count)
            {
                if (inventory[i] != null)
                    slots[i].SetItem(inventory[i].item, inventory[i].quantity);
            }
            else
            {
                slots[i].Clear();
            }
        }
    }



    private void UpdateGold()
    {
        if (InventoryManager.Instance == null) return;

        string goldValue = InventoryManager.Instance.currentGold.ToString();

        if (goldText != null)
            goldText.text = goldValue;
        else
            Debug.LogWarning("Chưa gán Gold Text trong Inspector!");

        if (goldTextInventory != null)
            goldTextInventory.text = goldValue;
    }

    // ==============================
    // Public API (Shop / Alchemy)
    // ==============================
    public void ToggleInventory()
    {
        bool isOpening = !inventoryPanel.activeSelf;

        if (isOpening)
        {
            // Chỉ tắt Alchemy
            if (AlchemyUI.Instance != null)
                AlchemyUI.Instance.HidePanel();
        }

        inventoryPanel.SetActive(isOpening);
    }

    public void OpenInventory()
    {
        inventoryPanel.SetActive(true);
    }

    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);
    }

    public void OpenInventoryForSelection()
    {
        if (AlchemyUI.Instance != null)
            AlchemyUI.Instance.HidePanel();

        inventoryPanel.SetActive(true);
    }
}
