using UnityEngine;
using UnityEngine.UI;
using TMPro; // Nhớ thêm dòng này
using System.Collections.Generic;

public class AlchemyUI : MonoBehaviour
{
    public static AlchemyUI Instance { get; private set; }

    [Header("UI Components")]
    public GameObject alchemyPanel;
    public AlchemySlot inputSlot1;
    public AlchemySlot inputSlot2;
    public Image outputIcon;
    public TextMeshProUGUI outputAmountText; // <--- BIẾN MỚI

    [Header("Dữ liệu")]
    public List<RecipeData> allRecipes;

    private AlchemySlot currentSelectingSlot;
    private int craftTimes = 0; // Biến lưu số lần nấu được

    private void Awake() { Instance = this; }

    private void Start()
    {
        alchemyPanel.SetActive(false);
        outputAmountText.gameObject.SetActive(false); // Ẩn số lúc đầu
    }

    public void HidePanel() { alchemyPanel.SetActive(false); }

    public void CloseButtonAction()
    {
        alchemyPanel.SetActive(false);
        inputSlot1.UpdateVisual(null, 0);
        inputSlot2.UpdateVisual(null, 0);

        outputIcon.enabled = false;
        outputIcon.sprite = null;
        outputAmountText.gameObject.SetActive(false); // Reset số

        CancelSelection();
    }

    // ... (Giữ nguyên các hàm StartSelection, IsSelecting, CancelSelection, ReceiveItemFromInventory) ...
    public void StartSelection(AlchemySlot slot) { currentSelectingSlot = slot; if (InventoryUI.Instance) InventoryUI.Instance.OpenInventoryForSelection(); }
    public bool IsSelecting() { return currentSelectingSlot != null; }
    public void ReceiveItemFromInventory(ItemData item)
    {
        if (currentSelectingSlot != null)
        {
            if (currentSelectingSlot.currentItem != item)
                currentSelectingSlot.UpdateVisual(item, 1);
            else
            {
                int newAmount = currentSelectingSlot.currentAmount + 1;
                if (InventoryManager.Instance.HasItem(item, newAmount))
                    currentSelectingSlot.UpdateVisual(item, newAmount);
            }
            currentSelectingSlot = null;
            if (InventoryUI.Instance) InventoryUI.Instance.CloseInventory();
            alchemyPanel.SetActive(true);
            CheckRecipe();
        }
    }

    // --- LOGIC MỚI: TÍNH TOÁN SỐ LƯỢNG ---

    public void CheckRecipe()
    {
        outputIcon.enabled = false;
        outputIcon.sprite = null;
        outputAmountText.gameObject.SetActive(false);
        craftTimes = 0;

        if (inputSlot1.currentItem == null || inputSlot2.currentItem == null) return;

        foreach (var recipe in allRecipes)
        {
            // Tìm yêu cầu của từng món trong công thức
            int req1 = 0;
            int req2 = 0;
            bool match1 = false;
            bool match2 = false;

            foreach (var ing in recipe.ingredients)
            {
                if (ing.item == inputSlot1.currentItem) { req1 = ing.count; match1 = true; }
                if (ing.item == inputSlot2.currentItem) { req2 = ing.count; match2 = true; }
            }

            // Nếu đúng loại nguyên liệu
            if (match1 && match2)
            {
                // Tính xem nấu được bao nhiêu lần?
                // Ví dụ: Có 5 Nấm (cần 2) -> 5/2 = 2 lần. Có 2 Cỏ (cần 1) -> 2/1 = 2 lần.
                // Lấy số nhỏ nhất (Min)
                int times1 = inputSlot1.currentAmount / req1;
                int times2 = inputSlot2.currentAmount / req2;
                craftTimes = Mathf.Min(times1, times2);

                if (craftTimes > 0)
                {
                    // Hiện hình ảnh
                    outputIcon.sprite = recipe.resultItem.icon;
                    outputIcon.enabled = true;
                    outputIcon.color = Color.white;

                    // Hiện số lượng kết quả
                    int totalResult = recipe.resultCount * craftTimes;
                    outputAmountText.text = totalResult.ToString();
                    outputAmountText.gameObject.SetActive(true);
                }
                return;
            }
        }
    }

    public void OnCookButtonPress()
    {
        if (!outputIcon.enabled || craftTimes <= 0) return;

        RecipeData validRecipe = null;

        // Tìm lại công thức (Logic cũ)
        foreach (var recipe in allRecipes)
        {
            bool m1 = false, m2 = false;
            foreach (var ing in recipe.ingredients)
            {
                if (ing.item == inputSlot1.currentItem) m1 = true;
                if (ing.item == inputSlot2.currentItem) m2 = true;
            }
            if (m1 && m2) { validRecipe = recipe; break; }
        }

        if (validRecipe != null)
        {
            // Trừ nguyên liệu nhân với số lần nấu
            int remove1 = 0;
            int remove2 = 0;
            foreach (var ing in validRecipe.ingredients)
            {
                if (ing.item == inputSlot1.currentItem) remove1 = ing.count;
                if (ing.item == inputSlot2.currentItem) remove2 = ing.count;
            }

            InventoryManager.Instance.RemoveItem(inputSlot1.currentItem, remove1 * craftTimes);
            InventoryManager.Instance.RemoveItem(inputSlot2.currentItem, remove2 * craftTimes);

            // Thêm thuốc nhân với số lần nấu
            InventoryManager.Instance.AddItem(validRecipe.resultItem, validRecipe.resultCount * craftTimes);

            Debug.Log($"<color=green>Đã chế tạo {validRecipe.resultCount * craftTimes} {validRecipe.resultItem.itemName}</color>");
            CloseButtonAction();
        }
    }
    public void CancelSelection()
    {
        currentSelectingSlot = null;
    }
}