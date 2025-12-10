using UnityEngine;

public class AlchemyStation : MonoBehaviour, IInteractable
{
    // Xóa dòng biến GameObject alchemyPanel cũ đi
    // [SerializeField] private GameObject alchemyPanel; <--- XÓA HOẶC COMMENT

    public void Interact()
    {
        // Gọi thẳng vào Singleton của AlchemyUI
        if (AlchemyUI.Instance != null)
        {
            // Kiểm tra xem bảng đang mở hay đóng
            bool isOpening = !AlchemyUI.Instance.alchemyPanel.activeSelf;

            if (isOpening)
            {
                AlchemyUI.Instance.OpenPanel();
            }
            else
            {
                AlchemyUI.Instance.CloseButtonAction();
            }
        }
        else
        {
            Debug.LogError("Lỗi: Không tìm thấy AlchemyUI! (Kiểm tra xem UI_Manager đã chạy từ đầu chưa?)");
        }
    }
}