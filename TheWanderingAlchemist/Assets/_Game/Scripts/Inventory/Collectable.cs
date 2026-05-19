using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class Collectable : MonoBehaviour, IInteractable
{
    [Header("World State")]
    public bool isPermanent = true;
    public string uniqueID;
    public WorldState worldState;

    [Header("Items Data")]
    public ItemData itemData;

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

        if (isPermanent && worldState != null && worldState.IsCollected(uniqueID))
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
        if (isPickedUp || itemData == null || InventoryManager.Instance == null) return;

        bool added = InventoryManager.Instance.AddItem(itemData, 1);

        if (added)
        {
            isPickedUp = true;

            if (isPermanent && worldState != null)
            {
                worldState.RecordPickup(uniqueID);
            }

            if (HotbarManager.Instance != null)
                HotbarManager.Instance.UpdateAllSlotsUI();

            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.UpdateGatherProgress();
            }

            DisableAndDestroy();

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(AudioManager.Instance.pickupItems, 1f, true);
        }
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