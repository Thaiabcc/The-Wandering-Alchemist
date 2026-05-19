using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorDungeon : MonoBehaviour
{
    [Header("Yêu Cầu Vật Phẩm")]
    [SerializeField] private ItemData requiredKey;

    [Tooltip("Dùng xong có mất chìa khóa không?")]
    [SerializeField] private bool consumeKey = true;

    [Header("Cài Đặt Cửa")]
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D physicsCollider;
    [SerializeField] private Collider2D triggerCollider;

    private bool isOpen = false;

    public void Interact()
    {
        if (isOpen) return;
        if (InventoryManager.Instance != null)
        {
            if (InventoryManager.Instance.HasItem(requiredKey, 1))
            {
                OpenDoor();
                if (consumeKey)
                {
                    InventoryManager.Instance.RemoveItem(requiredKey, 1);
                    Debug.Log("Đã dùng chìa khóa!");
                }
            }
            else
            {
                Debug.Log("Không có chìa khóa! Cần tìm: " + requiredKey.itemName);
            }
        }
    }

    private void OpenDoor()
    {
        isOpen = true;
        Debug.Log("Cửa đã mở!");

        if (animator != null)
        {
            animator.SetTrigger("Open");
        }

        if (physicsCollider != null)
        {
            physicsCollider.enabled = false;
        }

        if (triggerCollider != null)
        {
            triggerCollider.enabled = false;
        }

        if (AudioManager.Instance != null)
        {
            // AudioManager.Instance.PlaySFX(AudioManager.Instance.doorOpen);
        }
    }
}
