using UnityEngine;
using System.Collections.Generic;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance { get; private set; }

    [Header("UI")]
    public GameObject shopPanel;
    public Transform shopGrid; // Nơi chứa các slot
    public GameObject shopSlotPrefab; // Prefab ô hàng

    [Header("Dữ liệu Hàng Hóa (Test)")]
    public List<ItemData> itemsForSale; // Kéo Nấm, Thuốc... vào đây để bán

    private void Awake() { Instance = this; }

    private void Start()
    {
        shopPanel.SetActive(false);
    }

    // Hàm mở Shop (Sẽ được gọi bởi NPC)
    public void OpenShop()
    {
        shopPanel.SetActive(true);
        // Tắt túi đồ cá nhân đi cho đỡ vướng (hoặc bật lên để so sánh tùy bạn)
        // Refresh danh sách hàng
        LoadShopItems();
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
    }

    private void LoadShopItems()
    {
        // 1. Xóa sạch các ô cũ (để tránh bị nhân đôi)
        foreach (Transform child in shopGrid)
        {
            Destroy(child.gameObject);
        }

        // 2. Tạo ô mới theo danh sách hàng
        foreach (ItemData item in itemsForSale)
        {
            GameObject newSlot = Instantiate(shopSlotPrefab, shopGrid);
            ShopSlot_UI script = newSlot.GetComponent<ShopSlot_UI>();
            script.SetShopItem(item);
        }
    }

    // --- LOGIC MUA HÀNG ---
    public void TryBuyItem(ItemData item)
    {
        // 1. Kiểm tra tiền
        if (InventoryManager.Instance.currentGold >= item.baseValue)
        {
            // 2. Thử thêm vào túi (Kiểm tra xem túi đầy không)
            bool added = InventoryManager.Instance.AddItem(item);

            if (added)
            {
                // 3. Trừ tiền
                InventoryManager.Instance.UpdateGold(-item.baseValue);
                Debug.Log($"<color=green>Đã mua: {item.itemName}</color>");
            }
            else
            {
                Debug.Log("<color=red>Túi đầy rồi!</color>");
            }
        }
        else
        {
            Debug.Log("<color=red>Không đủ tiền!</color>");
        }
    }
}