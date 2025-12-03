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

        // 1. Nếu đang chế thuốc -> Chọn nguyên liệu (Logic cũ)
        if (AlchemyUI.Instance != null && AlchemyUI.Instance.IsSelecting())
        {
            AlchemyUI.Instance.ReceiveItemFromInventory(currentItem);
        }
        // 2. Nếu đang chơi bình thường -> SỬ DỤNG ĐỒ (Logic Mới)
        else
        {
            if (currentItem.isConsumable)
            {
                // Gọi PlayerStats để hồi máu
                if (PlayerStats.Instance.currentHealth < 100) // Kiểm tra sơ bộ
                {
                    PlayerStats.Instance.Heal(currentItem.healthRestore);

                    // Xóa 1 cái khỏi túi
                    InventoryManager.Instance.RemoveItem(currentItem, 1);
                }
                else
                {
                    Debug.Log("Máu đầy rồi!");
                }
            }
        }
    }
}