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

        // --- ƯU TIÊN 1: BÁN HÀNG (Nếu Shop đang mở) ---
        if (ShopUI.Instance != null && ShopUI.Instance.IsShopOpen())
        {
            ShopUI.Instance.TrySellItem(currentItem);
            return; // Bán xong thì thoát luôn, không làm các việc bên dưới
        }

        // --- ƯU TIÊN 2: CHỌN NGUYÊN LIỆU (Nếu Lò Luyện đang mở) ---
        if (AlchemyUI.Instance != null && AlchemyUI.Instance.IsSelecting())
        {
            AlchemyUI.Instance.ReceiveItemFromInventory(currentItem);
            return;
        }

        // --- ƯU TIÊN 3: SỬ DỤNG / ĂN (Nếu đang đi chơi bình thường) ---
        if (currentItem.isConsumable)
        {
            // Logic uống thuốc hồi máu cũ của bạn
            if (PlayerStats.Instance.currentHealth < 100)
            {
                PlayerStats.Instance.Heal(currentItem.healthRestore);
                InventoryManager.Instance.RemoveItem(currentItem, 1);
            }
            else
            {
                Debug.Log("Máu đầy rồi, không uống nữa!");
            }
        }
    }
}