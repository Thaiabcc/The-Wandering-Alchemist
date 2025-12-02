using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot_UI : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI amountText;

    private ItemData currentItem;

    // Function called by InventoryUI
    // Hàm hiển thị đồ
    public void SetItem(ItemData item, int amount)
    {
        currentItem = item;

        if (item != null)
        {
            // 1. Gán ảnh
            iconImage.sprite = item.icon;

            // 2. BẬT hiển thị ảnh lên (Quan trọng)
            iconImage.enabled = true;

            // 3. Reset màu về trắng tinh khiết (để không bị mờ)
            iconImage.color = Color.white;

            // 4. Xử lý số lượng
            if (amount > 1)
            {
                amountText.text = amount.ToString();
                amountText.gameObject.SetActive(true);
            }
            else
            {
                amountText.gameObject.SetActive(false);
            }
        }
        else
        {
            ClearSlot();
        }
    }

    // Hàm dọn dẹp ô trống
    public void ClearSlot()
    {
        currentItem = null;

        // 1. Xóa sprite
        iconImage.sprite = null;

        // 2. TẮT hẳn hiển thị ảnh đi (Để không bị hiện ô trắng)
        iconImage.enabled = false;

        // 3. Xóa số
        amountText.text = "";
        amountText.gameObject.SetActive(false);
    }

}
