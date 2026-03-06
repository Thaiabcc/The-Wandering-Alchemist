using UnityEngine;

public class HintObject : MonoBehaviour, IInteractable
{
    [Header("UI")]
    public GameObject notePanel; 

    [Header("Settings")]
    public float autoCloseDistance = 3.0f; 

    private bool isOpen = false;
    private Transform playerTransform; 

    private void Start()
    {
        if (notePanel) notePanel.SetActive(false);

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            PlayerMovement pm = FindObjectOfType<PlayerMovement>();
            if (pm != null) playerTransform = pm.transform;
        }
    }

    private void Update()
    {
        if (isOpen && playerTransform != null)
        {
            float distance = Vector2.Distance(transform.position, playerTransform.position);
            if (distance > autoCloseDistance)
            {
                CloseNote();
            }
        }
    }

    public void Interact()
    {
        if (notePanel == null) return;

        isOpen = !isOpen;
        notePanel.SetActive(isOpen);
    }

    public void CloseNote()
    {
        isOpen = false;
        if (notePanel) notePanel.SetActive(false);
    }
}