using System;
using UnityEngine;

[Serializable]
public class InventorySlot
{
    public ItemData itemData; 
    public int quantity;      

    public InventorySlot(ItemData item, int amount)
    {
        itemData = item;
        quantity = amount;
    }

    public void AddQuantity(int amount)
    {
        quantity += amount;
    }
}