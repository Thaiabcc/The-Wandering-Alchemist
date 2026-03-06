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
    [TextArea(2, 5)] public string[] defaultLines = { "Xin chào." };
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

        if (qm == null || dm == null)
        {
            Debug.LogError("Missing QuestManager hoặc DialogueManager");
            return;
        }
        if (dm.IsDialogueActive)
        {
            dm.DisplayNextSentence();
            return;
        }
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
        if (QuestPopupUI.Instance != null &&
            QuestPopupUI.Instance.popupPanel != null &&
            QuestPopupUI.Instance.popupPanel.activeSelf)
        {
            QuestPopupUI.Instance.popupPanel.SetActive(false);
        }

        currentGroupID = "MAIN";
        if (qm.activeQuest != null && qm.activeQuest.info != null)
        {
            var info = qm.activeQuest.info;
            if (info.type == QuestType.TalkToNPC &&
                !string.IsNullOrEmpty(info.targetName) &&
                info.targetName == npcName)
            {
                dm.onDialogueEnded = ShowCurrentDialogueGroup;
                dm.StartDialogue(
                    npcName,
                    (questIntroLines != null && questIntroLines.Length > 0) ? questIntroLines : defaultLines
                );
                return;
            }
            if (IsMyQuest(info.questName))
            {
                dm.onDialogueEnded = ShowCurrentDialogueGroup;
                dm.StartDialogue(
                    npcName,
                    (info.progressDialogue != null && info.progressDialogue.Length > 0)
                        ? info.progressDialogue
                        : new[] { "Việc ta nhờ thế nào rồi?" }
                );
                return;
            }
        }

        if (questsToGive != null)
        {
            foreach (var quest in questsToGive)
            {
                if (quest == null || string.IsNullOrEmpty(quest.questName)) continue;
                if (quest.prerequisiteQuest != null)
                {
                    if (!qm.IsQuestCompleted(quest.prerequisiteQuest.questName)) continue;
                }
                if (qm.IsQuestCompleted(quest.questName)) continue;
                if (qm.activeQuest != null && qm.activeQuest.info != null &&
                    qm.activeQuest.info.questName == quest.questName) continue;
                quest.startDialogue ??= new string[0];

                dm.onDialogueEnded = () =>
                {
                    if (QuestPopupUI.Instance != null)
                        QuestPopupUI.Instance.ShowPopup(quest);
                };

                if (quest.startDialogue.Length > 0)
                    dm.StartDialogue(npcName, quest.startDialogue);
                else
                    QuestPopupUI.Instance.ShowPopup(quest);

                return;
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

        var group = dialogueGroups.Find(g => g.groupID == currentGroupID) ??
                    dialogueGroups.Find(g => g.groupID == "MAIN");

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

                    currentGroupID = opt.returnToMain ? "MAIN" :
                                     (string.IsNullOrEmpty(opt.nextGroupID) ? currentGroupID : opt.nextGroupID);

                    opt.npcResponse ??= new string[0];

                    if (opt.npcResponse.Length > 0)
                    {
                        dm.onDialogueEnded = ShowCurrentDialogueGroup;
                        dm.StartDialogue(npcName, opt.npcResponse);
                    }
                    else
                    {
                        ShowCurrentDialogueGroup();
                    }
                }));
            }
        }

        if (currentGroupID == "MAIN" && qm != null && qm.activeQuest != null && qm.activeQuest.info != null)
        {
            var info = qm.activeQuest.info;

            if (IsMyQuest(info.questName))
            {
                if (qm.CheckCompletionCondition())
                {
                    uiOptions.Add(new ChoiceOption("[HOÀN THÀNH] Báo cáo", InteractionType.Quest, () =>
                    {
                        dm.ShowDynamicChoices(new List<ChoiceOption>());
                        bool isFirstTime = !qm.IsQuestCompleted(info.questName);
                        string storyContent = info.endStoryText;
                        string[] thanksLines = (info.completeDialogue != null && info.completeDialogue.Length > 0)
                                                ? info.completeDialogue
                                                : new[] { "Cảm ơn con rất nhiều!" };

                        System.Action runCompletionLogic = () =>
                        {
                            qm.CompleteQuest();
                            dm.onDialogueEnded = null;
                            dm.StartDialogue(npcName, thanksLines);
                        };
                        if (isFirstTime && !string.IsNullOrEmpty(storyContent) && StoryTransitionUI.Instance != null)
                        {
                            dm.ForceClose();
                            StoryTransitionUI.Instance.PlayStory(storyContent, onComplete: runCompletionLogic);
                        }
                        else
                        {
                            runCompletionLogic.Invoke();
                        }
                    }));
                }
                else
                {
                    uiOptions.Add(new ChoiceOption("[QUEST] Tiến độ nhiệm vụ?", InteractionType.Chat, () =>
                    {
                        dm.onDialogueEnded = ShowCurrentDialogueGroup;
                        dm.StartDialogue(
                            npcName,
                            (info.progressDialogue != null && info.progressDialogue.Length > 0)
                                ? info.progressDialogue
                                : new[] { "Vẫn chưa xong à?" }
                        );
                    }));
                }
            }
            else if (info.type == QuestType.TalkToNPC && !string.IsNullOrEmpty(info.targetName) && info.targetName == npcName)
            {
                uiOptions.Add(new ChoiceOption("[QUEST] Nói về nhiệm vụ...", InteractionType.Quest, () =>
                {
                    qm.UpdateQuestProgress(1);
                    dm.onDialogueEnded = destroyAfterTalk
                        ? () => Destroy(gameObject)
                        : ShowCurrentDialogueGroup;

                    dm.StartDialogue(
                        npcName,
                        (info.targetDialogue != null && info.targetDialogue.Length > 0)
                            ? info.targetDialogue
                            : new[] { "..." }
                    );
                }));
            }
        }

        uiOptions.Add(new ChoiceOption("Tạm biệt", InteractionType.Exit, () =>
        {
            dm.onDialogueEnded = null;
            dm.StartDialogue(npcName, new[] { "Hẹn gặp lại." });
        }));

        dm.ShowDynamicChoices(uiOptions);
    }

    public bool IsMyQuest(string questName)
    {
        if (questsToGive == null || string.IsNullOrEmpty(questName)) return false;

        foreach (var q in questsToGive)
            if (q != null && q.questName == questName)
                return true;

        return false;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player")) isPlayerInRange = true;
    }

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