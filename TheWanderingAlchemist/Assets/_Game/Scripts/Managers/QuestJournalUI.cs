using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestJournalUI : MonoBehaviour
{
    [Header("--- CỘT TRÁI (DANH SÁCH) ---")]
    public GameObject journalPanel;
    public Transform contentContainer;
    public GameObject questSlotPrefab;

    [Header("--- CỘT PHẢI (CHI TIẾT MÔ TẢ) ---")]
    public GameObject detailPanel;       
    public TextMeshProUGUI detailTitle;   
    public TextMeshProUGUI detailDesc;    
    public TextMeshProUGUI detailProgress;

    private Quest selectedQuest; 
    private void Start()
    {
        journalPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (journalPanel.activeSelf) CloseJournal();
            else OpenJournal();
        }
    }

    public void OpenJournal()
    {
        journalPanel.SetActive(true);
        RefreshList();
        if (QuestManager.Instance.activeQuests.Count > 0)
        {
            ShowQuestDetails(QuestManager.Instance.activeQuests[0]);
        }
        else
        {
            detailPanel.SetActive(false); 
        }
    }

    public void CloseJournal()
    {
        journalPanel.SetActive(false);
    }

    private void RefreshList()
    {
        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }
        foreach (Quest q in QuestManager.Instance.activeQuests)
        {
            if (q == null) continue;
            if (q.info == null)
            {
                continue;
            }

            GameObject slot = Instantiate(questSlotPrefab, contentContainer);
            Transform nameQuestObj = slot.transform.Find("NameQuest");
            Transform followBtnObj = slot.transform.Find("FollowButton/Button");

            if (nameQuestObj == null || followBtnObj == null)
            {
                continue;
            }
            TextMeshProUGUI txtName = nameQuestObj.GetComponent<TextMeshProUGUI>();
            if (txtName == null)
            {
                continue;
            }

            Button trackBtn = followBtnObj.GetComponent<Button>();
            if (trackBtn == null)
            {
                continue;
            }

            TextMeshProUGUI btnText = trackBtn.GetComponentInChildren<TextMeshProUGUI>();
            string status = q.isCompleted ? " <color=green>(Completed)</color>" : "";
            txtName.text = q.info.questName + status;
            if (QuestManager.Instance.trackedQuest == q)
            {
                if (btnText != null) btnText.text = "Untrack";
            }
            else
            {
                if (btnText != null) btnText.text = "Track";
            }
            trackBtn.onClick.RemoveAllListeners();
            trackBtn.onClick.AddListener(() => 
            {
                if (QuestManager.Instance.trackedQuest == q)
                    QuestManager.Instance.UntrackQuest();
                else
                    QuestManager.Instance.TrackQuest(q);

                RefreshList();
            });
            Button slotBtn = slot.GetComponent<Button>();
            if (slotBtn != null)
            {
                slotBtn.onClick.RemoveAllListeners();
                slotBtn.onClick.AddListener(() => 
                {
                    ShowQuestDetails(q);
                });
            }
        }
    }
    private void ShowQuestDetails(Quest q)
    {
        selectedQuest = q;
        detailPanel.SetActive(true); 
        detailTitle.text = q.info.questName;
        detailDesc.text = q.info.description;
        detailProgress.text = $"Progress: <color=yellow>{q.currentAmount} / {q.info.requiredAmount}</color>";
    }
}