using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AlchemySlot : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image iconDisplay;
    [SerializeField] private TextMeshProUGUI amountText;

    [Header("Quantity Controls (MỚI)")]
    [SerializeField] private GameObject quantityGroup; 
    [SerializeField] private Button btnPlus;           
    [SerializeField] private Button btnMinus;         
    [SerializeField] private Button btnRemove;        

    [Header("Data")]
    [SerializeField] private ItemData currentItem;
    [SerializeField] private int currentAmount;

    public ItemData CurrentItem => currentItem;
    public int CurrentAmount => currentAmount;

    private void Start()
    {
        if (btnPlus) btnPlus.onClick.AddListener(OnPlusClicked);
        if (btnMinus) btnMinus.onClick.AddListener(OnMinusClicked);
        if (btnRemove) btnRemove.onClick.AddListener(OnRemoveClicked);
        UpdateVisual(currentItem, currentAmount);
    }
    private void OnPlusClicked()
    {
        if (currentItem == null) return;
        int inventoryAmount = InventoryManager.Instance.GetItemAmount(currentItem);
        if (currentAmount < inventoryAmount)
        {
            currentAmount++;
            UpdateVisual(currentItem, currentAmount);
            if (AlchemyUI.Instance != null) AlchemyUI.Instance.CheckRecipe();
        }
        else
        {
            Debug.Log("Đã hết hàng trong kho!");
        }
    }

    private void OnMinusClicked()
    {
        if (currentItem == null) return;

        if (currentAmount > 1)
        {
            currentAmount--;
            UpdateVisual(currentItem, currentAmount);
            if (AlchemyUI.Instance != null) AlchemyUI.Instance.CheckRecipe();
        }
    }

    private void OnRemoveClicked()
    {
        UpdateVisual(null, 0);
        if (AlchemyUI.Instance != null) AlchemyUI.Instance.CheckRecipe();
    }
    public void UpdateVisual(ItemData item, int amount)
    {
        currentItem = item;
        currentAmount = amount;

        if (item == null || amount <= 0)
        {
            Clear();
            return;
        }
        iconDisplay.sprite = item.icon;
        iconDisplay.enabled = true;
        iconDisplay.color = Color.white;
        UpdateAmountText(amount);
        if (quantityGroup) quantityGroup.SetActive(true);
        if (btnRemove) btnRemove.gameObject.SetActive(true);
    }
    private void Clear()
    {
        currentItem = null;
        currentAmount = 0;

        iconDisplay.sprite = null;
        iconDisplay.enabled = false;

        if (amountText != null) amountText.gameObject.SetActive(false);
        if (quantityGroup) quantityGroup.SetActive(false);
        if (btnRemove) btnRemove.gameObject.SetActive(false);
    }
    private void UpdateAmountText(int amount)
    {
        if (amountText == null) return;
        amountText.text = amount.ToString();
        amountText.gameObject.SetActive(true);
    }
    public void SetItem(ItemData item)
    {
        UpdateVisual(item, 1);
    }
    public void OnSlotClicked()
    {
        if (AlchemyUI.Instance == null) return;
        AlchemyUI.Instance.StartSelection(this);
    }
}