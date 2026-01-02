using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [Header("Thông tin NPC")]
    [SerializeField] private string npcName = "Trưởng Làng";

    [Header("Danh sách File Nhiệm Vụ (Kéo thả vào đây)")]
    public QuestData[] questChain;

    [Header("Hội thoại khi hết việc")]
    [TextArea(2, 5)][SerializeField] private string[] allQuestsDoneLines;

    [Header("Cài đặt khác")]
    [SerializeField] private GameObject interactKey;

    private bool isPlayerInRange;

    private void Start() { SetInteractKey(false); }

    private void Update()
    {
        if (!isPlayerInRange) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            DialogueManager dialogue = DialogueManager.Instance;
            if (dialogue == null) return;

            if (dialogue.IsDialogueActive)
                dialogue.DisplayNextSentence();
            else
                CheckQuestStatusAndTalk();
        }
    }

    private void CheckQuestStatusAndTalk()
    {
        QuestManager qm = QuestManager.Instance;
        // Kiểm tra an toàn: Nếu không có QuestManager thì thôi
        if (qm == null) return;

        // --- 1. TRƯỜNG HỢP: ĐANG CÓ NHIỆM VỤ TRONG NGƯỜI ---
        if (qm.activeQuest != null && qm.activeQuest.info != null)
        {
            // Duyệt qua danh sách nhiệm vụ của NPC này
            foreach (QuestData qData in questChain)
            {
                // 👇 [FIX QUAN TRỌNG]: Nếu ô này trong Inspector bị để trống -> Bỏ qua ngay
                if (qData == null) continue;

                // So sánh tên nhiệm vụ
                if (qm.activeQuest.info.questName == qData.questName)
                {
                    HandleActiveQuest(qm);
                    return; // Tìm thấy rồi thì xử lý và thoát luôn
                }
            }

            // Nếu chạy hết vòng lặp mà không khớp (Tức là đang làm quest của NPC khác)
            DialogueManager.Instance.StartDialogue(npcName, new string[] {
                "Ngươi đang bận việc khác à? Xong đi rồi quay lại đây."
            });
            return;
        }

        // --- 2. TRƯỜNG HỢP: CHƯA CÓ NHIỆM VỤ (HOẶC TÌM NHIỆM VỤ MỚI) ---
        QuestData questToGive = null;

        foreach (QuestData qData in questChain)
        {
            // 👇 [FIX QUAN TRỌNG]: Bỏ qua ô trống
            if (qData == null) continue;

            // Kiểm tra xem nhiệm vụ này đã làm xong chưa
            if (!qm.IsQuestCompleted(qData.questName))
            {
                // Nếu chưa xong -> Đây là nhiệm vụ tiếp theo cần giao
                questToGive = qData;
                break; // Lấy cái đầu tiên tìm thấy
            }
        }

        // Nếu tìm được nhiệm vụ mới
        if (questToGive != null)
        {
            DialogueManager.Instance.StartDialogue(npcName, questToGive.startDialogue);
            qm.AcceptQuest(questToGive);
        }
        else
        {
            // Nếu không tìm được cái nào (Tức là đã làm hết sạch sành sanh)
            DialogueManager.Instance.StartDialogue(npcName, allQuestsDoneLines);
        }
    }

    private void HandleActiveQuest(QuestManager qm)
    {
        bool isReady = qm.CheckCompletionCondition();

        if (isReady)
        {
            // Lấy hội thoại Complete TỪ FILE QUEST
            DialogueManager.Instance.StartDialogue(npcName, qm.activeQuest.info.completeDialogue);
            qm.CompleteQuest();
        }
        else
        {
            // Lấy hội thoại Progress TỪ FILE QUEST
            if (qm.activeQuest.info.progressDialogue != null && qm.activeQuest.info.progressDialogue.Length > 0)
            {
                DialogueManager.Instance.StartDialogue(npcName, qm.activeQuest.info.progressDialogue);
            }
            else
            {
                DialogueManager.Instance.StartDialogue(npcName, new string[] { "Vẫn chưa xong à?" });
            }
        }
    }

    // ... (Phần Trigger giữ nguyên) ...
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) { isPlayerInRange = true; SetInteractKey(true); }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) { isPlayerInRange = false; SetInteractKey(false); DialogueManager.Instance?.EndDialogue(); }
    }
    private void SetInteractKey(bool state) { if (interactKey != null) interactKey.SetActive(state); }
}