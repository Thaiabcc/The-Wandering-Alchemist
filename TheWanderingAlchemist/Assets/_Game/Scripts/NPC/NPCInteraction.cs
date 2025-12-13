using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [Header("Tên NPC")]
    [SerializeField] private string npcName = "Trưởng Làng";

    [Header("Các đoạn hội thoại")]
    [TextArea(2, 5)][SerializeField] private string[] questStartLines; // 1. Mời nhận Q
    [TextArea(2, 5)][SerializeField] private string[] questProgressLines; // 2. Đang làm (Hối thúc)
    [TextArea(2, 5)][SerializeField] private string[] questCompletedLines; // 3. Đã xong (Khen thưởng)
    [TextArea(2, 5)][SerializeField] private string[] afterQuestLines; // 4. Sau khi xong hết (Chém gió)

    [Header("Cài đặt khác")]
    [SerializeField] private GameObject interactKey;

    private bool isPlayerInRange = false;

    private void Start()
    {
        if (interactKey != null) interactKey.SetActive(false);
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (DialogueManager.Instance != null)
            {
                if (DialogueManager.Instance.IsDialogueActive)
                {
                    // Nếu đang nói dở -> Next câu
                    DialogueManager.Instance.DisplayNextSentence();
                }
                else
                {
                    // Nếu chưa nói -> Bắt đầu hội thoại (Kiểm tra trạng thái Q)
                    CheckQuestStatusAndTalk();
                }
            }
        }
    }

    private void CheckQuestStatusAndTalk()
    {
        QuestManager qm = QuestManager.Instance;
        if (qm == null) return;

        // TRƯỜNG HỢP 1: Chưa nhận nhiệm vụ
        if (!qm.isQuestStarted)
        {
            DialogueManager.Instance.StartDialogue(npcName, questStartLines);
            qm.AcceptQuest(); // Tự động nhận Q luôn khi nói chuyện
            return;
        }

        // TRƯỜNG HỢP 2: Đã xong nhiệm vụ (Đủ số lượng) NHƯNG chưa nhận thưởng
        if (qm.isQuestStarted && !qm.isQuestCompleted && qm.killCount >= qm.targetKills)
        {
            DialogueManager.Instance.StartDialogue(npcName, questCompletedLines);
            qm.CompleteQuest(); // Nhận thưởng
            return;
        }

        // TRƯỜNG HỢP 3: Đang làm nhiệm vụ (Chưa đủ số lượng)
        if (qm.isQuestStarted && !qm.isQuestCompleted)
        {
            // Tạo câu thoại động: "Ngươi mới giết X/3 con thôi!"
            string statusLine = $"Ngươi mới diệt được {qm.killCount}/{qm.targetKills} con thôi. Nhanh lên!";
            string[] dynamicLines = { questProgressLines[0], statusLine };

            DialogueManager.Instance.StartDialogue(npcName, dynamicLines);
            return;
        }

        // TRƯỜNG HỢP 4: Đã xong tất cả
        if (qm.isQuestCompleted)
        {
            DialogueManager.Instance.StartDialogue(npcName, afterQuestLines);
        }
    }

    // ... (Giữ nguyên OnTriggerEnter/Exit như cũ) ...
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (interactKey != null) interactKey.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (interactKey != null) interactKey.SetActive(false);
            if (DialogueManager.Instance != null) DialogueManager.Instance.EndDialogue();
        }
    }
}