using UnityEngine;

public class RecipeItem : MonoBehaviour
{
    public RecipeData recipeData; // Kéo file ScriptableObject công thức tương ứng vào đây

    // Gọi hàm này khi người chơi click "Dùng" tờ giấy trong Inventory
    public void UseRecipe()
    {
        if (recipeData == null) return;

        if (recipeData.isUnlocked)
        {
            Debug.Log("Công thức này ông học rồi!");
            return;
        }

        // Mở khóa công thức
        recipeData.isUnlocked = true;
        
        // Lưu trạng thái xuống máy để không bị mất khi tắt game
        PlayerPrefs.SetInt("Recipe_" + recipeData.resultItem.itemName, 1);
        PlayerPrefs.Save();

        Debug.Log($"🎉 Đã học công thức: {recipeData.resultItem.itemName}");
        
        // Code xóa tờ giấy này khỏi Inventory của ông ở đây...
    }
}