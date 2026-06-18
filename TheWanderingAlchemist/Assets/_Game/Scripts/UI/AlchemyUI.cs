using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

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

    [Header("Recipe Book UI")]
    [SerializeField] private GameObject recipeBookPanel;
    [SerializeField] private TextMeshProUGUI recipeListText;

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
    private Coroutine noRecipeCoroutine;

    private EventTrigger outputTrigger;
    private AlchemyAudio alchemyAudio;
    private bool wasRecipeOpenedThisFrame = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        alchemyAudio = GetComponent<AlchemyAudio>();
    }

    private void Start()
    {
        if (alchemyPanel != null) alchemyPanel.SetActive(false);
        if (timingSlider != null) timingSlider.gameObject.SetActive(false);
        if (successTextObj != null) successTextObj.SetActive(false);
        if (failTextObj != null) failTextObj.SetActive(false);
        if (norecipe != null) norecipe.SetActive(false);
        if (alchemyAnimator != null) alchemyAnimator.gameObject.SetActive(false);
        if (recipeBookPanel != null) recipeBookPanel.SetActive(false);
        
        ResetOutput();
        SetupOutputTooltip();
    }

    private void SetupOutputTooltip()
    {
        if (outputIcon == null) return;

        if (outputTrigger == null)
            outputTrigger = outputIcon.gameObject.GetComponent<EventTrigger>() 
                         ?? outputIcon.gameObject.AddComponent<EventTrigger>();

        outputTrigger.triggers.Clear();

        EventTrigger.Entry enterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        enterEntry.callback.AddListener((e) =>
        {
            if (outputIcon.enabled && outputIcon.sprite != null)
            {
                ItemData resultItem = GetCurrentResultItem();
                if (resultItem != null)
                    ItemTooltipUI.Instance.Show(resultItem);
            }
        });
        outputTrigger.triggers.Add(enterEntry);

        EventTrigger.Entry exitEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        exitEntry.callback.AddListener((e) => ItemTooltipUI.Instance.Hide());
        outputTrigger.triggers.Add(exitEntry);
    }

    private ItemData GetCurrentResultItem()
    {
        foreach (var recipe in allRecipes)
        {
            if (TryMatchRecipe(recipe, out _))
                return recipe.resultItem;
        }
        return null;
    }

    public void OpenPanel()
    {
        if (alchemyPanel != null) alchemyPanel.SetActive(true);
        if (InventoryUI.Instance != null) InventoryUI.Instance.CloseInventory();
    }

    public void HidePanel()
    {
        if (alchemyPanel != null) alchemyPanel.SetActive(false);
        if (recipeBookPanel != null) recipeBookPanel.SetActive(false);
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

    private void Update()
    {
        if (recipeBookPanel != null && recipeBookPanel.activeSelf)
        {
            if (wasRecipeOpenedThisFrame)
            {
                wasRecipeOpenedThisFrame = false;
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (!RectTransformUtility.RectangleContainsScreenPoint(
                        recipeBookPanel.GetComponent<RectTransform>(), Input.mousePosition))
                {
                    recipeBookPanel.SetActive(false);
                }
            }
        }
    }

    public void ToggleRecipeBook()
    {
        if (recipeBookPanel == null) return;

        bool isOpening = !recipeBookPanel.activeSelf;
        recipeBookPanel.SetActive(isOpening);

        if (isOpening)
        {
            recipeBookPanel.transform.SetAsLastSibling();
            wasRecipeOpenedThisFrame = true;
            RenderRecipeList();
        }
    }

    private void RenderRecipeList()
    {
        if (recipeListText == null || allRecipes == null) return;

        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        sb.AppendLine("<color=#F5B642><b>□ RECIPE BOOK</b></color>\n");

        bool hasAnyRecipe = false;

        foreach (var recipe in allRecipes)
        {
            if (recipe == null || recipe.resultItem == null) continue;
            if (!recipe.IsUnlocked()) continue;

            hasAnyRecipe = true;

            sb.AppendLine(
                $"<color=#7CFF7C><b>{recipe.resultItem.itemName}</b></color>");

            sb.AppendLine();

            foreach (var ing in recipe.ingredients)
            {
                if (ing == null || ing.item == null) continue;

                sb.AppendLine(
                    $"<color=#EAEAEA>• {ing.item.itemName}</color> <color=#FFB347>x{ing.count}</color>");
            }

            sb.AppendLine();
            sb.AppendLine("<color=#666666>----------------</color>");
            sb.AppendLine();
        }

        if (!hasAnyRecipe)
        {
            sb.AppendLine("<color=#808080>No recipes learned yet.</color>");
        }

        recipeListText.text = sb.ToString();
    }

    public void OnCookButtonPress()
    {
        if (isCooking)
        {
            if (isWaitingForInput)
                CheckTiming();
            return;
        }

        RecipeData matchedRecipe = null;
        foreach (var recipe in allRecipes)
        {
            if (TryMatchRecipe(recipe, out int times))
            {
                matchedRecipe = recipe;
                craftTimes = times;
                break;
            }
        }

        if (matchedRecipe == null)
        {
            ShowNoRecipeMessage("No matching recipe!");
            return;
        }

        if (!matchedRecipe.IsUnlocked())
        {
            ShowNoRecipeMessage("You're not owned recipe !");
            return;
        }

        StartCoroutine(CookingRoutineWithMiniGame());
    }

    private void ShowNoRecipeMessage(string message)
    {
        if (norecipe == null) return;
        TextMeshProUGUI t = norecipe.GetComponent<TextMeshProUGUI>();
        if (t != null) t.text = message;

        if (noRecipeCoroutine != null) StopCoroutine(noRecipeCoroutine);
        noRecipeCoroutine = StartCoroutine(ShowFailNotificationTemporarily());
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

        if (val >= 0.47f && val <= 0.53f)
            currentResult = CookResult.Perfect;
        else if (val >= 0.25f && val <= 0.75f)
            currentResult = CookResult.Good;
        else
            currentResult = CookResult.Fail;
    }

    private IEnumerator CookingRoutineWithMiniGame()
    {
        if (timingSlider == null) yield break;

        alchemyAudio?.PlayCooking();

        isCooking = true;
        isWaitingForInput = true;
        currentResult = CookResult.Fail;

        timingSlider.gameObject.SetActive(true);
        timingSlider.value = 0f;

        if (alchemyAnimator != null)
        {
            alchemyAnimator.gameObject.SetActive(true);
            alchemyAnimator.SetTrigger("Cook");        
        }

        float startTime = Time.time;

        while (isWaitingForInput && (Time.time - startTime) < cookTime)
        {
            timingSlider.value = Mathf.PingPong((Time.time - startTime) * sliderSpeed, 1f);
            yield return null;
        }

        if (isWaitingForInput)
            CheckTiming();

        if (timingSlider != null) timingSlider.gameObject.SetActive(false);
        if (alchemyAnimator != null) alchemyAnimator.gameObject.SetActive(false);

        PerformCrafting();
    }

    private void PerformCrafting()
    {
        RecipeData validRecipe = null;
        foreach (var recipe in allRecipes)
        {
            if (TryMatchRecipe(recipe, out int times))
            {
                validRecipe = recipe;
                craftTimes = times;
                break;
            }
        }

        if (validRecipe == null)
        {
            isCooking = false;
            return;
        }

        ItemData itemToGive = validRecipe.resultItem;
        int finalAmount = validRecipe.resultCount * craftTimes;

        if (currentResult == CookResult.Perfect)
            finalAmount *= 2;
        else if (currentResult == CookResult.Fail)
        {
            itemToGive = trashItem;
            finalAmount = trashAmountPerBatch * craftTimes;
        }

        List<AlchemySlot> slots = new List<AlchemySlot> { inputSlot1, inputSlot2, inputSlot3 };

        foreach (var ing in validRecipe.ingredients)
        {
            foreach (var slot in slots)
            {
                if (slot.CurrentItem == ing.item && slot.CurrentAmount >= ing.count * craftTimes)
                {
                    InventoryManager.Instance.RemoveItem(slot.CurrentItem, ing.count * craftTimes);
                    break;
                }
            }
        }

        inputSlot1.UpdateVisual(null, 0);
        inputSlot2.UpdateVisual(null, 0);
        inputSlot3.UpdateVisual(null, 0);

        if (itemToGive != null)
        {
            InventoryManager.Instance.AddItem(itemToGive, finalAmount);
            outputIcon.sprite = itemToGive.icon;
            outputIcon.enabled = true;
            outputIcon.color = Color.white;
            outputAmountText.text = finalAmount.ToString();
            outputAmountText.gameObject.SetActive(true);
        }

        if (currentResult != CookResult.Fail)
            StartCoroutine(SuccessFeedbackRoutine(currentResult == CookResult.Perfect));
        else
            StartCoroutine(FailFeedbackRoutine());

        CancelSelection();
    }

    private IEnumerator SuccessFeedbackRoutine(bool isPerfect)
    {
        alchemyAudio?.PlaySuccess();

        if (successTextObj != null)
        {
            TextMeshProUGUI t = successTextObj.GetComponent<TextMeshProUGUI>();
            if (t != null) t.text = isPerfect ? "PERFECT! x2" : "SUCCESS";
            successTextObj.SetActive(true);
        }

        yield return new WaitForSeconds(feedbackDuration);

        if (successTextObj != null) successTextObj.SetActive(false);
        ResetOutput();
        isCooking = false;
    }

    private IEnumerator FailFeedbackRoutine()
    {
        alchemyAudio?.PlayFail();

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

        int activeSlotCount = 0;
        foreach (var slot in allInputSlots)
        {
            if (slot.CurrentItem != null && slot.CurrentAmount > 0)
                activeSlotCount++;
        }

        if (activeSlotCount != recipe.ingredients.Count)
            return false;

        int minPossible = int.MaxValue;

        foreach (var ing in recipe.ingredients)
        {
            bool found = false;
            foreach (var slot in allInputSlots)
            {
                if (slot.CurrentItem == ing.item)
                {
                    if (slot.CurrentAmount < ing.count)
                        return false;

                    int possible = slot.CurrentAmount / ing.count;
                    if (possible < minPossible)
                        minPossible = possible;

                    found = true;
                    break;
                }
            }
            if (!found) return false;
        }

        times = minPossible;
        return true;
    }

    private void ResetOutput()
    {
        if (outputIcon != null)
            outputIcon.enabled = false;
        if (outputAmountText != null) 
            outputAmountText.gameObject.SetActive(false);
    }

    public void StartSelection(AlchemySlot slot)
    {
        currentSelectingSlot = slot;
        if (InventoryUI.Instance != null)
            InventoryUI.Instance.OpenInventoryForSelection();
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

        if (inputSlot1.CurrentItem == null && inputSlot2.CurrentItem == null && inputSlot3.CurrentItem == null)
            return;

        foreach (var recipe in allRecipes)
        {
            if (TryMatchRecipe(recipe, out int times))
            {
                outputIcon.sprite = recipe.resultItem.icon;
                outputIcon.enabled = true;
                outputIcon.color = recipe.IsUnlocked() ? Color.white : Color.gray;
                outputAmountText.text = (recipe.resultCount * times).ToString();
                outputAmountText.gameObject.SetActive(true);
                craftTimes = times;

                SetupOutputTooltip();
                return;
            }
        }
    }

    public void CancelSelection() => currentSelectingSlot = null;
}