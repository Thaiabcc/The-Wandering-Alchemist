using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("Tham chiếu")]
    [SerializeField] private Transform slotsParent; // Cái lưới (Slots_Grid)
    [SerializeField] private GameObject inventoryPanel; // Cái bảng tổng

    private InventorySlot_UI[] slots; // Mảng chứa các ô

    private void Start()
    {
        // 1. Tìm tất cả các ô con trong lưới
        slots = slotsParent.GetComponentsInChildren<InventorySlot_UI>();

        // 2. Đăng ký sự kiện: Khi túi thay đổi -> Vẽ lại
        InventoryManager.Instance.OnInventoryChanged += UpdateUI;

        // 3. Ẩn bảng đi lúc đầu
        inventoryPanel.SetActive(false);
    }

    private void UpdateUI()
    {
        // Lấy dữ liệu từ Manager
        List<InventorySlot> inventory = InventoryManager.Instance.inventory;

        // Duyệt qua tất cả các ô UI đang có
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.Count)
            {
                // Ô này có đồ -> Hiển thị
                slots[i].SetItem(inventory[i].itemData, inventory[i].quantity);
            }
            else
            {
                // Ô này trống -> Xóa
                slots[i].ClearSlot();
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Đã bấm nút TAB!"); // <--- Thêm dòng này

            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(!inventoryPanel.activeSelf);
                Debug.Log("Trạng thái bảng: " + inventoryPanel.activeSelf); // <--- Thêm dòng này
            }
            else
            {
                Debug.LogError("QUÊN GẮN INVENTORY PANEL RỒI BẠN ƠI!"); // <--- Báo lỗi nếu quên gắn
            }
        }
    }
}