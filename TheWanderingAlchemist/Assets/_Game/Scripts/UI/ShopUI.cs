using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance { get; private set; }

    [Header("UI References")]
    public GameObject shopPanel;
    public Transform shopGrid;
    public GameObject shopSlotPrefab;

    public ShopBuyPopup buyPopup;
    private Canvas canvas;

    [Header("Data")]
    public List<ItemData> itemsForSale;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        canvas = GetComponent<Canvas>();
        if (canvas == null) canvas = GetComponentInParent<Canvas>();
    }

    private void Start()
    {
        shopPanel.SetActive(false);
        SceneManager.sceneLoaded += OnSceneLoaded;

        // ⚠️ QUAN TRỌNG: Không Load đồ ở đây để tránh lag lúc vào game
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            canvas.worldCamera = Camera.main;
            canvas.planeDistance = 10;
        }
        CloseShop();
    }

    // --- LOGIC ĐÓNG MỞ (ĐÃ TỐI ƯU) ---
    public void ToggleShop()
    {
        if (shopPanel.activeSelf) CloseShop();
        else OpenShop();
    }

    public void OpenShop()
    {
        shopPanel.SetActive(true);

        if (InventoryUI.Instance != null)
            InventoryUI.Instance.OpenInventoryForSelection();

        // 🔥 TỐI ƯU HÓA Ở ĐÂY 🔥
        // Kiểm tra: Nếu trong Grid chưa có gì (childCount == 0) thì mới Load.
        // Nếu đã có đồ rồi thì không Load lại nữa -> Hết Lag ngay!
        if (shopGrid.childCount == 0)
        {
            LoadShopItems();
        }
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        if (buyPopup != null) buyPopup.ClosePopup();

        if (InventoryUI.Instance != null)
            InventoryUI.Instance.CloseInventory();
    }

    // --- LOAD ĐỒ ---
    private void LoadShopItems()
    {
        // Dọn dẹp cũ (Phòng hờ trường hợp refresh shop)
        foreach (Transform child in shopGrid) Destroy(child.gameObject);

        // Tạo mới
        foreach (ItemData item in itemsForSale)
        {
            GameObject newSlot = Instantiate(shopSlotPrefab, shopGrid);
            newSlot.GetComponent<ShopSlot_UI>().SetShopItem(item);
        }
    }

    // --- MUA BÁN ---
    public void TryBuyItem(ItemData item)
    {
        buyPopup.OpenPopup(item);
    }

    public void ProcessBuying(ItemData item, int quantity)
    {
        int totalCost = item.baseValue * quantity;

        // Check tiền
        if (InventoryManager.Instance.currentGold >= totalCost)
        {
            // Check túi đầy
            if (InventoryManager.Instance.AddItem(item, quantity))
            {
                InventoryManager.Instance.UpdateGold(-totalCost);
                // Debug.Log($"Mua thành công {quantity} cái {item.itemName}"); // Tắt Log cho mượt
            }
            else
            {
                // Debug.Log("Túi đầy!"); // Nên thay bằng thông báo UI
            }
        }
        else
        {
            // Debug.Log("Không đủ tiền!"); // Nên thay bằng thông báo UI
        }
    }

    public bool IsShopOpen() => shopPanel.activeSelf;

    public void TrySellItem(ItemData item)
    {
        if (item == null) return;
        int sellPrice = Mathf.FloorToInt(item.baseValue * 0.5f);
        if (sellPrice < 1) sellPrice = 1;

        if (InventoryManager.Instance.HasItem(item, 1))
        {
            InventoryManager.Instance.RemoveItem(item, 1);
            InventoryManager.Instance.UpdateGold(sellPrice);
        }
    }
}