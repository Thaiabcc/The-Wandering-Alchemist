using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Alchemist/Recipe Data")]
public class RecipeData : ScriptableObject
{
    [Header("Recipe Settings")]
    public ItemData recipeItem;

    [Header("Output")]
    public ItemData resultItem;
    public int resultCount = 1;

    [Header("Input")]
    public List<Ingredient> ingredients;

    public bool IsUnlocked()
    {
        if (SaveManager.Instance == null || resultItem == null)
            return false;

        string checkID = !string.IsNullOrEmpty(resultItem.id) ? resultItem.id : resultItem.itemName;
        return SaveManager.Instance.unlockedRecipes.Contains(checkID);
    }
}

[System.Serializable]
public class Ingredient
{
    public ItemData item;
    public int count;
}