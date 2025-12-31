using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot_UI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Button button;

    private ItemData item;
    public ItemData Item => item;

    private void Awake()
    {
        AutoWireButton();
    }

    // ------------------ SETUP ------------------

    public void SetItem(ItemData newItem, int amount)
    {
        item = newItem;

        if (item == null)
        {
            Clear();
            return;
        }

        icon.sprite = item.icon;
        icon.enabled = true;

        amountText.gameObject.SetActive(amount > 1);
        amountText.text = amount.ToString();
    }

    public void Clear()
    {
        item = null;

        if (icon != null)
        {
            icon.enabled = false;
        }

        if (amountText != null)
        {
            amountText.gameObject.SetActive(false);
        }
    }
    public void ClearSlot() 
    {
        Clear();
    }

    // ------------------ CLICK ------------------

    private void OnClick()
    {
        if (item == null) return;

        if (TrySellItem()) return;
        if (TrySendToAlchemy()) return;

        TryConsumeItem();
    }

    // ------------------ ACTIONS ------------------

    private bool TrySellItem()
    {
        if (ShopUI.Instance == null || !ShopUI.Instance.IsShopOpen())
            return false;

        ShopUI.Instance.TrySellItem(item);
        return true;
    }

    private bool TrySendToAlchemy()
    {
        if (AlchemyUI.Instance == null || !AlchemyUI.Instance.IsSelecting())
            return false;

        AlchemyUI.Instance.ReceiveItemFromInventory(item);
        return true;
    }

    private void TryConsumeItem()
    {
        if (!item.isConsumable) return;
        PlayerStats player = PlayerStats.Instance;
        if (player == null) return;
        bool itemUsed = false;

        // Health
        if(item.healthRestore > 0 && player.currentHealth < player.MaxHealth)
        {
            player.Heal((int)item.healthRestore);
            itemUsed = true;
        }    

        // Stamina
        if(item.staminaRestore > 0 && player.currentStamina < player.maxStamina)
        {
            player.RegenerateStamina(item.staminaRestore);
            itemUsed = true;
        }

        // Buff Dame
        if(item.damagebuffAmount > 0)
        {
            player.ApplyBuffDamage(item.damagebuffAmount, item.buffduration);
            itemUsed = true;
        }

        // End
        if (itemUsed)
        {
            InventoryManager.Instance.RemoveItem(item, 1);
        }
        else
        {
            Debug.Log("Error....!");
        }
    }

    // ------------------ UTIL ------------------

    private void AutoWireButton()
    {
        if (!button)
            button = GetComponent<Button>() ?? GetComponentInChildren<Button>();

        if (!button) return;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }
}
