using System;
using UnityEngine;

[Serializable]
public class InventorySlot
{
    public ItemData item;
    public int quantity;

    public InventorySlot(ItemData item, int amount)
    {
        this.item = item;
        quantity = Mathf.Clamp(amount, 0, item != null ? item.maxStackSize : 0);
    }

    public void AddQuantity(int amount)
    {
        quantity = Mathf.Max(0, quantity + amount);
    }
}
