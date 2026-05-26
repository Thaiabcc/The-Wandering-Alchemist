using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorDungeon : MonoBehaviour
{
    [Header("Item Required")]
    [SerializeField] private ItemData requiredKey;

    [Tooltip("Key Comsume")]
    [SerializeField] private bool consumeKey = true;

    [Header("Door Settings")]
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
                }
            }
        }
    }

    private void OpenDoor()
    {
        isOpen = true;

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
