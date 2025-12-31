using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AlchemySlot : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image iconDisplay;
    [SerializeField] private TextMeshProUGUI amountText;

    [Header("Data")]
    [SerializeField] private ItemData currentItem;
    [SerializeField] private int currentAmount;

    public ItemData CurrentItem => currentItem;
    public int CurrentAmount => currentAmount;

    // ==============================
    // Visual Update
    // ==============================
    public void UpdateVisual(ItemData item, int amount)
    {
        if (item == null || amount <= 0)
        {
            Clear();
            return;
        }

        currentItem = item;
        currentAmount = amount;

        iconDisplay.sprite = item.icon;
        iconDisplay.enabled = true;
        iconDisplay.color = Color.white;

        UpdateAmountText(amount);
    }

    // ==============================
    // Clear
    // ==============================
    private void Clear()
    {
        currentItem = null;
        currentAmount = 0;

        iconDisplay.sprite = null;
        iconDisplay.enabled = false;

        if (amountText != null)
            amountText.gameObject.SetActive(false);
    }

    // ==============================
    // Amount Text
    // ==============================
    private void UpdateAmountText(int amount)
    {
        if (amountText == null)
            return;

        amountText.text = amount.ToString();
        amountText.gameObject.SetActive(true);
    }

    // ==============================
    // Input
    // ==============================
    public void OnSlotClicked()
    {
        if (AlchemyUI.Instance == null)
            return;

        AlchemyUI.Instance.StartSelection(this);
    }
}
