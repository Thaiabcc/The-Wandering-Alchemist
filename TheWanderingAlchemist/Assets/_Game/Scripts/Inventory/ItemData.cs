using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Create item by using right click in Project
[CreateAssetMenu(fileName ="New Item Data",menuName ="Alchemist/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Display in4")]
    public string id;
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Atribute Game")]
    public ItemType typel;
    public int maxStackSize = 99;
    public int baseValue = 10;

    // For w3
    [Header("Image in Game")]
    public GameObject worldPrefeb;
}   

public enum ItemType 
{
    Ingredient, //Material
    Potion,     //medician
    Tool,
    keyItem
}
