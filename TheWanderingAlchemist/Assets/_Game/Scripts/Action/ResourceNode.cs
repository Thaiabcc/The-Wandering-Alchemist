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
                return;
            }
        }
        HitNode();
    }

    private void HitNode()
    {
        currentDamage++;
        if (currentDamage >= health)
        {
            HarvestResource();
        }
    }

    private void HarvestResource()
    {
        if (itemToDrop != null)
        {
            bool added = InventoryManager.Instance.AddItem(itemToDrop, dropCount);

            if (!added)
            {
                return;
            }
        }
        Destroy(gameObject); 
    }
}