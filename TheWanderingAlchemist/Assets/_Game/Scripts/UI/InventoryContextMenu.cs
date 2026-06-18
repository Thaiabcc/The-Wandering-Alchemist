using UnityEngine;
using UnityEngine.UI;

public class InventoryContextMenu : MonoBehaviour
{
    public static InventoryContextMenu Instance { get; private set; }

    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Button useButton;
    [SerializeField] private Button discardButton;

    private int targetSlotIndex = -1;
    private ItemData targetItem;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        
        if (menuPanel != null) menuPanel.SetActive(false);
        
        if (useButton != null) useButton.onClick.AddListener(OnUseClicked);
        if (discardButton != null) discardButton.onClick.AddListener(OnDiscardClicked);
    }

    private void Update()
    {
        if (menuPanel != null && menuPanel.activeSelf && Input.GetMouseButtonUp(0))
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(
                    menuPanel.GetComponent<RectTransform>(), Input.mousePosition))
            {
                Hide();
            }
        }
    }

    public void Show(ItemData item, int slotIndex, Vector3 mousePosition)
    {
        if (item == null || menuPanel == null) return;

        targetItem = item;
        targetSlotIndex = slotIndex;

        menuPanel.SetActive(true);
        transform.SetAsLastSibling();
        menuPanel.transform.position = mousePosition;
    }

    public void Hide()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
        targetItem = null;
        targetSlotIndex = -1;
    }

    private void OnUseClicked()
    {
        if (targetItem != null && targetItem.UseItem(PlayerStats.Instance))
        {
            InventoryManager.Instance.RemoveItem(targetItem, 1);
        }
        Hide();
    }

    private void OnDiscardClicked()
    {
        if (targetSlotIndex != -1 && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.RemoveItemAtSlotIndex(targetSlotIndex, 1);
        }
        Hide();
    }
}