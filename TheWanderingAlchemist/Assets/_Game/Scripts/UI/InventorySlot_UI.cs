using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot_UI : MonoBehaviour
{
    [Header("Thành phần UI")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI amountText;

    private ItemData currentItem;

    public void SetItem(ItemData item, int amount)
    {
        currentItem = item;
        if (item != null)
        {
            iconImage.sprite = item.icon;
            iconImage.enabled = true; // Bật ảnh lên
            iconImage.color = Color.white;

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

    public void ClearSlot()
    {
        currentItem = null;
        iconImage.sprite = null;
        iconImage.enabled = false; // Tắt ảnh đi để không bị ô trắng
        amountText.text = "";
        amountText.gameObject.SetActive(false);
    }

    // --- HÀM MỚI: Xử lý khi bấm vào ô này ---
    // (Nhớ gắn vào Button OnClick ở Prefab)
    public void OnClick()
    {
        if (currentItem == null) return;

        // Kiểm tra xem bên Alchemy có đang chờ chọn đồ không?
        if (AlchemyUI.Instance != null && AlchemyUI.Instance.IsSelecting())
        {
            // Gửi món đồ này sang cho Alchemy
            AlchemyUI.Instance.ReceiveItemFromInventory(currentItem);
        }
        else
        {
            Debug.Log("Bấm vào: " + currentItem.itemName);
            // Sau này làm code Sử dụng/Ăn đồ ở đây
        }
    }
}