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
        Debug.Log($"Đã nhặt được : {itemData.itemName}");

        //Inventory

        //dissapear effect
        Destroy(gameObject);
    }    


}
