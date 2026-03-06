using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopSlot_UI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;           
    [SerializeField] private TextMeshProUGUI nameText;  
    [SerializeField] private TextMeshProUGUI priceText; 
    [SerializeField] private Button buyButton;         

    private ItemData currentItem;
    public void SetShopItem(ItemData item)
    {
        currentItem = item;

        if (item != null)
        {
            if (iconImage != null)
            {
                if (item.icon != null)
                {
                    iconImage.sprite = item.icon;
                    iconImage.enabled = true; 
                    iconImage.preserveAspect = true;
                }
                else
                {
                    iconImage.enabled = false;
                }
            }
            if (nameText != null) nameText.text = item.itemName;
            if (priceText != null) priceText.text = $"{item.baseValue} G";
            if (buyButton != null)
            {
                buyButton.onClick.RemoveAllListeners();
                buyButton.onClick.AddListener(OnBuyClick);
            }
            else
            {
                Debug.LogWarning($"Slot {item.itemName} chưa được gán Button trong Prefab!");
            }
        }
    }
    private void OnBuyClick()
    {
        if (currentItem != null && ShopUI.Instance != null)
        {
            ShopUI.Instance.TryBuyItem(currentItem);
        }
    }
}