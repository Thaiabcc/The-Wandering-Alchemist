using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class AlchemyUI : MonoBehaviour
{
    public static AlchemyUI Instance { get; private set; }

    [Header("UI Components")]
    public GameObject alchemyPanel;
    public AlchemySlot inputSlot1;
    public AlchemySlot inputSlot2;
    public AlchemySlot inputSlot3; 
    public Image outputIcon;
    public TextMeshProUGUI outputAmountText;
    
    [Header("Mini-Game Timing")]
    public Slider timingSlider;
    public float sliderSpeed = 1.5f; 
    private bool isWaitingForInput = false;
    private enum CookResult { Perfect, Good, Fail }
    private CookResult currentResult;

    [Header("Audio & Feedback")]
    [SerializeField] private GameObject successTextObj;
    [SerializeField] private GameObject failTextObj; 
    [SerializeField] private GameObject norecipe;
    [SerializeField] private float feedbackDuration = 1.5f;
    [SerializeField] private AudioClip cookingSound;
    [SerializeField] private AudioClip successSound;
    [SerializeField] private AudioClip failSound;
    
    [Header("Fail Item Setup")]
    public ItemData trashItem; 
    public int trashAmountPerBatch = 1;

    [Header("Animation Settings")]
    public Animator alchemyAnimator;
    public float cookTime = 2.5f; 
    
    public List<RecipeData> allRecipes;
    private AlchemySlot currentSelectingSlot;
    private int craftTimes;
    private bool isCooking = false;
    
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (alchemyPanel != null) alchemyPanel.SetActive(false);
        if (timingSlider != null) timingSlider.gameObject.SetActive(false);
        if (successTextObj != null) successTextObj.SetActive(false);
        if (failTextObj != null) failTextObj.SetActive(false);
        if (norecipe != null) norecipe.SetActive(false);
        if (alchemyAnimator != null) alchemyAnimator.gameObject.SetActive(false);
        ResetOutput();

        if (allRecipes != null)
        {
            foreach (var rc in allRecipes)
            {
                if (rc != null && rc.resultItem != null)
                {
                    rc.isUnlocked = PlayerPrefs.GetInt("Recipe" + rc.resultItem.itemName, 0) == 1;
                }
            }
        }
    }

    public void OpenPanel()
    {
        if (alchemyPanel != null) alchemyPanel.SetActive(true);
        if (InventoryUI.Instance != null) InventoryUI.Instance.CloseInventory();
    }

    public void HidePanel()
    {
        if (alchemyPanel != null) alchemyPanel.SetActive(false);
    }

    public void CloseButtonAction()
    {
        HidePanel();
        inputSlot1.UpdateVisual(null, 0);
        inputSlot2.UpdateVisual(null, 0);
        inputSlot3.UpdateVisual(null, 0);
        ResetOutput();
        CancelSelection();
        isCooking = false;
        isWaitingForInput = false;
        if (timingSlider != null) timingSlider.gameObject.SetActive(false);
        if (alchemyAnimator != null) alchemyAnimator.gameObject.SetActive(false);
        StopAllCoroutines();
    }

    private Coroutine noRecipeCoroutine; 

    public void OnCookButtonPress()
    {
        if (isCooking)
        {
            if (isWaitingForInput) CheckTiming();
            return;
        }
        RecipeData matchedRecipe = null;
        foreach (var recipe in allRecipes)
        {
            if (TryMatchRecipe(recipe, out int times))
            {
                matchedRecipe = recipe;
                break;
            }
        }

        if (matchedRecipe != null && !matchedRecipe.isUnlocked)
        {
            Debug.LogError("Bạn chưa học công thức chế tạo món này!");
            
            // Nếu có cái Text chuyên dụng này thì mới chạy
            if (norecipe != null) 
            {
                TextMeshProUGUI t = norecipe.GetComponent<TextMeshProUGUI>();
                if (t != null) t.text = "You're not owned recipe !";
                if (noRecipeCoroutine != null) StopCoroutine(noRecipeCoroutine);
                
                noRecipeCoroutine = StartCoroutine(ShowFailNotificationTemporarily());
            }
            return; 
        }

        if (outputIcon.enabled) StartCoroutine(CookingRoutineWithMiniGame());
    }

    private IEnumerator ShowFailNotificationTemporarily()
    {
        norecipe.SetActive(true);
        yield return new WaitForSeconds(feedbackDuration);
        norecipe.SetActive(false);
        noRecipeCoroutine = null;
    }

    private void CheckTiming()
    {
        if (timingSlider == null) return;
        isWaitingForInput = false; 
        float val = timingSlider.value;

        if (val >= 0.47f && val <= 0.53f) currentResult = CookResult.Perfect;
        else if (val >= 0.25f && val <= 0.75f) currentResult = CookResult.Good;
        else currentResult = CookResult.Fail;
    }

    private IEnumerator CookingRoutineWithMiniGame()
    {
        if (timingSlider == null) yield break;

        isCooking = true;
        isWaitingForInput = true;
        currentResult = CookResult.Fail; 

        timingSlider.gameObject.SetActive(true);
        timingSlider.value = 0;

        if (alchemyAnimator != null) alchemyAnimator.gameObject.SetActive(true);
        if (AudioManager.Instance != null && cookingSound != null) 
            AudioManager.Instance.PlaySFX(cookingSound, 1f);

        float startTime = Time.time;
        float elapsed = 0;

        while (elapsed < cookTime && isWaitingForInput)
        {
            elapsed += Time.deltaTime;
            timingSlider.value = Mathf.PingPong((Time.time - startTime) * sliderSpeed, 1f);
            yield return null;
        }

        if (timingSlider != null) timingSlider.gameObject.SetActive(false);
        if (alchemyAnimator != null) alchemyAnimator.gameObject.SetActive(false);

        PerformCrafting();
    }

    private void PerformCrafting()
    {
        RecipeData validRecipe = null;
        foreach (var recipe in allRecipes)
        {
            if (TryMatchRecipe(recipe, out int times)) { validRecipe = recipe; break; }
        }

        if (validRecipe == null) { isCooking = false; return; }

        // Mặc định ban đầu
        ItemData itemToGive = validRecipe.resultItem;
        int finalAmount = validRecipe.resultCount * craftTimes;

        // Xử lý logic Mini-game
        if (currentResult == CookResult.Perfect) 
        {
            finalAmount *= 2;
        }
        else if (currentResult == CookResult.Fail) 
        {
            // Thay thế bằng rác nếu trượt
            itemToGive = trashItem;
            finalAmount = trashAmountPerBatch * craftTimes;
        }

        // Trừ nguyên liệu (luôn trừ khi đã nấu)
        List<AlchemySlot> slots = new List<AlchemySlot> { inputSlot1, inputSlot2, inputSlot3 };
        foreach (var ing in validRecipe.ingredients)
        {
            foreach (var slot in slots)
            {
                if (slot.CurrentItem == ing.item)
                {
                    InventoryManager.Instance.RemoveItem(slot.CurrentItem, ing.count * craftTimes);
                    break;
                }
            }
        }

        inputSlot1.UpdateVisual(null, 0);
        inputSlot2.UpdateVisual(null, 0);
        inputSlot3.UpdateVisual(null, 0);

        // Trao vật phẩm (Dù là thuốc hay rác)
        if (itemToGive != null)
        {
            InventoryManager.Instance.AddItem(itemToGive, finalAmount);
            
            outputIcon.sprite = itemToGive.icon;
            outputIcon.enabled = true;
            outputIcon.color = Color.white;
            outputAmountText.text = finalAmount.ToString();
            outputAmountText.gameObject.SetActive(true);
        }

        // Hiện Feedback tương ứng
        if (currentResult != CookResult.Fail)
        {
            StartCoroutine(SuccessFeedbackRoutine(currentResult == CookResult.Perfect));
        }
        else
        {
            StartCoroutine(FailFeedbackRoutine());
        }

        CancelSelection();
    }

    private IEnumerator SuccessFeedbackRoutine(bool isPerfect)
    {
        if (AudioManager.Instance != null && successSound != null) AudioManager.Instance.PlaySFX(successSound, 1f);
        if (successTextObj != null) 
        {
            TextMeshProUGUI t = successTextObj.GetComponent<TextMeshProUGUI>();
            if (t != null) t.text = isPerfect ? "HOÀN HẢO! x2" : "THÀNH CÔNG";
            successTextObj.SetActive(true);
        }
        yield return new WaitForSeconds(feedbackDuration);
        if (successTextObj != null) successTextObj.SetActive(false);
        ResetOutput();
        isCooking = false;
    }

    private IEnumerator FailFeedbackRoutine()
    {
        if (AudioManager.Instance != null && failSound != null) AudioManager.Instance.PlaySFX(failSound, 1f);
        if (failTextObj != null) failTextObj.SetActive(true);
        yield return new WaitForSeconds(feedbackDuration);
        if (failTextObj != null) failTextObj.SetActive(false);
        ResetOutput();
        isCooking = false;
    }

    private bool TryMatchRecipe(RecipeData recipe, out int times)
    {
        times = 0;
        List<AlchemySlot> allInputSlots = new List<AlchemySlot> { inputSlot1, inputSlot2, inputSlot3 };
    
        // 1. Lọc ra danh sách các ô thực sự có chứa nguyên liệu
        List<AlchemySlot> activeSlots = new List<AlchemySlot>();
        foreach (var slot in allInputSlots)
        {
            if (slot.CurrentItem != null && slot.CurrentAmount > 0)
                activeSlots.Add(slot);
        }

        // 2. SO KHỚP CHÍNH XÁC: Số lượng ô có đồ phải bằng đúng số lượng nguyên liệu trong công thức
        // Nếu nồi có 3 món mà công thức chỉ cần 2 -> Không khớp.
        if (activeSlots.Count != recipe.ingredients.Count) return false;

        int minPossible = int.MaxValue;

        // 3. Kiểm tra từng nguyên liệu trong công thức
        foreach (var ing in recipe.ingredients)
        {
            bool found = false;
            foreach (var slot in activeSlots)
            {
                if (slot.CurrentItem == ing.item)
                {
                    // Kiểm tra số lượng có đủ để nấu ít nhất 1 lần không
                    if (slot.CurrentAmount < ing.count) return false;

                    int possible = slot.CurrentAmount / ing.count;
                    if (possible < minPossible) minPossible = possible;
                
                    found = true;
                    break;
                }
            }
            // Nếu có 1 nguyên liệu trong công thức không tìm thấy trong nồi -> Fail
            if (!found) return false;
        }

        times = minPossible;
        return true;
    }

    private void ResetOutput()
    {
        if (outputIcon != null) outputIcon.enabled = false;
        if (outputAmountText != null) outputAmountText.gameObject.SetActive(false);
    }

    public void StartSelection(AlchemySlot slot)
    {
        currentSelectingSlot = slot;
        if (InventoryUI.Instance != null) InventoryUI.Instance.OpenInventoryForSelection();
    }

    public bool IsSelecting() => currentSelectingSlot != null;

    public void ReceiveItemFromInventory(ItemData item)
    {
        if (currentSelectingSlot == null) return;
        currentSelectingSlot.SetItem(item);
        currentSelectingSlot = null;
        if (InventoryUI.Instance != null) InventoryUI.Instance.CloseInventory();
        if (alchemyPanel != null) alchemyPanel.SetActive(true);
        CheckRecipe();
    }

    public void CheckRecipe()
    {
        ResetOutput();
        if (inputSlot1.CurrentItem == null && inputSlot2.CurrentItem == null && inputSlot3.CurrentItem == null) return;
        foreach (var recipe in allRecipes)
        {
            if (TryMatchRecipe(recipe, out int times))
            {
                if (!recipe.isUnlocked) continue;
                craftTimes = times;
                outputIcon.sprite = recipe.resultItem.icon;
                outputIcon.enabled = true;
                outputIcon.color = new Color(1, 1, 1, 0.5f);
                outputAmountText.text = (recipe.resultCount * times).ToString();
                outputAmountText.gameObject.SetActive(true);
                return;
            }
        }
    }
    public void CancelSelection() => currentSelectingSlot = null;
}