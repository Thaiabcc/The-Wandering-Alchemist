using UnityEngine;

public class NoteItem : MonoBehaviour
{
    public string noteTitle = "Title";

    [TextArea(5, 10)]
    public string noteContent = "Content..";

    private bool isPlayerNearby;

    private void Update()
    {
        if (!isPlayerNearby) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            NoteUIManager.Instance.ShowNote(noteTitle, noteContent);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        isPlayerNearby = true;
        Debug.Log("Press E to read");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        isPlayerNearby = false;
        NoteUIManager.Instance.CloseNote();
    }
}
