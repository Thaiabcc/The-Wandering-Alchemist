using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class Collectable : MonoBehaviour, IInteractable
{
    [Header("Items Data")]
    public ItemData itemData;

    private SpriteRenderer spriteRenderer;

    private void OnValidate()
    {
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
        if (itemData != null)
        {
            spriteRenderer.sprite = itemData.icon;
            gameObject.name = "Item_" + itemData.itemName;
        }
    }

    public void Interact()
    {
        if (InventoryManager.Instance != null)
        {
            // Thử thêm vào túi
            bool added = InventoryManager.Instance.AddItem(itemData, 1);

            if (added)
            {
                // 👇 [ĐÃ SỬA] Bật âm thanh nhặt đồ lên
                if (AudioManager.Instance != null)
                {
                    // Volume 1f, Random Pitch = true (để nhặt nhiều cái nghe cho sướng)
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.pickupItems, 1f, true);
                }

                // Hủy vật phẩm trên mặt đất
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Túi đầy rồi, không nhặt được!");
                // Có thể thêm âm thanh báo lỗi ở đây nếu muốn
            }
        }
        else
        {
            Debug.LogError("LỖI: Không tìm thấy InventoryManager trong Scene!");
        }
    }
}