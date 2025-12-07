using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }

    [Header("Tham chiếu")]
    [SerializeField] private Transform slotsParent;
    [SerializeField] private GameObject inventoryPanel;
    [Header("Tiền tệ")]
    [SerializeField] private TMPro.TextMeshProUGUI goldText; // Kéo cái Gold_Text vào đây

    private InventorySlot_UI[] slots;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        slots = slotsParent.GetComponentsInChildren<InventorySlot_UI>();
        InventoryManager.Instance.OnInventoryChanged += UpdateUI;
        inventoryPanel.SetActive(false);
        UpdateUI();
    }

    private void UpdateUI()
    {
        List<InventorySlot> inventory = InventoryManager.Instance.inventory;
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.Count)
                slots[i].SetItem(inventory[i].itemData, inventory[i].quantity);
            else
                slots[i].ClearSlot();
        }
        // Cập nhật tiền
        if (goldText != null)
        {
            goldText.text = "Gold: " + InventoryManager.Instance.currentGold.ToString();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    // Hàm bật tắt thông minh
    public void ToggleInventory()
    {
        bool isOpening = !inventoryPanel.activeSelf;

        if (isOpening)
        {
            // Nếu mở túi bằng tay (Tab) -> Cũng tạm ẩn Alchemy
            if (AlchemyUI.Instance != null) AlchemyUI.Instance.HidePanel();
        }

        inventoryPanel.SetActive(isOpening);
    }

    // Hàm để bên Alchemy gọi sang tắt túi
    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);
    }
    public void OpenInventoryForSelection()
    {
        // Khi Alchemy nhờ mở túi -> Chỉ Tạm Ẩn bảng Alchemy thôi
        if (AlchemyUI.Instance != null) AlchemyUI.Instance.HidePanel();

        inventoryPanel.SetActive(true);
    }

}