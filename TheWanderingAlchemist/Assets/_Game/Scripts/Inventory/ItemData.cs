using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Alchemist/Item")]
public class ItemData : ScriptableObject
{
    public string id;
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;

    public ItemType itemType;
    public int maxStackSize = 99;
    public int baseValue = 10;

    public GameObject worldPrefab;

    public bool isConsumable;
    public int healthRestore;
    public float staminaRestore;
    public float damagebuffAmount;
    public float buffduration;
}

public enum ItemType
{
    Ingredient,
    Potion,
    Tool,
    KeyItem
}
