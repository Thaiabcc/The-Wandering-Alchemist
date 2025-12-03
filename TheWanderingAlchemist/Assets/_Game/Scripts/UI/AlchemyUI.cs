using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections; // Cần cái này để dùng bộ đếm thời gian
using System.Collections.Generic;

public class AlchemyUI : MonoBehaviour
{
    public static AlchemyUI Instance { get; private set; }

    [Header("UI Components")]
    public GameObject alchemyPanel;
    public AlchemySlot inputSlot1;
    public AlchemySlot inputSlot2;
    public Image outputIcon;
    public TextMeshProUGUI outputAmountText;

    [Header("Hiệu ứng Nấu")]
    public GameObject smokeFX; // <--- Kéo cái Smoke_FX vào đây
    public float cookTime = 1.0f; // Thời gian nấu (giây)

    [Header("Dữ liệu")]
    public List<RecipeData> allRecipes;

    private AlchemySlot currentSelectingSlot;
    private int craftTimes = 0;

    private void Awake() { Instance = this; }

    private void Start()
    {
        alchemyPanel.SetActive(false);
        outputAmountText.gameObject.SetActive(false);
        if (smokeFX != null) smokeFX.SetActive(false); // Đảm bảo khói tắt lúc đầu
    }

    // --- CÁC HÀM CŨ (Giữ nguyên logic) ---
    public void HidePanel() { alchemyPanel.SetActive(false); }

    public void CloseButtonAction()
    {
        alchemyPanel.SetActive(false);
        inputSlot1.UpdateVisual(null, 0);
        inputSlot2.UpdateVisual(null, 0);
        outputIcon.enabled = false;
        outputIcon.sprite = null;
        outputAmountText.gameObject.SetActive(false);
        if (smokeFX != null) smokeFX.SetActive(false);
        CancelSelection();
    }

    public void StartSelection(AlchemySlot slot) { currentSelectingSlot = slot; if (InventoryUI.Instance) InventoryUI.Instance.OpenInventoryForSelection(); }
    public bool IsSelecting() { return currentSelectingSlot != null; }
    public void CancelSelection() { currentSelectingSlot = null; }

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

    public void CheckRecipe()
    {
        outputIcon.enabled = false;
        outputIcon.sprite = null;
        outputAmountText.gameObject.SetActive(false);
        craftTimes = 0;

        if (inputSlot1.currentItem == null || inputSlot2.currentItem == null) return;

        foreach (var recipe in allRecipes)
        {
            int req1 = 0; int req2 = 0;
            bool match1 = false; bool match2 = false;

            foreach (var ing in recipe.ingredients)
            {
                if (ing.item == inputSlot1.currentItem) { req1 = ing.count; match1 = true; }
                if (ing.item == inputSlot2.currentItem) { req2 = ing.count; match2 = true; }
            }

            if (match1 && match2)
            {
                int times1 = inputSlot1.currentAmount / req1;
                int times2 = inputSlot2.currentAmount / req2;
                craftTimes = Mathf.Min(times1, times2);

                if (craftTimes > 0)
                {
                    outputIcon.sprite = recipe.resultItem.icon;
                    outputIcon.enabled = true;
                    outputIcon.color = Color.white;
                    int totalResult = recipe.resultCount * craftTimes;
                    outputAmountText.text = totalResult.ToString();
                    outputAmountText.gameObject.SetActive(true);
                }
                return;
            }
        }
    }

    // --- HÀM NẤU MỚI (ĐƠN GIẢN HÓA) ---
    // Gắn cái này vào nút COOK nhé
    public void OnCookButtonPress()
    {
        // Nếu chưa có kết quả hiện lên thì không cho nấu
        if (!outputIcon.enabled) return;

        // Bắt đầu quy trình (Chạy cái đồng hồ đếm ngược)
        StartCoroutine(CookingRoutine());
    }

    // Đây là cái "Đồng hồ đếm ngược"
    IEnumerator CookingRoutine()
    {
        // 1. Bật Khói lên
        if (smokeFX != null) smokeFX.SetActive(true);

        // 2. Tạm ẩn cái hình thuốc đi (để tạo bất ngờ)
        outputIcon.enabled = false;
        outputAmountText.gameObject.SetActive(false);

        // 3. Chờ 1 giây (hoặc số giây bạn chỉnh trong Inspector)
        yield return new WaitForSeconds(cookTime);

        // 4. Tắt Khói
        if (smokeFX != null) smokeFX.SetActive(false);

        // 5. Thực hiện Logic Trừ đồ + Thêm thuốc
        PerformCrafting();
    }

    private void PerformCrafting()
    {
        RecipeData validRecipe = null;
        // Tìm lại công thức lần cuối để chắc ăn
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
            int remove1 = 0; int remove2 = 0;
            foreach (var ing in validRecipe.ingredients)
            {
                if (ing.item == inputSlot1.currentItem) remove1 = ing.count;
                if (ing.item == inputSlot2.currentItem) remove2 = ing.count;
            }

            // Trừ đồ
            InventoryManager.Instance.RemoveItem(inputSlot1.currentItem, remove1 * craftTimes);
            InventoryManager.Instance.RemoveItem(inputSlot2.currentItem, remove2 * craftTimes);

            // Thêm thuốc
            InventoryManager.Instance.AddItem(validRecipe.resultItem, validRecipe.resultCount * craftTimes);

            Debug.Log("Chế tạo thành công!");

            // Đóng bảng
            CloseButtonAction();
        }
    }
}