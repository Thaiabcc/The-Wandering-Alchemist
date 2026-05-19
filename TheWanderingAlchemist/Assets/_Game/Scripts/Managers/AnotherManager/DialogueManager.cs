using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public enum InteractionType
{
    Chat,       
    Quest,     
    Combat,     
    Exit,       
    Shop,      
    Heal       
}
public class ChoiceOption
{
    public string buttonText;
    public InteractionType type; 
    public Action onChosen;

    public ChoiceOption(string text, InteractionType iconType, Action action)
    {
        buttonText = text;
        type = iconType;
        onChosen = action;
    }
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI Components")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI contentText;

    [Header("Dynamic Choice UI")]
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private GameObject choiceButtonPrefab;
    [SerializeField] private Transform choiceContainer;

    [Header("Icons Database")]
    public List<Sprite> interactionIcons;

    private List<GameObject> currentButtons = new List<GameObject>();
    private Queue<string> sentences = new Queue<string>();
    public bool IsDialogueActive { get; private set; } = false;
    public Action onDialogueEnded;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        sentences = new Queue<string>();
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (choicePanel != null) choicePanel.SetActive(false);
    }

    public void StartDialogue(string name, string[] lines)
    {
        IsDialogueActive = true;
        dialoguePanel.SetActive(true);
        choicePanel.SetActive(false);
        nameText.text = name;
        sentences.Clear();
        foreach (string line in lines) sentences.Enqueue(line);
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0) { EndDialogue(); return; }
        contentText.text = sentences.Dequeue();
    }

    public void EndDialogue()
    {
        if (choicePanel.activeSelf) return;
        IsDialogueActive = false;
        dialoguePanel.SetActive(false);
        onDialogueEnded?.Invoke();
        onDialogueEnded = null;
    }

    public void ForceClose()
    {
        IsDialogueActive = false;
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (choicePanel != null) choicePanel.SetActive(false);
        onDialogueEnded = null;
        foreach (var btn in currentButtons) if (btn != null) Destroy(btn);
        currentButtons.Clear();
    }

    public void ShowDynamicChoices(List<ChoiceOption> options)
    {
        IsDialogueActive = true;
        dialoguePanel.SetActive(true);
        choicePanel.SetActive(true);

        foreach (var btn in currentButtons) if (btn != null) Destroy(btn);
        currentButtons.Clear();

        foreach (var option in options)
        {
            GameObject btnObj = Instantiate(choiceButtonPrefab, choiceContainer);
            btnObj.transform.localScale = Vector3.one;
            currentButtons.Add(btnObj);
            TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null) btnText.text = option.buttonText;
            Image iconImg = null;
            foreach (var img in btnObj.GetComponentsInChildren<Image>())
            {
                if (img.gameObject.name == "Icon")
                {
                    iconImg = img;
                    break;
                }
            }

            if (iconImg != null && interactionIcons.Count > (int)option.type)
            {
                iconImg.sprite = interactionIcons[(int)option.type];
            }
            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => {
                choicePanel.SetActive(false);
                option.onChosen?.Invoke();
            });
        }
    }
}