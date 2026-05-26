using UnityEngine;

public class DebugInventoryCheat : MonoBehaviour
{
    [Header("Button")]
    public KeyCode cheatKey = KeyCode.F1;

    [Header("List")]
    public ItemData[] allItemsInGame;

    [Header("Quantity of per items")]
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