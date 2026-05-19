using UnityEngine;

public class DebugInventoryCheat : MonoBehaviour
{
    [Header("Cấu hình phím bấm")]
    public KeyCode cheatKey = KeyCode.F1;

    [Header("Danh sách tất cả Item trong Game")]
    public ItemData[] allItemsInGame;

    [Header("Số lượng muốn thêm mỗi item")]
    public int amountToAdd = 10;

    private void Update()
    {
        if (Input.GetKeyDown(cheatKey))
        {
            GiveAllItemsToInventory();
        }
    }

    private void GiveAllItemsToInventory()
    {
        if (InventoryManager.Instance == null || allItemsInGame == null) return;

        foreach (ItemData item in allItemsInGame)
        {
            if (item != null)
            {
                InventoryManager.Instance.AddItem(item, amountToAdd);
            }
        }
    }
}