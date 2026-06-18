using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlot_UI : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IPointerClickHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [Header("UI Setup")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI amountText;

    public static bool isDraggingItem = false;

    public int slotIndex;

    private ItemData item;
    public ItemData Item => item;

    private GameObject ghostObj;

    public void SetItem(ItemData newItem, int amount)
    {
        item = newItem;

        if (item == null)
        {
            Clear();
            return;
        }

        icon.sprite = item.icon;
        icon.enabled = true;
        icon.color = Color.white;
        icon.raycastTarget = false;

        amountText.gameObject.SetActive(amount > 1);
        amountText.text = amount.ToString();
    }

    public void Clear()
    {
        item = null;

        if (icon != null)
            icon.enabled = false;

        if (amountText != null)
        {
            amountText.text = "";
            amountText.gameObject.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (item == null || isDraggingItem)
            return;

        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (ShopUI.Instance != null && ShopUI.Instance.IsShopOpen())
        {
            ShopUI.Instance.TrySellItem(item);
            return;
        }

        if (AlchemyUI.Instance != null && AlchemyUI.Instance.IsSelecting())
        {
            AlchemyUI.Instance.ReceiveItemFromInventory(item);
            return;
        }

        if (AlchemyUI.Instance != null && AlchemyUI.Instance.allRecipes != null)
        {
            foreach (var recipe in AlchemyUI.Instance.allRecipes)
            {
                if (recipe == null || recipe.recipeItem != item)
                    continue;

                if (recipe.IsUnlocked())
                    return;

                SaveManager.Instance.UnlockRecipe(recipe);
                InventoryManager.Instance.RemoveItem(item, 1);
                return;
            }
        }

        if (InventoryContextMenu.Instance != null)
        {
            InventoryContextMenu.Instance.Show(item, slotIndex, Input.mousePosition);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (item == null)
            return;
        if (InventoryContextMenu.Instance != null) InventoryContextMenu.Instance.Hide();

        isDraggingItem = true;
        CreateDragGhost();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggingItem || ghostObj == null)
            return;

        ghostObj.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggingItem)
            return;

        isDraggingItem = false;

        if (ghostObj != null)
        {
            Destroy(ghostObj);
            ghostObj = null;
        }

        if (eventData.pointerEnter != null)
        {
            HotbarSlot hotbarSlot = eventData.pointerEnter.GetComponentInParent<HotbarSlot>();

            if (hotbarSlot != null)
            {
                HotbarManager.Instance.PreventDuplicate(item);
                hotbarSlot.assignedItem = item;
                HotbarManager.Instance.UpdateAllSlotsUI();
            }
        }
    }

    private void CreateDragGhost()
    {
        if (ghostObj != null)
            Destroy(ghostObj);

        ghostObj = new GameObject("DragGhost");
        ghostObj.transform.SetParent(GetComponentInParent<Canvas>().transform, false);
        ghostObj.transform.SetAsLastSibling();

        Image ghostImage = ghostObj.AddComponent<Image>();
        ghostImage.sprite = icon.sprite;
        ghostImage.color = new Color(1f, 1f, 1f, 0.85f);
        ghostImage.raycastTarget = false;

        RectTransform rt = ghostObj.GetComponent<RectTransform>();
        rt.sizeDelta = icon.GetComponent<RectTransform>().sizeDelta;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null && ItemTooltipUI.Instance != null)
        {
            ItemTooltipUI.Instance.Show(item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDraggingItem)
            return;

        ItemTooltipUI.Instance?.Hide();
    }

    private void OnDisable()
    {
        ItemTooltipUI.Instance?.Hide();
    }
}