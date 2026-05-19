using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject shopPanel;      
    [SerializeField] private Transform shopGrid;        
    [SerializeField] private GameObject shopSlotPrefab; 

    [SerializeField] private ShopBuyPopup buyPopup;     
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
        LoadShopItems();
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);

        if (buyPopup != null) buyPopup.ClosePopup();

        if (InventoryUI.Instance != null)
            InventoryUI.Instance.CloseInventory();
    }
    private void LoadShopItems()
    {
        foreach (Transform child in shopGrid)
        {
            Destroy(child.gameObject);
        }
        foreach (ItemData item in itemsForSale)
        {
            if (item == null) continue; 

            GameObject newSlot = Instantiate(shopSlotPrefab, shopGrid);
            ShopSlot_UI slotUI = newSlot.GetComponent<ShopSlot_UI>();

            if (slotUI != null)
            {
                slotUI.SetShopItem(item);
            }
        }
    }


    public void TryBuyItem(ItemData item)
    {
        if (buyPopup != null)
        {
            buyPopup.OpenPopup(item);
        }
        else
        {
            Debug.LogError("Chưa gán ShopBuyPopup vào ShopUI!");
        }
    }
    public void ProcessBuying(ItemData item, int quantity)
    {
        int totalCost = item.baseValue * quantity;

        if (InventoryManager.Instance.currentGold >= totalCost)
        {
            if (InventoryManager.Instance.AddItem(item, quantity))
            {
                InventoryManager.Instance.UpdateGold(-totalCost);
            }
            else
            {
                Debug.Log("Túi đồ đã đầy!");
            }
        }
        else
        {
            Debug.Log("Không đủ tiền!");
        }
    }
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

    public bool IsShopOpen() => shopPanel.activeSelf;
}