using UnityEngine;

public class BossDoor : MonoBehaviour, IInteractable
{
    [Header("Item Needed")]
    [SerializeField] private ItemData requiredKey;
    [SerializeField] private bool consumeKey = true;

    [Header("Setting Gate")]
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