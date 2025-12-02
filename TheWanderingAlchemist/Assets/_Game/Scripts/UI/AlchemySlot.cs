using UnityEngine;
using UnityEngine.UI;
using TMPro; // Thêm thư viện TextMeshPro

public class AlchemySlot : MonoBehaviour
{
    [Header("UI")]
    public Image iconDisplay;
    public TextMeshProUGUI amountText; // <--- THÊM MỚI: Hiển thị số lượng

    [Header("Dữ liệu")]
    public ItemData currentItem;
    public int currentAmount = 0; // <--- THÊM MỚI: Đang chứa bao nhiêu?

    // Hàm cập nhật mới (Nhận cả Item và Số lượng)
    public void UpdateVisual(ItemData item, int amount)
    {
        currentItem = item;
        currentAmount = amount;

        if (item != null && amount > 0)
        {
            iconDisplay.sprite = item.icon;
            iconDisplay.enabled = true;
            iconDisplay.color = Color.white;

            // Hiển thị số lượng
            if (amountText != null)
            {
                amountText.text = amount.ToString();
                amountText.gameObject.SetActive(true);
            }
        }
        else
        {
            // Reset
            currentItem = null;
            currentAmount = 0;
            iconDisplay.sprite = null;
            iconDisplay.enabled = false;
            if (amountText != null) amountText.gameObject.SetActive(false);
        }
    }

    public void OnSlotClicked()
    {
        // Gọi lên trùm, bảo là tao muốn chọn đồ
        AlchemyUI.Instance.StartSelection(this);
    }
}