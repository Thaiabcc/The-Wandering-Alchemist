using UnityEngine;

public class ShopNPC : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        if (ShopUI.Instance != null)
        {
            // Bật/Tắt shop
            ShopUI.Instance.ToggleShop();
        }
    }

    // 2. [MỚI] Hàm này tự động chạy khi Player đi ra khỏi vùng Trigger
    private void OnTriggerExit2D(Collider2D other)
    {
        // Kiểm tra đúng là Player đi ra không
        if (other.CompareTag("Player"))
        {
            // Ép tắt Shop ngay lập tức
            if (ShopUI.Instance != null)
            {
                ShopUI.Instance.CloseShop();
            }
        }
    }
}