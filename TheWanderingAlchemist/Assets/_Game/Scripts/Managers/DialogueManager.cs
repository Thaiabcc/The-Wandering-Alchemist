using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System; // 👈 [MỚI] Cần cái này để dùng Action

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI Components")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI contentText;

    private Queue<string> sentences;
    public bool IsDialogueActive { get; private set; } = false;

    // 👇 [MỚI] Biến này chứa hành động sẽ làm sau khi nói xong
    public Action onDialogueEnded;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        sentences = new Queue<string>();
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    public void StartDialogue(string name, string[] lines)
    {
        IsDialogueActive = true;
        if (dialoguePanel != null) dialoguePanel.SetActive(true);

        nameText.text = name;
        sentences.Clear();

        foreach (string line in lines)
        {
            sentences.Enqueue(line);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        contentText.text = sentence;
    }

    public void EndDialogue()
    {
        IsDialogueActive = false;
        if (dialoguePanel != null) dialoguePanel.SetActive(false);

        // 👇 [MỚI] Kích hoạt hành động chờ (Ví dụ: Mở bảng Quest)
        onDialogueEnded?.Invoke();

        // 👇 [MỚI] Xóa hành động đi để lần sau nói chuyện phiếm không bị lặp lại
        onDialogueEnded = null;
    }
}