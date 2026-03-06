using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestPopupUI : MonoBehaviour
{
    public static QuestPopupUI Instance { get; private set; }

    [Header("UI Components")]
    public GameObject popupPanel;       
    public TextMeshProUGUI titleText;   
    public TextMeshProUGUI descText;    
    public TextMeshProUGUI rewardText;  

    public Button acceptButton;
    public Button declineButton;

    private QuestData pendingQuest;   
    private void Awake()
    {
        Instance = this;
        popupPanel.SetActive(false);
        acceptButton.onClick.AddListener(OnAccept);
        declineButton.onClick.AddListener(OnDecline);
    }

    public void ShowPopup(QuestData quest)
    {
        pendingQuest = quest;
        titleText.text = quest.questName;
        descText.text = quest.description;

        string reward = $"{quest.goldReward} Vàng";
        if (quest.itemReward != null) reward += $" + {quest.itemReward.itemName}";
        rewardText.text = "Thưởng: " + reward;

        popupPanel.SetActive(true);
    }

    private void OnAccept()
    {
        if (pendingQuest != null) QuestManager.Instance.AcceptQuest(pendingQuest);
        ClosePopup();
    }

    private void OnDecline()
    {
        ClosePopup(); 
    }

    private void ClosePopup()
    {
        popupPanel.SetActive(false);
        pendingQuest = null;
    }
}