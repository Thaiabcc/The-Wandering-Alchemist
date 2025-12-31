using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Trạng thái Nhiệm vụ")]
    public bool isQuestStarted;
    public bool isQuestCompleted;

    [Header("Tiến độ")]
    public int killCount;
    public int targetKills = 3;

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

    // ==============================
    // Quest Logic
    // ==============================
    public void AcceptQuest()
    {
        if (isQuestStarted)
            return;

        isQuestStarted = true;
        killCount = 0;

        Debug.Log("Đã nhận nhiệm vụ: Diệt Slime!");
    }

    public void AddKill()
    {
        if (!isQuestStarted || isQuestCompleted)
            return;

        killCount++;
        Debug.Log($"Tiến độ: {killCount}/{targetKills}");

        if (killCount >= targetKills)
        {
            Debug.Log("Nhiệm vụ hoàn tất! Về gặp NPC nhận thưởng thôi.");
        }
    }

    public void CompleteQuest()
    {
        if (isQuestCompleted)
            return;

        isQuestCompleted = true;
        Debug.Log("Đã nhận thưởng! +100 Vàng (Ví dụ)");
    }
}
