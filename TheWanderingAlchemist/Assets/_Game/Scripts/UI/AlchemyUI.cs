using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class AlchemyUI : MonoBehaviour
{
    public static AlchemyUI Instance { get; private set; }

    // ==============================
    // UI Components
    // ==============================
    [Header("UI Components")]
    public GameObject alchemyPanel;
    public AlchemySlot inputSlot1;
    public AlchemySlot inputSlot2;
    public Image outputIcon;
    public TextMeshProUGUI outputAmountText;

    // ==============================
    // [MỚI] Audio & Feedback
    // ==============================
    [Header("Audio & Feedback")]
    [SerializeField] private GameObject successTextObj;
    [SerializeField] private float feedbackDuration = 1.5f;

    // 👇 HAI BIẾN ÂM THANH RIÊNG BIỆT 👇
    [Tooltip("Kéo file tiếng sôi sùng sục vào đây")]
    [SerializeField] private AudioClip cookingSound;

    [Tooltip("Kéo file tiếng Ting thành công vào đây")]
    [SerializeField] private AudioClip successSound;

    // ==============================
    // Animation Settings
    // ==============================
    [Header("Cài đặt Animation")]
    public Animator alchemyAnimator;
    public float cookTime = 1.0f;

    // ==============================
    // Data
    // ==============================
    [Header("Dữ liệu")]
    public List<RecipeData> allRecipes;

    private AlchemySlot currentSelectingSlot;
    private int craftTimes;

    // ==============================
    // Unity Lifecycle
    // ==============================
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        alchemyPanel.SetActive(false);
        if (alchemyAnimator != null) alchemyAnimator.gameObject.SetActive(false);
        if (successTextObj != null) successTextObj.SetActive(false);

        ResetOutput();
    }

    // ==============================
    // Panel Control
    // ==============================
    public void OpenPanel()
    {
        alchemyPanel.SetActive(true);
        if (InventoryUI.Instance != null)
            InventoryUI.Instance.CloseInventory();
    }

    public void HidePanel()
    {
        alchemyPanel.SetActive(false);
    }

    public void CloseButtonAction()
    {
        HidePanel();
        inputSlot1.UpdateVisual(null, 0);
        inputSlot2.UpdateVisual(null, 0);
        ResetOutput();
        CancelSelection();

        if (successTextObj != null) successTextObj.SetActive(false);
        StopAllCoroutines();
    }

    // ==============================
    // Selection Logic
    // ==============================
    public void StartSelection(AlchemySlot slot)
    {
        currentSelectingSlot = slot;
        if (InventoryUI.Instance != null)
            InventoryUI.Instance.OpenInventoryForSelection();
    }

    public bool IsSelecting() => currentSelectingSlot != null;

    public void CancelSelection() => currentSelectingSlot = null;

    public void ReceiveItemFromInventory(ItemData item)
    {
        if (currentSelectingSlot == null || item == null) return;

        if (currentSelectingSlot.CurrentItem != item)
        {
            currentSelectingSlot.UpdateVisual(item, 1);
        }
        else
        {
            int newAmount = currentSelectingSlot.CurrentAmount + 1;
            if (InventoryManager.Instance.HasItem(item, newAmount))
                currentSelectingSlot.UpdateVisual(item, newAmount);
        }

        currentSelectingSlot = null;

        if (InventoryUI.Instance != null)
            InventoryUI.Instance.CloseInventory();

        alchemyPanel.SetActive(true);
        CheckRecipe();
    }

    // ==============================
    // Recipe Logic
    // ==============================
    public void CheckRecipe()
    {
        ResetOutput();
        craftTimes = 0;

        if (inputSlot1.CurrentItem == null || inputSlot2.CurrentItem == null)
            return;

        foreach (var recipe in allRecipes)
        {
            if (!TryMatchRecipe(recipe, out int times))
                continue;

            craftTimes = times;
            ShowOutput(recipe);
            return;
        }
    }

    private bool TryMatchRecipe(RecipeData recipe, out int times)
    {
        times = 0;
        int req1 = 0, req2 = 0;
        bool match1 = false, match2 = false;

        foreach (var ing in recipe.ingredients)
        {
            if (ing.item == inputSlot1.CurrentItem) { req1 = ing.count; match1 = true; }
            if (ing.item == inputSlot2.CurrentItem) { req2 = ing.count; match2 = true; }
        }

        if (!match1 || !match2) return false;

        int t1 = inputSlot1.CurrentAmount / req1;
        int t2 = inputSlot2.CurrentAmount / req2;

        times = Mathf.Min(t1, t2);
        return times > 0;
    }

    private void ShowOutput(RecipeData recipe)
    {
        outputIcon.sprite = recipe.resultItem.icon;
        outputIcon.enabled = true;
        outputIcon.color = new Color(1, 1, 1, 0.5f);
        int total = recipe.resultCount * craftTimes;
        outputAmountText.text = total.ToString();
        outputAmountText.gameObject.SetActive(true);
    }

    // ==============================
    // Cooking Logic
    // ==============================
    public void OnCookButtonPress()
    {
        if (!outputIcon.enabled) return;

        StartCoroutine(CookingRoutine());
    }

    private IEnumerator CookingRoutine()
    {
        // 1. BẬT hoạt ảnh
        if (alchemyAnimator != null)
        {
            alchemyAnimator.gameObject.SetActive(true);
        }

        // 👇👇👇 GIAI ĐOẠN 1: CHƠI TIẾNG SÔI (BOILING) LÚC ĐANG NẤU 👇👇👇
        if (AudioManager.Instance != null && cookingSound != null)
        {
            AudioManager.Instance.PlaySFX(cookingSound, 1f);
        }

        outputIcon.enabled = false;
        outputAmountText.gameObject.SetActive(false);

        // 2. CHỜ nấu (Thời gian sôi bằng thời gian animation)
        yield return new WaitForSeconds(cookTime);

        // 3. TẮT hoạt ảnh
        if (alchemyAnimator != null)
        {
            alchemyAnimator.gameObject.SetActive(false);
        }

        // 4. Trả đồ & Hiệu ứng thành công
        PerformCrafting();
    }

    private void PerformCrafting()
    {
        RecipeData validRecipe = null;
        foreach (var recipe in allRecipes)
        {
            bool m1 = false, m2 = false;
            foreach (var ing in recipe.ingredients)
            {
                if (ing.item == inputSlot1.CurrentItem) m1 = true;
                if (ing.item == inputSlot2.CurrentItem) m2 = true;
            }
            if (m1 && m2) { validRecipe = recipe; break; }
        }

        if (validRecipe == null) return;

        // Trừ đồ
        int remove1 = 0, remove2 = 0;
        foreach (var ing in validRecipe.ingredients)
        {
            if (ing.item == inputSlot1.CurrentItem) remove1 = ing.count;
            if (ing.item == inputSlot2.CurrentItem) remove2 = ing.count;
        }

        InventoryManager.Instance.RemoveItem(inputSlot1.CurrentItem, remove1 * craftTimes);
        InventoryManager.Instance.RemoveItem(inputSlot2.CurrentItem, remove2 * craftTimes);
        InventoryManager.Instance.AddItem(validRecipe.resultItem, validRecipe.resultCount * craftTimes);

        Debug.Log("Chế tạo thành công!");

        // --- CẬP NHẬT UI KẾT QUẢ ---
        inputSlot1.UpdateVisual(null, 0);
        inputSlot2.UpdateVisual(null, 0);

        outputIcon.sprite = validRecipe.resultItem.icon;
        outputIcon.enabled = true;
        outputIcon.color = Color.white;

        int totalCreated = validRecipe.resultCount * craftTimes;
        outputAmountText.text = totalCreated.ToString();
        outputAmountText.gameObject.SetActive(true);

        CancelSelection();

        // 3. Gọi hiệu ứng ăn mừng (Kèm tiếng Ting)
        StartCoroutine(SuccessFeedbackRoutine());
    }

    // --- HIỆU ỨNG THÀNH CÔNG ---
    private IEnumerator SuccessFeedbackRoutine()
    {
        // 👇👇👇 GIAI ĐOẠN 2: CHƠI TIẾNG TING (SUCCESS) LÚC XONG VIỆC 👇👇👇
        if (AudioManager.Instance != null && successSound != null)
        {
            AudioManager.Instance.PlaySFX(successSound, 1f);
        }

        // 2. Hiện chữ Success
        if (successTextObj != null)
        {
            successTextObj.SetActive(true);
        }

        // 3. Hiệu ứng nảy (Pop) icon kết quả
        float timer = 0;
        Vector3 originalScale = Vector3.one;
        Vector3 punchScale = Vector3.one * 1.3f;

        // Phóng to
        while (timer < 0.2f)
        {
            timer += Time.deltaTime;
            outputIcon.transform.localScale = Vector3.Lerp(originalScale, punchScale, timer / 0.2f);
            yield return null;
        }

        // Thu nhỏ
        timer = 0;
        while (timer < 0.1f)
        {
            timer += Time.deltaTime;
            outputIcon.transform.localScale = Vector3.Lerp(punchScale, originalScale, timer / 0.1f);
            yield return null;
        }
        outputIcon.transform.localScale = originalScale;

        // 4. Chờ rồi tắt chữ
        yield return new WaitForSeconds(feedbackDuration);

        if (successTextObj != null)
        {
            successTextObj.SetActive(false);
        }
    }

    private void ResetOutput()
    {
        outputIcon.enabled = false;
        outputIcon.sprite = null;
        outputAmountText.gameObject.SetActive(false);
        outputIcon.transform.localScale = Vector3.one;
    }
}