using UnityEngine;

public class ResourceNode : MonoBehaviour, IInteractable
{
    [Header("Cấu hình")]
    [SerializeField] private ItemData toolRequired; // Cần dụng cụ gì? (Rìu/Cúp)
    [SerializeField] private ItemData itemToDrop;   // Rớt ra cái gì? (Gỗ/Đá)
    [SerializeField] private int dropCount = 1;     // Rớt bao nhiêu cái?
    [SerializeField] private int health = 3;        // Chặt mấy phát thì đứt? (Tạm thời để 1 phát cho nhanh)

    // Biến đếm số lần đã chặt
    private int currentDamage = 0;

    public void Interact()
    {
        // 1. Kiểm tra xem có dụng cụ trong túi không?
        if (toolRequired != null)
        {
            // Kiểm tra trong túi có Rìu/Cúp không?
            if (!InventoryManager.Instance.HasItem(toolRequired, 1))
            {
                Debug.Log($"<color=red>Cần có {toolRequired.itemName} để khai thác!</color>");
                // Sau này có thể hiện UI thông báo lên màn hình ở đây
                return;
            }
        }

        // 2. Nếu có đồ (hoặc không yêu cầu đồ) -> Thực hiện khai thác
        HitNode();
    }

    private void HitNode()
    {
        currentDamage++;
        Debug.Log("Cốp! (Đang chặt...)");

        // Có thể thêm hiệu ứng rung cây hoặc âm thanh ở đây

        // 3. Kiểm tra đủ damge chưa
        if (currentDamage >= health)
        {
            HarvestResource();
        }
    }

    private void HarvestResource()
    {
        Debug.Log($"<color=green>Đã thu hoạch: {itemToDrop.itemName}</color>");

        // 4. Thêm nguyên liệu vào túi
        if (itemToDrop != null)
        {
            bool added = InventoryManager.Instance.AddItem(itemToDrop, dropCount);

            if (!added)
            {
                Debug.Log("Túi đầy rồi, không nhặt được gỗ!");
                return; // Túi đầy thì không phá cây, giữ cây lại
            }
        }

        // 5. Hủy cái cây đi (Biến mất)
        Destroy(gameObject);
    }
}