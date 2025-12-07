using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public event Action OnInventoryChanged;

    [Header("Kinh tế")]
    public int currentGold = 100; // Tiền khởi điểm

    // Hàm thay đổi tiền (Mua/Bán)
    public void UpdateGold(int amount)
    {
        currentGold += amount;

        // Không cho tiền âm
        if (currentGold < 0) currentGold = 0;

        // Báo cho UI biết để cập nhật số tiền hiển thị
        OnInventoryChanged?.Invoke();
    }

    [Header("Setting")]
    [SerializeField] private int maxSlots = 25;

    public List<InventorySlot> inventory = new List<InventorySlot>();

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public bool AddItem(ItemData itemToAdd, int amount = 1)
    {
        // 1. Kiểm tra Stack
        foreach (InventorySlot slot in inventory)
        {
            if (slot.itemData == itemToAdd && slot.quantity < itemToAdd.maxStackSize)
            {
                slot.AddQuantity(amount); // <--- Sửa số 1 thành amount
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        // 2. Kiểm tra ô trống
        if (inventory.Count < maxSlots)
        {
            InventorySlot newSlot = new InventorySlot(itemToAdd, amount); // <--- Sửa số 1 thành amount
            inventory.Add(newSlot);
            OnInventoryChanged?.Invoke();
            return true;
        }

        Debug.Log("Túi đã đầy!");
        return false;
    }

    // 1. Hàm kiểm tra xem trong túi có đủ đồ không
    public bool HasItem(ItemData itemToCheck, int amountData)
    {
        int count = 0;
        foreach (InventorySlot slot in inventory)
        {
            if (slot.itemData == itemToCheck)
            {
                count += slot.quantity;
            }
        }
        return count >= amountData;
    }

    // 2. Hàm xóa đồ (Trừ đi số lượng)
    public void RemoveItem(ItemData itemToRemove, int amountToRemove)
    {
        int amountLeft = amountToRemove;

        // Duyệt ngược từ cuối lên đầu để xóa an toàn
        for (int i = inventory.Count - 1; i >= 0; i--)
        {
            if (inventory[i].itemData == itemToRemove)
            {
                if (inventory[i].quantity > amountLeft)
                {
                    // Ô này đủ đồ để trừ
                    inventory[i].quantity -= amountLeft;
                    amountLeft = 0;
                }
                else
                {
                    // Ô này ít hơn hoặc bằng số cần trừ -> Xóa luôn ô này
                    amountLeft -= inventory[i].quantity;
                    inventory.RemoveAt(i);
                }

                if (amountLeft <= 0) break; // Đã trừ xong
            }
        }

        // Cập nhật lại UI
        OnInventoryChanged?.Invoke();
    }
}