using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }

    [Header("Tham chiếu")]
    [SerializeField] private Transform slotsParent;
    [SerializeField] private GameObject inventoryPanel;
    [Header("Tiền tệ")]
    [SerializeField] private TMPro.TextMeshProUGUI goldText;
    [SerializeField] private TMPro.TextMeshProUGUI goldTextInventory;

    private InventorySlot_UI[] slots;

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
            goldText.text = "" + InventoryManager.Instance.currentGold.ToString();
        }
        if(goldTextInventory != null)
        {
            goldTextInventory.text = "" + InventoryManager.Instance.currentGold.ToString();
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
            // [SỬA LẠI ĐOẠN NÀY]
            // Chỉ tắt Alchemy (Lò luyện) thôi
            if (AlchemyUI.Instance != null)
            {
                AlchemyUI.Instance.HidePanel();
            }

            // KHÔNG tắt ShopUI! Hãy để Shop và Túi sống chung hòa bình.
            // (Xóa hoặc comment dòng ShopUI.CloseShop() nếu có)
        }

        inventoryPanel.SetActive(isOpening);
    }

    // Hàm này để Shop gọi (Bắt buộc mở túi)
    public void OpenInventory()
    {
        // Khi Shop gọi mở, ta chỉ cần hiện lên thôi, không cần tắt ai cả
        inventoryPanel.SetActive(true);
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