using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopSlot_UI : MonoBehaviour
{
    [Header("Tham chiếu UI")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;

    // 1. THÊM DÒNG NÀY: Để code biết cái nút nằm đâu
    public Button buyButton;

    private ItemData currentItem;

    public void SetShopItem(ItemData item)
    {
        currentItem = item;

        if (item != null)
        {
            iconImage.sprite = item.icon;
            nameText.text = item.itemName;
            priceText.text = item.baseValue.ToString() + " G";

            // 2. THÊM ĐOẠN NÀY: "Hàn dây điện" cho nút bấm
            // Xóa lệnh cũ (nếu có) để tránh lỗi bấm 1 lần mua 2 lần
            buyButton.onClick.RemoveAllListeners();

            // Bảo cái nút là: "Khi bị bấm, hãy chạy hàm OnBuyClick ngay!"
            buyButton.onClick.AddListener(OnBuyClick);
        }
    }

    // Hàm này giữ nguyên
    public void OnBuyClick()
    {
        if (currentItem != null)
        {
            ShopUI.Instance.TryBuyItem(currentItem);
        }
    }
}