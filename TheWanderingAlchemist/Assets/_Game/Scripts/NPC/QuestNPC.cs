using UnityEngine;
using System.Collections.Generic;

public class QuestNPC : MonoBehaviour, IInteractable
{
    [Header("Cài đặt NPC")]
    public string npcName;
    public List<QuestData> questsToGive;
    [TextArea(2, 5)] public string[] defaultLines = { "Chào người anh em thiện lành!" };

    public void Interact()
    {
        // Lấy các Manager (Giả sử đã Setup đúng trong Scene)
        var qm = QuestManager.Instance;
        var dm = DialogueManager.Instance;

        // 1. NẾU ĐANG NÓI CHUYỆN -> BẤM E ĐỂ NEXT CÂU TIẾP THEO
        if (dm.IsDialogueActive)
        {
            dm.DisplayNextSentence();
            return;
        }

        // 2. NẾU ĐANG LÀM QUEST -> KIỂM TRA TIẾN ĐỘ / TRẢ NHIỆM VỤ
        if (qm.activeQuest != null && qm.activeQuest.info != null)
        {
            // Trường hợp A: Quest yêu cầu gặp NPC này (TalkToNPC)
            if (qm.activeQuest.info.type == QuestType.TalkToNPC &&
                qm.activeQuest.info.targetName == npcName)
            {
                qm.TalkToNPC(npcName);
                dm.StartDialogue(npcName, qm.activeQuest.info.progressDialogue);
                return;
            }

            // Trường hợp B: Trả Quest do NPC này giao
            if (questsToGive.Contains(qm.activeQuest.info))
            {
                if (qm.CheckCompletionCondition())
                {
                    // Hoàn thành -> Nói xong thì nhận thưởng
                    dm.onDialogueEnded = () => qm.CompleteQuest();
                    dm.StartDialogue(npcName, qm.activeQuest.info.completeDialogue);
                }
                else
                {
                    // Chưa xong -> Nhắc nhở
                    dm.StartDialogue(npcName, qm.activeQuest.info.progressDialogue);
                }
                return;
            }
        }

        // 3. NẾU CHƯA CÓ QUEST -> TÌM QUEST MỚI ĐỂ GIAO
        foreach (var quest in questsToGive)
        {
            if (quest == null) continue; // Bỏ qua ô trống
            if (qm.IsQuestCompleted(quest.questName)) continue; // Bỏ qua quest đã làm

            // Kiểm tra Quest tiền đề (Quest Chain)
            if (quest.prerequisiteQuest != null)
            {
                if (!qm.IsQuestCompleted(quest.prerequisiteQuest.questName))
                {
                    dm.StartDialogue(npcName, new string[] { "Ta chưa có việc gì cho ngươi. Hãy hoàn thành công việc trước đó đi." });
                    return;
                }
            }

            // --- ĐỦ ĐIỀU KIỆN GIAO QUEST ---

            // Cài đặt: Nói xong thì hiện bảng Popup
            dm.onDialogueEnded = () => {
                QuestPopupUI.Instance.ShowPopup(quest);
            };

            // Bắt đầu hội thoại dẫn dắt (nếu có), không thì hiện bảng luôn
            if (quest.startDialogue != null && quest.startDialogue.Length > 0)
                dm.StartDialogue(npcName, quest.startDialogue);
            else
                QuestPopupUI.Instance.ShowPopup(quest);

            return;
        }

        // 4. HẾT VIỆC -> NÓI CHUYỆN PHIẾM
        dm.StartDialogue(npcName, defaultLines);
    }
}