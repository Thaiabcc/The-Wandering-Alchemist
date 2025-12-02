using UnityEngine;

public class AlchemyStation : MonoBehaviour, IInteractable
{
    [Header("Liên kết")]
    [SerializeField] private GameObject alchemyPanel; // Kéo cái Panel UI vào đây

    public void Interact()
    {
        // Đơn giản là bật cái bảng lên
        if (alchemyPanel != null)
        {
            bool isActive = alchemyPanel.activeSelf;
            alchemyPanel.SetActive(!isActive);
        }
    }
}