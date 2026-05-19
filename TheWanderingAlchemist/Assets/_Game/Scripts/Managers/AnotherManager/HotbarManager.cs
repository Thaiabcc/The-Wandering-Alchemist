using UnityEngine;
using UnityEngine.SceneManagement;

public class HotbarManager : MonoBehaviour
{
    public static HotbarManager Instance;

    public HotbarSlot[] hotbarSlots;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ReassignHotbarSlots();
    }

    private void ReassignHotbarSlots()
    {
        hotbarSlots = FindObjectsOfType<HotbarSlot>(true);
        
        // Sắp xếp theo slotID
        System.Array.Sort(hotbarSlots, (a, b) => a.slotID.CompareTo(b.slotID));

        foreach (var slot in hotbarSlots)
            slot.ClearSlot();

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged -= UpdateAllSlotsUI;
            InventoryManager.Instance.OnInventoryChanged += UpdateAllSlotsUI;
        }

        UpdateAllSlotsUI();
    }

    private void Start()
    {
        ReassignHotbarSlots();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) TryUseItem(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) TryUseItem(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) TryUseItem(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) TryUseItem(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) TryUseItem(4);
        if (Input.GetKeyDown(KeyCode.Alpha6)) TryUseItem(5);
        if (Input.GetKeyDown(KeyCode.Alpha7)) TryUseItem(6);
        if (Input.GetKeyDown(KeyCode.Alpha8)) TryUseItem(7);
    }

    private void TryUseItem(int slotID)
    {
        if (slotID < 0 || slotID >= hotbarSlots.Length) return;

        HotbarSlot slot = hotbarSlots[slotID];
        if (slot.assignedItem == null) return;

        int currentAmount = InventoryManager.Instance.GetItemAmount(slot.assignedItem);

        if (currentAmount > 0)
        {
            bool isSuccess = slot.assignedItem.UseItem(PlayerStats.Instance);
            if (isSuccess)
            {
                InventoryManager.Instance.RemoveItem(slot.assignedItem, 1);
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.potionUse);
            }
        }
        else
        {
            slot.ClearSlot();
        }
    }

    public void UpdateSlotUI(int slotID)
    {
        if (slotID < 0 || slotID >= hotbarSlots.Length) return;

        HotbarSlot slot = hotbarSlots[slotID];

        if (slot.assignedItem == null)
        {
            slot.ClearSlot();
            return;
        }

        int totalQuantity = InventoryManager.Instance.GetItemAmount(slot.assignedItem);

        if (totalQuantity <= 0)
        {
            slot.ClearSlot();
        }
        else
        {
            slot.itemIcon.sprite = slot.assignedItem.icon;
            slot.itemIcon.enabled = true;
            slot.quantityText.text = totalQuantity.ToString();
            slot.quantityText.enabled = true;
        }
    }

    public void UpdateAllSlotsUI()
    {
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            UpdateSlotUI(i);
        }
    }

    public void PreventDuplicate(ItemData itemToCheck)
    {
        foreach (var slot in hotbarSlots)
        {
            if (slot.assignedItem == itemToCheck)
            {
                slot.ClearSlot();
            }
        }
    }
}