using UnityEngine;

public class BossDoor : MonoBehaviour, IInteractable
{
    [Header("Yêu Cầu Vật Phẩm")]
    [Tooltip("Kéo cái File Item (ScriptableObject) của chìa khóa vào đây")]
    [SerializeField] private ItemData requiredKey;

    [Tooltip("Dùng xong có mất chìa khóa không?")]
    [SerializeField] private bool consumeKey = true;

    [Header("Cài Đặt Cửa")]
    [SerializeField] private Animator animator;          // Kéo Animator của cửa vào
    [SerializeField] private Collider2D physicsCollider; // Kéo cái BoxCollider chặn đường vào
    [SerializeField] private Collider2D triggerCollider; // Kéo cái BoxCollider vùng bấm E vào

    private bool isOpen = false;

    public void Interact()
    {
        // 1. Nếu mở rồi thì thôi
        if (isOpen) return;

        // 2. Kiểm tra Inventory
        if (InventoryManager.Instance != null)
        {
            // --- SỬ DỤNG HÀM CÓ SẴN CỦA BRO ---
            // Kiểm tra xem có 1 cái chìa khóa requiredKey không
            if (InventoryManager.Instance.HasItem(requiredKey, 1))
            {
                OpenDoor();

                // Nếu cài đặt là mất chìa khóa thì xóa đi
                if (consumeKey)
                {
                    InventoryManager.Instance.RemoveItem(requiredKey, 1);
                    Debug.Log("Đã dùng chìa khóa!");
                }
            }
            else
            {
                Debug.Log("Không có chìa khóa! Cần tìm: " + requiredKey.itemName);
                // Bro có thể gọi UI thông báo ở đây
            }
        }
    }

    private void OpenDoor()
    {
        isOpen = true;
        Debug.Log("Cửa đã mở!");

        // 1. Chạy Animation mở
        if (animator != null)
        {
            animator.SetTrigger("Open");
        }

        // 2. Tắt tường chặn đường -> Đi qua được
        if (physicsCollider != null)
        {
            physicsCollider.enabled = false;
        }

        // 3. Tắt vùng bấm E -> Không bấm lại được nữa
        if (triggerCollider != null)
        {
            triggerCollider.enabled = false;
        }

        // 4. Âm thanh (nếu có)
        if (AudioManager.Instance != null)
        {
            // AudioManager.Instance.PlaySFX(AudioManager.Instance.doorOpen);
        }
    }
}