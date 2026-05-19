using UnityEngine;

public class QuestIcon : MonoBehaviour
{
    [Header("Icons")]
    public GameObject chatBubbleBG;
    public GameObject questAvailableIcon; 
    public GameObject questActiveIcon;    
    public GameObject questReadyIcon;     

    private QuestNPC myNPC;

    private void Start()
    {
        myNPC = GetComponentInParent<QuestNPC>();

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestUpdated += UpdateIcon;
            UpdateIcon(); 
        }
    }

    private void OnDestroy()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestUpdated -= UpdateIcon;
        }
    }

    public void UpdateIcon()
    {
        if (myNPC == null || QuestManager.Instance == null) return;

        TurnOffAllIcons();

        foreach (Quest q in QuestManager.Instance.activeQuests)
        {
            if (myNPC.IsMyQuest(q.info.questName))
            {
                if (q.isCompleted || QuestManager.Instance.CheckCompletionCondition(q))
                {
                    if (questReadyIcon) questReadyIcon.SetActive(true);
                    if (chatBubbleBG) chatBubbleBG.SetActive(true);
                }
                else
                {
                    if (questActiveIcon) questActiveIcon.SetActive(true);
                    if (chatBubbleBG) chatBubbleBG.SetActive(true);
                }
                return; 
            }

            if (q.info.type == QuestType.TalkToNPC && q.info.targetName == myNPC.npcName)
            {
                if (questReadyIcon) questReadyIcon.SetActive(true);
                if (chatBubbleBG) chatBubbleBG.SetActive(true);
                return;
            }
        }

        if (myNPC.questsToGive != null)
        {
            foreach (QuestData data in myNPC.questsToGive)
            {
                if (data == null) continue;
                
                if (QuestManager.Instance.IsQuestCompleted(data.questName)) continue;
                
                if (data.prerequisiteQuest != null && !QuestManager.Instance.IsQuestCompleted(data.prerequisiteQuest.questName)) continue;

                if (questAvailableIcon) questAvailableIcon.SetActive(true);
                if (chatBubbleBG) chatBubbleBG.SetActive(true); 
                return;
            }
        }
    }

    private void TurnOffAllIcons()
    {
        if (questAvailableIcon) questAvailableIcon.SetActive(false);
        if (questActiveIcon) questActiveIcon.SetActive(false);
        if (questReadyIcon) questReadyIcon.SetActive(false);
        if (chatBubbleBG) chatBubbleBG.SetActive(false);
    }
}