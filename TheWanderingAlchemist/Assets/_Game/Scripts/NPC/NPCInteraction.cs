using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [Header("Tên NPC")]
    [SerializeField] private string npcName = "Trưởng Làng";

    [Header("Các đoạn hội thoại")]
    [TextArea(2, 5)][SerializeField] private string[] questStartLines;
    [TextArea(2, 5)][SerializeField] private string[] questProgressLines;
    [TextArea(2, 5)][SerializeField] private string[] questCompletedLines;
    [TextArea(2, 5)][SerializeField] private string[] afterQuestLines;

    [Header("Cài đặt khác")]
    [SerializeField] private GameObject interactKey;

    private bool isPlayerInRange;

    // ==============================
    // Unity Lifecycle
    // ==============================
    private void Start()
    {
        SetInteractKey(false);
    }

    private void Update()
    {
        if (!isPlayerInRange)
            return;

        if (!Input.GetKeyDown(KeyCode.E))
            return;

        DialogueManager dialogue = DialogueManager.Instance;
        if (dialogue == null)
            return;

        if (dialogue.IsDialogueActive)
        {
            dialogue.DisplayNextSentence();
            return;
        }

        CheckQuestStatusAndTalk();
    }

    // ==============================
    // Dialogue Logic
    // ==============================
    private void CheckQuestStatusAndTalk()
    {
        QuestManager qm = QuestManager.Instance;
        if (qm == null)
            return;

        // 1️⃣ Chưa nhận nhiệm vụ
        if (!qm.isQuestStarted)
        {
            DialogueManager.Instance.StartDialogue(npcName, questStartLines);
            qm.AcceptQuest();
            return;
        }

        // 2️⃣ Đã đủ điều kiện hoàn thành nhưng chưa nhận thưởng
        if (!qm.isQuestCompleted && qm.killCount >= qm.targetKills)
        {
            DialogueManager.Instance.StartDialogue(npcName, questCompletedLines);
            qm.CompleteQuest();
            return;
        }

        // 3️⃣ Đang làm nhiệm vụ
        if (!qm.isQuestCompleted)
        {
            string statusLine =
                $"Ngươi mới diệt được {qm.killCount}/{qm.targetKills} con thôi. Làm việc nhanh cái tay lên!";

            string[] dynamicLines =
            {
                questProgressLines.Length > 0 ? questProgressLines[0] : statusLine,
                statusLine
            };

            DialogueManager.Instance.StartDialogue(npcName, dynamicLines);
            return;
        }

        // 4️⃣ Hoàn thành xong hết
        DialogueManager.Instance.StartDialogue(npcName, afterQuestLines);
    }

    // ==============================
    // Trigger
    // ==============================
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        isPlayerInRange = true;
        SetInteractKey(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        isPlayerInRange = false;
        SetInteractKey(false);

        if (DialogueManager.Instance != null)
            DialogueManager.Instance.EndDialogue();
    }

    // ==============================
    // Utils
    // ==============================
    private void SetInteractKey(bool state)
    {
        if (interactKey != null)
            interactKey.SetActive(state);
    }
}
