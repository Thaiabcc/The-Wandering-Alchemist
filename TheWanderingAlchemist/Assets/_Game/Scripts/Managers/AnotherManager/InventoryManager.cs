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

    public void UpdateGold(int amount)
    {
        currentGold = Mathf.Max(0, currentGold + amount);
        OnInventoryChanged?.Invoke();
    }

    public void SortInventory()
    {
        inventory.RemoveAll(slot => slot.item == null || slot.quantity <= 0);

        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].item == null) continue;
            for (int j = inventory.Count - 1; j > i; j--)
            {
                if (inventory[i].item == inventory[j].item)
                {
                    int spaceLeft = inventory[i].item.maxStackSize - inventory[i].quantity;
                    if (spaceLeft > 0)
                    {
                        int amountToMove = Mathf.Min(spaceLeft, inventory[j].quantity);
                        inventory[i].quantity += amountToMove;
                        inventory[j].quantity -= amountToMove;
                    }
                }
            }
        }

        inventory.RemoveAll(slot => slot.quantity <= 0);

        inventory.Sort((slotA, slotB) =>
        {
            int typeCompare = slotA.item.itemType.CompareTo(slotB.item.itemType);
            if (typeCompare != 0)
                return typeCompare;
            return slotA.item.itemName.CompareTo(slotB.item.itemName);
        });

        OnInventoryChanged?.Invoke();
    }

    public bool AddItem(ItemData item, int amount = 1)
    {
        if (item == null || amount <= 0)
            return false;

        int remaining = amount;
        
        foreach (var slot in inventory)
        {
            if (slot.item != item) continue;
            if (slot.quantity >= item.maxStackSize) continue;

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

        while (remaining > 0 && inventory.Count < maxSlots)
        {
            int addAmount = Mathf.Min(item.maxStackSize, remaining);
            inventory.Add(new InventorySlot(item, addAmount));
            remaining -= addAmount;
        }

        OnInventoryChanged?.Invoke();
        return remaining <= 0;
    }

    public bool HasItem(ItemData item, int requiredAmount)
    {
        if (item == null || requiredAmount <= 0) return false;
        int total = 0;
        foreach (var slot in inventory)
        {
            if (slot.item == item) total += slot.quantity;
        }
        return total >= requiredAmount;
    }

    public void RemoveItem(ItemData item, int amount)
    {
        if (item == null || amount <= 0) return;

        int remaining = amount;

        for (int i = inventory.Count - 1; i >= 0; i--)
        {
            if (inventory[i].item != item) continue;

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

            if (remaining <= 0) break;
        }

        OnInventoryChanged?.Invoke();
    }

    public int GetItemAmount(ItemData item)
    {
        if (item == null) return 0;
        int total = 0;
        foreach (var slot in inventory)
        {
            if (slot.item == item) total += slot.quantity;
        }
        return total;
    }

    public void SplitItem(int sourceIdx, int targetIdx, int amount)
    {
        if (sourceIdx < 0 || sourceIdx >= inventory.Count) return;
        
        InventorySlot sourceSlot = inventory[sourceIdx];
        if (sourceSlot == null || sourceSlot.item == null || sourceSlot.quantity <= amount) return;

        ItemData splitItem = sourceSlot.item;
        sourceSlot.quantity -= amount;

        if (targetIdx >= 0 && targetIdx < maxSlots)
        {
            while (inventory.Count <= targetIdx)
            {
                inventory.Add(new InventorySlot(null, 0));
            }

            InventorySlot targetSlot = inventory[targetIdx];
            
            if (targetSlot.item == null)
            {
                targetSlot.item = splitItem;
                targetSlot.quantity = amount;
            }
            else if (targetSlot.item == splitItem)
            {
                targetSlot.quantity += amount;
            }
            else
            {
                AddItem(splitItem, amount);
            }
        }
        else
        {
            AddItem(splitItem, amount);
        }

        inventory.RemoveAll(slot => slot.item != null && slot.quantity <= 0);
        OnInventoryChanged?.Invoke();
    }
}