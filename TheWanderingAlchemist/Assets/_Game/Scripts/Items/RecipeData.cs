using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Alchemist/Recipe Data")]
public class RecipeData : ScriptableObject
{
    [Header("Status")] 
    public bool isUnlocked = false;
    
    [Header("Recipe Settings")]
    public ItemData recipeItem;
    
    [Header("Output")]
    public ItemData resultItem; 
    public int resultCount = 1; 

    [Header("Input")]
    public List<Ingredient> ingredients; 
}

[System.Serializable]
public class Ingredient
{
    public ItemData item;  
    public int count;     
}