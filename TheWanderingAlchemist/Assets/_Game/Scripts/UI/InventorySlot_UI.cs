using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlot_UI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [Header("UI Setup")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI amountText;

    public static bool isDraggingItem = false;
    public int slotIndex;
    
    private ItemData item;
    public ItemData Item => item;
    
    private Transform originalIconParent;
    private int currentAmount;
    private bool isSplitting = false;
    private int splitAmount = 0;
    
    private float holdTimer = 0f;
    private float delayBeforeFastCount = 0.5f;
    private float fastCountInterval = 0.1f;
    private float fastCountTimer = 0f;

    private GameObject ghostObj;
    private TextMeshProUGUI ghostText;

    public void SetItem(ItemData newItem, int amount)
    {
        item = newItem;
        currentAmount = amount;
        
        if (item == null)
        {
            Clear();
            return;
        }

        icon.sprite = item.icon;
        icon.enabled = true;
        icon.color = Color.white;
        icon.raycastTarget = true;

        amountText.gameObject.SetActive(amount > 1);
        amountText.text = amount.ToString();
    }

    public void Clear()
    {
        item = null;
        if (icon != null) icon.enabled = false;
        if (amountText != null) amountText.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (item == null || isDraggingItem) return;

        if (Input.GetKey(KeyCode.LeftShift) && eventData.button == PointerEventData.InputButton.Left && currentAmount > 1)
        {
            isSplitting = true;
            splitAmount = 1;
            holdTimer = 0f;
            fastCountTimer = 0f;
            isDraggingItem = true;
            CreateGhostUI();
            UpdateGhostUI();
        }
    }

    private void Update()
    {
        if (isSplitting && Input.GetMouseButton(0))
        {
            if (ghostObj != null)
                ghostObj.transform.position = Input.mousePosition;

            holdTimer += Time.deltaTime;
            if (holdTimer >= delayBeforeFastCount)
            {
                fastCountTimer += Time.deltaTime;
                if (fastCountTimer >= fastCountInterval)
                {
                    if (splitAmount < currentAmount - 1)
                    {
                        splitAmount++;
                        UpdateGhostUI();
                    }
                    fastCountTimer = 0f;
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isSplitting)
        {
            isSplitting = false;
            isDraggingItem = false;

            InventorySlot_UI targetSlot = null;
            if (eventData.hovered != null)
            {
                foreach (var hovered in eventData.hovered)
                {
                    var slot = hovered.GetComponentInParent<InventorySlot_UI>();
                    if (slot != null)
                    {
                        targetSlot = slot;
                        break;
                    }
                }
            }

            if (targetSlot != null && targetSlot != this)
            {
                InventoryManager.Instance.SplitItem(this.slotIndex, targetSlot.slotIndex, splitAmount);
            }
            
            if (ghostObj != null) Destroy(ghostObj);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isSplitting || Input.GetKey(KeyCode.LeftShift)) return;
        if (item == null || isDraggingItem) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
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
            bool isRecipeItem = false;
            if (AlchemyUI.Instance != null && AlchemyUI.Instance.allRecipes != null)
            {
                foreach (var recipe in AlchemyUI.Instance.allRecipes)
                {
                    if (recipe != null && recipe.recipeItem == this.item)
                    {
                        isRecipeItem = true; 

                        if (recipe.isUnlocked)
                        {
                            Debug.Log("Công thức này ông đã học từ trước rồi bro!");
                            return; // Thoát hàm, không làm gì cả
                        }

                        recipe.isUnlocked = true;
                        PlayerPrefs.SetInt("Recipe_" + recipe.resultItem.itemName, 1);
                        PlayerPrefs.Save();

                        Debug.LogWarning($"🎉 Tuyệt vời! Đã mở khóa công thức nấu: {recipe.resultItem.itemName}");
                        InventoryManager.Instance.RemoveItem(this.item, 1);
                        return; 
                    }
                }
            }
            bool isSuccess = item.UseItem(PlayerStats.Instance);
            if (isSuccess)
            {
                InventoryManager.Instance.RemoveItem(item, 1);
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (item == null || isSplitting || Input.GetKey(KeyCode.LeftShift)) return;

        isDraggingItem = true;
        originalIconParent = icon.transform.parent;
        icon.transform.SetParent(icon.transform.root);
        icon.transform.SetAsLastSibling();
        icon.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDraggingItem && !isSplitting)
        {
            icon.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggingItem || isSplitting) return;

        isDraggingItem = false;
        icon.transform.SetParent(originalIconParent);
        icon.transform.localPosition = Vector3.zero;
        icon.raycastTarget = true;
    }

    private void CreateGhostUI()
    {
        if (ghostObj != null) Destroy(ghostObj);
        ghostObj = new GameObject("SplitGhost");
        ghostObj.transform.SetParent(GetComponentInParent<Canvas>().transform);
        ghostObj.transform.SetAsLastSibling();

        Image img = ghostObj.AddComponent<Image>();
        img.sprite = item.icon;
        img.color = new Color(1, 1, 1, 0.6f);
        img.raycastTarget = false;

        RectTransform ghostRT = ghostObj.GetComponent<RectTransform>();
        RectTransform originalRT = icon.GetComponent<RectTransform>();
        ghostRT.sizeDelta = originalRT.sizeDelta;

        GameObject textObj = new GameObject("GhostText");
        textObj.transform.SetParent(ghostObj.transform);

        ghostText = textObj.AddComponent<TextMeshProUGUI>();
        ghostText.fontSize = amountText.fontSize;
        ghostText.font = amountText.font;
        ghostText.alignment = TextAlignmentOptions.BottomRight;
        ghostText.color = Color.yellow;
        ghostText.raycastTarget = false;

        RectTransform rt = ghostText.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private void UpdateGhostUI()
    {
        if (ghostText != null) ghostText.text = splitAmount.ToString();
        
        int remaining = currentAmount - splitAmount;
        amountText.text = remaining.ToString();
        amountText.gameObject.SetActive(remaining > 0);
    }
}