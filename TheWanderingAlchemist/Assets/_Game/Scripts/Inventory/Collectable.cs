using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class Collectable : MonoBehaviour, IInteractable
{
    [Header("Save Settings")]
    public bool isPermanent = true;
    public string uniqueID;

    [Header("Items Data")]
    public ItemData itemData;
    
    [Header("Audio")]
    [SerializeField] private AudioClip pickupSFX;

    private SpriteRenderer spriteRenderer;
    private bool isPickedUp = false;

    private void OnValidate()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisual();
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisual();

        if (isPermanent &&
            SaveManager.Instance != null &&
            SaveManager.Instance.collectedUniqueIDs.Contains(uniqueID))
        {
            Destroy(gameObject);
        }
    }

    public void UpdateVisual()
    {
        if (itemData != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = itemData.icon;
            gameObject.name = "Item_" + itemData.itemName;
        }
    }

    public void Interact()
    {
        if (isPickedUp || itemData == null || InventoryManager.Instance == null)
            return;

        bool added = InventoryManager.Instance.AddItem(itemData, 1);

        if (!added)
            return;

        isPickedUp = true;

        if (isPermanent &&
            SaveManager.Instance != null &&
            !SaveManager.Instance.collectedUniqueIDs.Contains(uniqueID))
        {
            SaveManager.Instance.collectedUniqueIDs.Add(uniqueID);
        }

        HotbarManager.Instance?.UpdateAllSlotsUI();
        QuestManager.Instance?.UpdateGatherProgress();
        
        if (pickupSFX != null)
        {
            AudioManager.Instance?.PlaySFX(pickupSFX, 1f, true);
        }
        DisableAndDestroy();

        
    }

    private void DisableAndDestroy()
    {
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();

        foreach (var col in colliders)
            col.enabled = false;

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        Destroy(gameObject);
    }
}