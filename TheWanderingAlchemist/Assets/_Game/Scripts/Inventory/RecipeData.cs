using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Alchemist/Recipe Data")]
public class RecipeData : ScriptableObject
{
    [Header("Thành Phẩm (Đầu ra)")]
    public ItemData resultItem; 
    public int resultCount = 1; 

    [Header("Nguyên Liệu (Đầu vào)")]
    public List<Ingredient> ingredients; 
}

[System.Serializable]
public class Ingredient
{
    public ItemData item;  
    public int count;     
}