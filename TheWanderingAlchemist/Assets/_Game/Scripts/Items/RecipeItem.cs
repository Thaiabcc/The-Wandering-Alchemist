using UnityEngine;

public class RecipeItem : MonoBehaviour
{
    public RecipeData recipeData;

    public void UseRecipe()
    {
        if (recipeData == null || recipeData.resultItem == null)
            return;

        if (recipeData.IsUnlocked())
        {
            Debug.Log("You already have this recipe!");
            return;
        }

        SaveManager.Instance.UnlockRecipe(recipeData);

        Debug.Log($"Success: {recipeData.resultItem.itemName}");

        if (recipeData.recipeItem != null)
        {
            InventoryManager.Instance.RemoveItem(recipeData.recipeItem, 1);
        }

        if (AlchemyUI.Instance != null)
            AlchemyUI.Instance.CheckRecipe();
    }
}