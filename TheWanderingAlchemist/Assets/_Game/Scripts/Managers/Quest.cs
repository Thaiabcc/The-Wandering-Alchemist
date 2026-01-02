using UnityEngine;

[System.Serializable]
public class Quest
{
    // Tham chiếu đến file dữ liệu gốc (không sửa đổi file này lúc chơi)
    public QuestData info;

    // Biến thay đổi theo thời gian thực (lưu tiến độ)
    public int currentAmount;
    public bool isCompleted;

    // Hàm khởi tạo nhanh
    public Quest(QuestData data)
    {
        info = data;
        currentAmount = 0;
        isCompleted = false;
    }
}