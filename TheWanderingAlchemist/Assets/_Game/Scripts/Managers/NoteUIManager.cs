using UnityEngine;
using TMPro;

public class NoteUIManager : MonoBehaviour
{
    public static NoteUIManager Instance { get; private set; }

    [Header("UI Components")]
    public GameObject notePanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI contentText;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        if (notePanel != null) notePanel.SetActive(false);
    }

    public void ShowNote(string title, string content)
    {
        notePanel.SetActive(true);
        titleText.text = title;
        contentText.text = content;
        Time.timeScale = 0f;
    }
    public void CloseNote()
    {
        notePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (notePanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E))
            {
                CloseNote();
            }
        }
    }
}