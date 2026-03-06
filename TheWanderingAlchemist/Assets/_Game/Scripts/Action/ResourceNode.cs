using UnityEngine;

public class ResourceNode : MonoBehaviour, IInteractable
{
    [Header("Cấu hình")]
    [SerializeField] private ItemData toolRequired; 
    [SerializeField] private ItemData itemToDrop;   
    [SerializeField] private int dropCount = 1;     
    [SerializeField] private int health = 3;        
    private int currentDamage = 0;

    public void Interact()
    {
        if (toolRequired != null)
        {
            if (!InventoryManager.Instance.HasItem(toolRequired, 1))
            {
                Debug.Log($"<color=red>Cần có {toolRequired.itemName} để khai thác!</color>");
                return;
            }
        }
        HitNode();
    }

    private void HitNode()
    {
        currentDamage++;
        Debug.Log("Cốp! (Đang chặt...)");

        // Cần thêm effect rung cây & audio

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
                return;
            }
        }
        Destroy(gameObject);
    }
}