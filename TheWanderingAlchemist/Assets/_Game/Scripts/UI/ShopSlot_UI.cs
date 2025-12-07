using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopSlot_UI : MonoBehaviour
{
    [Header("Tham chiếu UI")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;

    private ItemData currentItem;

    // Hàm này để ShopUI gọi khi vẽ danh sách
    public void SetShopItem(ItemData item)
    {
        currentItem = item;

        if (item != null)
        {
            iconImage.sprite = item.icon;
            nameText.text = item.itemName;
            priceText.text = item.baseValue.ToString() + " G"; // Ví dụ: "10 G"
        }
    }

    // Gắn hàm này vào Nút (Button) của Slot
    public void OnBuyClick()
    {
        if (currentItem != null)
        {
            // Gọi lên quản lý Shop để mua
            ShopUI.Instance.TryBuyItem(currentItem);
        }
    }
}