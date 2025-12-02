using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("Tham chiếu")]
    [SerializeField] private Transform slotsParent; 
    [SerializeField] private GameObject inventoryPanel; 

    private InventorySlot_UI[] slots; 

    private void Start()
    {
        // 1. Searching slot in grid
        slots = slotsParent.GetComponentsInChildren<InventorySlot_UI>();

        // 2. Inven change -> draw
        InventoryManager.Instance.OnInventoryChanged += UpdateUI;

        // 3. hide 
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
            Debug.Log("Đã bấm nút TAB!"); 

            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(!inventoryPanel.activeSelf);
                Debug.Log("Trạng thái bảng: " + inventoryPanel.activeSelf); 
            }
            else
            {
                Debug.LogError("QUÊN GẮN INVENTORY PANEL"); 
            }
        }
    }
}