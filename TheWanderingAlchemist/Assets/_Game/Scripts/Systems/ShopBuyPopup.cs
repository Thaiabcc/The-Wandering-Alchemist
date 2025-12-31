using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopBuyPopup : MonoBehaviour
{
    [Header("UI Reference")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI amountText;
    public TextMeshProUGUI totalPriceText;
    public GameObject popupPanel;

    private ItemData currentItem;
    private int quantity = 1;

    private void Start()
    {
        popupPanel.SetActive(false);
    }
    public void OpenPopup(ItemData item)
    {
        currentItem = item;
        quantity = 1;
        UpdateUI();
        popupPanel.SetActive(true);
    }
    public void ClosePopup()
    {
        popupPanel.SetActive(false);
    }
    public void IncreaseAmount()
    {
        quantity++;
        if (quantity > 99) quantity = 99;
        UpdateUI();
    }    
    public void DecreaseAmount()
    {
        quantity--;
        if (quantity < 1) quantity = 1;
        UpdateUI();
    }
    private void UpdateUI()
    {
        if (currentItem == null) return;

        iconImage.sprite = currentItem.icon;
        nameText.text = currentItem.name;
        priceText.text = $"Giá thành : {currentItem.baseValue} G";
        amountText.text = quantity.ToString();

        int totalCost = currentItem.baseValue * quantity;
        totalPriceText.text = $"Total : {totalCost} G";

        if(InventoryManager.Instance.currentGold < totalCost)
        {
            totalPriceText.color = Color.red;
        }
        else
        {
            totalPriceText.color = Color.white;
        }
    }
    public void OnConfirmBuy()
    {
        if(currentItem == null) return;
        ShopUI.Instance.ProcessBuying(currentItem, quantity);
        ClosePopup();
    }
    public void SetMinAmount()
    {
        quantity = 1;
        UpdateUI();
    }
    public void SetMaxAmount()
    {
        if (InventoryManager.Instance == null || currentItem == null) return;
        int currentGold = InventoryManager.Instance.currentGold;
        int price = currentItem.baseValue;
        int maxAffordable = (price > 0) ? (currentGold / price) : 9999;
        int stackLimit = currentItem.maxStackSize > 0 ? currentItem.maxStackSize : 99;
        quantity = Mathf.Min(maxAffordable, stackLimit);
        quantity = Mathf.Max(quantity, 1);
        UpdateUI();
    }
}
