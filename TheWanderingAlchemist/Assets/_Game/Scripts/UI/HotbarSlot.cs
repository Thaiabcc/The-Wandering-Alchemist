using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class HotbarSlot : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int slotID;
    public Image itemIcon;
    public TextMeshProUGUI quantityText;

    [HideInInspector] public ItemData assignedItem = null;

    private Transform originalIconParent;

    private void Start()
    {
        if (itemIcon != null)
        {
            itemIcon.enabled = (itemIcon.sprite != null);
        }
    }

    private void Update()
    {
        if (itemIcon != null)
        {
            itemIcon.enabled = (itemIcon.sprite != null);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObj = eventData.pointerDrag;
        if (droppedObj == null) return;

        InventorySlot_UI invSlot = droppedObj.GetComponent<InventorySlot_UI>();
        if (invSlot != null && invSlot.Item != null)
        {
            HotbarManager.Instance.PreventDuplicate(invSlot.Item);
            assignedItem = invSlot.Item;
            HotbarManager.Instance.UpdateAllSlotsUI();
            return;
        }

        HotbarSlot otherHotbarSlot = droppedObj.GetComponent<HotbarSlot>();
        if (otherHotbarSlot != null && otherHotbarSlot != this)
        {
            ItemData tempItem = this.assignedItem;
            this.assignedItem = otherHotbarSlot.assignedItem;
            otherHotbarSlot.assignedItem = tempItem;

            HotbarManager.Instance.UpdateSlotUI(this.slotID);
            HotbarManager.Instance.UpdateSlotUI(otherHotbarSlot.slotID);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (assignedItem == null || itemIcon == null) return;

        InventorySlot_UI.isDraggingItem = true;
        originalIconParent = itemIcon.transform.parent;

        // Tạo ghost thay vì di chuyển icon gốc
        CreateDragGhost();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ghostObj != null)
            ghostObj.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        InventorySlot_UI.isDraggingItem = false;

        if (ghostObj != null)
        {
            Destroy(ghostObj);
            ghostObj = null;
        }

        if (eventData.pointerEnter == null || 
            eventData.pointerEnter.GetComponentInParent<HotbarSlot>() == null)
        {
            ClearSlot();
        }

        HotbarManager.Instance.UpdateSlotUI(slotID);
    }

    private GameObject ghostObj;

    private void CreateDragGhost()
    {
        if (ghostObj != null) Destroy(ghostObj);

        ghostObj = new GameObject("DragGhost");
        ghostObj.transform.SetParent(GetComponentInParent<Canvas>().transform, false);
        ghostObj.transform.SetAsLastSibling();

        Image ghostImage = ghostObj.AddComponent<Image>();
        ghostImage.sprite = itemIcon.sprite;
        ghostImage.color = new Color(1, 1, 1, 0.85f);
        ghostImage.raycastTarget = false;

        RectTransform rt = ghostObj.GetComponent<RectTransform>();
        rt.sizeDelta = itemIcon.GetComponent<RectTransform>().sizeDelta;
    }

    public void ClearSlot()
    {
        assignedItem = null;
        if (itemIcon != null)
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
        }
        if (quantityText != null) quantityText.text = "";
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (assignedItem != null)
        {
            ItemTooltipUI.Instance.Show(assignedItem);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemTooltipUI.Instance.Hide();
    }
}