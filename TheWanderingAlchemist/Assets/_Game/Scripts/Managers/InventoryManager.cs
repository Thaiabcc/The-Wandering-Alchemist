using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public event Action OnInventoryChanged;

    [Header("Setting")]
    [SerializeField] private int maxSlots = 25;

    public List<InventorySlot> inventory = new List<InventorySlot>();

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public bool AddItem(ItemData itemToAdd)
    {
        // --- [DEBUG] KIỂM TRA DỮ LIỆU ĐẦU VÀO ---
        if (itemToAdd == null)
        {
            Debug.LogError("LỖI TO: Đang cố nhặt một món đồ RỖNG (Null)!");
            return false;
        }
        Debug.Log("Đang nhận món đồ: " + itemToAdd.itemName);
        // ----------------------------------------
        // 1. Check Stack
        foreach (InventorySlot slot in inventory)
        {
            if (slot.itemData == itemToAdd && slot.quantity < itemToAdd.maxStackSize)
            {
                slot.AddQuantity(1);
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        // 2. Check emtpy (20)
        if (inventory.Count < maxSlots)
        {
            InventorySlot newSlot = new InventorySlot(itemToAdd, 1);
            inventory.Add(newSlot);
            OnInventoryChanged?.Invoke();
            return true;
        }

        Debug.Log("Túi đã đầy!");
        return false;
    }
}