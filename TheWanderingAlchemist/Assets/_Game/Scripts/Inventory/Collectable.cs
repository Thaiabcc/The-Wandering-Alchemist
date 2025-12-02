using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]   

public class Collectable : MonoBehaviour, IInteractable
{
    [Header("Items Data")]
    public ItemData itemData; // pull file so

    private SpriteRenderer spriteRenderer;

    private void OnValidate()
    {
        // Run immediately when change value
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisual();
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        if(itemData != null)
        {
            spriteRenderer.sprite = itemData.icon;
            gameObject.name = "Item_" + itemData.itemName;
        }    
    }  
    
    public void Interact()
    {
        if (InventoryManager.Instance != null)
        {
            bool added = InventoryManager.Instance.AddItem(itemData);

            if (added)
            {      
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Túi đầy rồi, không nhặt được!");
            }
        }
        else
        {
            Debug.LogError("LỖI: Không tìm thấy InventoryManager trong Scene!");
        }
    }
}
