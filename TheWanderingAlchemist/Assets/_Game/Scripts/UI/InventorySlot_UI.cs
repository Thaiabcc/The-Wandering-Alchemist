using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot_UI : MonoBehaviour
{
    [Header("Thành phần UI")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI amountText;

    // --- THÊM: Biến này để code tự tìm nút ---
    public Button slotButton;

    private ItemData currentItem;

    // --- THÊM: Hàm Awake để tự động nối dây (Auto-wiring) ---
    private void Awake()
    {
        // 1. Tự tìm nút nếu quên kéo (Tìm trên chính nó hoặc con nó)
        if (slotButton == null)
        {
            slotButton = GetComponent<Button>();
            if (slotButton == null) slotButton = GetComponentInChildren<Button>();
        }

        // 2. Tự động gắn hàm OnClick vào nút
        if (slotButton != null)
        {
            slotButton.onClick.RemoveAllListeners(); // Xóa sạch dây cũ cho chắc
            slotButton.onClick.AddListener(OnClick); // Nối dây mới vào hàm OnClick ở dưới
        }
    }

    public void SetItem(ItemData item, int amount)
    {
        currentItem = item;
        if (item != null)
        {
            iconImage.sprite = item.icon;
            iconImage.enabled = true;
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
        iconImage.enabled = false;
        amountText.text = "";
        amountText.gameObject.SetActive(false);
    }

    // --- HÀM XỬ LÝ CLICK (Đã được nối dây tự động ở Awake) ---
    public void OnClick()
    {
        if (currentItem == null) return;

        // --- ƯU TIÊN 1: BÁN HÀNG (Nếu Shop đang mở) ---
        if (ShopUI.Instance != null && ShopUI.Instance.IsShopOpen())
        {
            ShopUI.Instance.TrySellItem(currentItem);
            return;
        }

        // --- ƯU TIÊN 2: CHỌN NGUYÊN LIỆU (Nếu Lò Luyện đang mở) ---
        // (Lưu ý: Bạn cần đảm bảo AlchemyUI có hàm IsSelecting và ReceiveItemFromInventory nhé)
        if (AlchemyUI.Instance != null && AlchemyUI.Instance.IsSelecting())
        {
            AlchemyUI.Instance.ReceiveItemFromInventory(currentItem);
            return;
        }

        // --- ƯU TIÊN 3: SỬ DỤNG / ĂN ---
        if (currentItem.isConsumable)
        {
            if (PlayerStats.Instance.currentHealth < 100)
            {
                PlayerStats.Instance.Heal(currentItem.healthRestore);
                // Trừ 1 cái sau khi ăn
                InventoryManager.Instance.RemoveItem(currentItem, 1);
            }
            else
            {
                Debug.Log("Máu đầy rồi, không uống nữa!");
            }
        }
    }
}