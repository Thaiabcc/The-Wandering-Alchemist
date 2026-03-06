using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public event Action OnInventoryChanged;

    [Header("Economy")]
    public int currentGold = 1000;

    [Header("Inventory Settings")]
    [SerializeField] private int maxSlots = 25;

    public List<InventorySlot> inventory = new List<InventorySlot>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ================= GOLD =================

    public void UpdateGold(int amount)
    {
        currentGold = Mathf.Max(0, currentGold + amount);
        OnInventoryChanged?.Invoke();
    }

    // ================= ADD ITEM =================

    public bool AddItem(ItemData item, int amount = 1)
    {
        if (item == null || amount <= 0)
            return false;

        int remaining = amount;

        // 1️⃣ Try stacking
        foreach (var slot in inventory)
        {
            if (slot.item != item)
                continue;

            if (slot.quantity >= item.maxStackSize)
                continue;

            int canAdd = item.maxStackSize - slot.quantity;
            int addAmount = Mathf.Min(canAdd, remaining);

            slot.AddQuantity(addAmount);
            remaining -= addAmount;

            if (remaining <= 0)
            {
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        // 2️⃣ Add new slots if space
        while (remaining > 0 && inventory.Count < maxSlots)
        {
            int addAmount = Mathf.Min(item.maxStackSize, remaining);
            inventory.Add(new InventorySlot(item, addAmount));
            remaining -= addAmount;
        }

        OnInventoryChanged?.Invoke();

        if (remaining > 0)
        {
            Debug.Log("Inventory is full!");
            return false;
        }

        return true;
    }

    // ================= CHECK ITEM =================

    public bool HasItem(ItemData item, int requiredAmount)
    {
        if (item == null || requiredAmount <= 0)
            return false;

        int total = 0;

        foreach (var slot in inventory)
        {
            if (slot.item == item)
                total += slot.quantity;
        }

        return total >= requiredAmount;
    }

    // ================= REMOVE ITEM =================

    public void RemoveItem(ItemData item, int amount)
    {
        if (item == null || amount <= 0)
            return;

        int remaining = amount;

        for (int i = inventory.Count - 1; i >= 0; i--)
        {
            if (inventory[i].item != item)
                continue;

            if (inventory[i].quantity > remaining)
            {
                inventory[i].quantity -= remaining;
                remaining = 0;
            }
            else
            {
                remaining -= inventory[i].quantity;
                inventory.RemoveAt(i);
            }

            if (remaining <= 0)
                break;
        }

        OnInventoryChanged?.Invoke();
    }
    public int GetItemAmount(ItemData item)
    {
        if (item == null) return 0;
        int total = 0;
        foreach (var slot in inventory)
        {
            if (slot.item == item)
            {
                total += slot.quantity;
            }
        }
        return total;
    }
}
