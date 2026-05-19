using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[System.Serializable]
public class FlavorOption
{
    public string buttonText;
    public InteractionType type = InteractionType.Chat;
    [TextArea(2, 5)] public string[] npcResponse;
    public string nextGroupID;
    public bool returnToMain = false;
    public bool closeDialogue = false;
    public bool isOneTimeOnly = false;
    public UnityEvent onOptionSelected;
    [HideInInspector] public bool hasBeenClicked = false;
}

[System.Serializable]
public class DialogueGroup
{
    public string groupID;
    public List<FlavorOption> options;
}

public class QuestNPC : MonoBehaviour, IInteractable
{
    [Header("--- NPC ---")]
    public string npcName;
    public List<QuestData> questsToGive;
    public bool destroyAfterTalk = false;

    [Header("--- DIALOGUE ---")]
    [TextArea(2, 5)] public string[] defaultLines = { "Hello." };
    [TextArea(2, 5)] public string[] questIntroLines = { "..." };

    [Header("--- DIALOGUE TREE ---")]
    public List<DialogueGroup> dialogueGroups = new();

    private bool isPlayerInRange;
    private string currentGroupID = "MAIN";

    public void Interact()
    {
        if (!isPlayerInRange) return;

        var qm = QuestManager.Instance;
        var dm = DialogueManager.Instance;

        if (qm == null || dm == null) return;

        if (dm.IsDialogueActive)
        {
            dm.DisplayNextSentence();
            return;
        }

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
        if (QuestPopupUI.Instance != null && QuestPopupUI.Instance.popupPanel.activeSelf)
        {
            QuestPopupUI.Instance.popupPanel.SetActive(false);
        }

        currentGroupID = "MAIN";

        Quest talkQuest = qm.activeQuests.Find(q => q.info.type == QuestType.TalkToNPC && q.info.targetName == npcName);
        if (talkQuest != null)
        {
            dm.onDialogueEnded = ShowCurrentDialogueGroup;
            dm.StartDialogue(npcName, (questIntroLines != null && questIntroLines.Length > 0) ? questIntroLines : defaultLines);
            return;
        }

        Quest myActiveQuest = qm.activeQuests.Find(q => IsMyQuest(q.info.questName));
        if (myActiveQuest != null)
        {
            dm.onDialogueEnded = ShowCurrentDialogueGroup;
            dm.StartDialogue(npcName, (myActiveQuest.info.progressDialogue != null && myActiveQuest.info.progressDialogue.Length > 0)
                    ? myActiveQuest.info.progressDialogue : new[] { "How is the task I asked you to do?" });
            return;
        }

        if (questsToGive != null)
        {
            foreach (var data in questsToGive)
            {
                if (data == null || string.IsNullOrEmpty(data.questName)) continue;
                if (qm.IsQuestCompleted(data.questName)) continue;
                if (data.prerequisiteQuest != null && !qm.IsQuestCompleted(data.prerequisiteQuest.questName)) continue;

                if (!qm.activeQuests.Exists(q => q.info.questName == data.questName))
                {
                    data.startDialogue ??= new string[0];
                    dm.onDialogueEnded = () => { if (QuestPopupUI.Instance != null) QuestPopupUI.Instance.ShowPopup(data); };

                    if (data.startDialogue.Length > 0) dm.StartDialogue(npcName, data.startDialogue);
                    else QuestPopupUI.Instance.ShowPopup(data);

                    return;
                }
            }
        }

        dm.onDialogueEnded = ShowCurrentDialogueGroup;
        dm.StartDialogue(npcName, defaultLines);
    }

    private void ShowCurrentDialogueGroup()
    {
        var dm = DialogueManager.Instance;
        var qm = QuestManager.Instance;
        if (dm == null) return;

        var group = dialogueGroups.Find(g => g.groupID == currentGroupID) ?? dialogueGroups.Find(g => g.groupID == "MAIN");
        var uiOptions = new List<ChoiceOption>();

        if (group != null && group.options != null)
        {
            foreach (var opt in group.options)
            {
                if (opt.isOneTimeOnly && opt.hasBeenClicked) continue;
                uiOptions.Add(new ChoiceOption(opt.buttonText, opt.type, () =>
                {
                    opt.hasBeenClicked = true;
                    opt.onOptionSelected?.Invoke();
                    if (opt.closeDialogue) return;

                    currentGroupID = opt.returnToMain ? "MAIN" : (string.IsNullOrEmpty(opt.nextGroupID) ? currentGroupID : opt.nextGroupID);
                    opt.npcResponse ??= new string[0];

                    if (opt.npcResponse.Length > 0)
                    {
                        dm.onDialogueEnded = ShowCurrentDialogueGroup;
                        dm.StartDialogue(npcName, opt.npcResponse);
                    }
                    else ShowCurrentDialogueGroup();
                }));
            }
        }

        if (currentGroupID == "MAIN" && qm != null)
        {
            Quest myActiveQuest = qm.activeQuests.Find(q => IsMyQuest(q.info.questName));
            if (myActiveQuest != null)
            {
                if (qm.CheckCompletionCondition(myActiveQuest))
                {
                    uiOptions.Add(new ChoiceOption("[COMPLETE] Report", InteractionType.Quest, () =>
                    {
                        dm.ShowDynamicChoices(new List<ChoiceOption>());
                        string storyContent = myActiveQuest.info.endStoryText;
                        string[] thanksLines = (myActiveQuest.info.completeDialogue != null && myActiveQuest.info.completeDialogue.Length > 0)
                                                ? myActiveQuest.info.completeDialogue : new[] { "Thank you very much!" };

                        System.Action runCompletionLogic = () =>
                        {
                            qm.CompleteQuest(myActiveQuest.info.questName);
                            dm.onDialogueEnded = null;
                            dm.StartDialogue(npcName, thanksLines);
                        };

                        if (!string.IsNullOrEmpty(storyContent) && StoryTransitionUI.Instance != null)
                        {
                            dm.ForceClose();
                            StoryTransitionUI.Instance.PlayStory(storyContent, onComplete: runCompletionLogic);
                        }
                        else runCompletionLogic.Invoke();
                    }));
                }
                else
                {
                    uiOptions.Add(new ChoiceOption("[QUEST] Quest Progress?", InteractionType.Chat, () =>
                    {
                        dm.onDialogueEnded = ShowCurrentDialogueGroup;
                        dm.StartDialogue(npcName, (myActiveQuest.info.progressDialogue != null && myActiveQuest.info.progressDialogue.Length > 0)
                                ? myActiveQuest.info.progressDialogue : new[] { "Not done yet?" });
                    }));
                }
            }

            Quest talkQuest = qm.activeQuests.Find(q => q.info.type == QuestType.TalkToNPC && q.info.targetName == npcName);
            if (talkQuest != null)
            {
                uiOptions.Add(new ChoiceOption("[QUEST] Talk about quest...", InteractionType.Quest, () =>
                {
                    qm.TalkToNPC(npcName); 
                    dm.onDialogueEnded = destroyAfterTalk ? () => Destroy(gameObject) : ShowCurrentDialogueGroup;
                    dm.StartDialogue(npcName, (talkQuest.info.targetDialogue != null && talkQuest.info.targetDialogue.Length > 0)
                            ? talkQuest.info.targetDialogue : new[] { "..." });
                }));
            }
        }

        uiOptions.Add(new ChoiceOption("Goodbye", InteractionType.Exit, () =>
        {
            dm.onDialogueEnded = null;
            dm.StartDialogue(npcName, new[] { "See you again." });
        }));

        dm.ShowDynamicChoices(uiOptions);
    }

    public bool IsMyQuest(string questName)
    {
        if (questsToGive == null || string.IsNullOrEmpty(questName)) return false;
        foreach (var q in questsToGive)
            if (q != null && q.questName == questName) return true;
        return false;
    }

    private void OnTriggerEnter2D(Collider2D col) { if (col.CompareTag("Player")) isPlayerInRange = true; }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;
        isPlayerInRange = false;
        currentGroupID = "MAIN";
        DialogueManager.Instance?.ForceClose();
        if (QuestPopupUI.Instance != null && QuestPopupUI.Instance.popupPanel != null)
            QuestPopupUI.Instance.popupPanel.SetActive(false);
    }
}