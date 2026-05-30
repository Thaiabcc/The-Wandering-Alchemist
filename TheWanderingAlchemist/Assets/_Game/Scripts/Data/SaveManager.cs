using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Reflection;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private string saveFilePath;

    [Header("Database Quest")]
    [SerializeField] private List<QuestData> allQuestDatabase = new List<QuestData>();

    [Header("Database Item")]
    [SerializeField] private List<ItemData> allItemDatabase = new List<ItemData>();

    [Header("Puzzle State")] 
    public List<string> solvedPuzzles = new List<string>();

    [Header("Chest State")] 
    public List<string> openedChests = new List<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        saveFilePath = Path.Combine(Application.persistentDataPath, "gamesave.dat");
    }

    public void SaveGame()
    {
        GameData data = new GameData();

        if (PlayerStats.Instance != null)
        {
            data.playerHealth = PlayerStats.Instance.currentHealth;
            data.playerStamina = PlayerStats.Instance.currentStamina;
            data.playerShield = PlayerStats.Instance.currentShield;
            data.playerBaseDamage = PlayerStats.Instance.baseDamage;
            data.isPoisoned = PlayerStats.Instance.isPoisoned;

            Vector3 pos = PlayerStats.Instance.transform.position;
            data.playerPosition = new float[] { pos.x, pos.y, pos.z };
        }

        data.currentSceneName = SceneManager.GetActiveScene().name;

        if (TimeManager.Instance != null)
        {
            var field = typeof(TimeManager).GetField("accumSeconds", BindingFlags.NonPublic | BindingFlags.Instance);
            double savedSeconds = field != null ? (double)field.GetValue(TimeManager.Instance) : 0;
            data.currentDay = TimeManager.Instance.CurrentDay;
            data.accumSeconds = savedSeconds;
        }

        if (WeatherManager.Instance != null)
        {
            data.currentWeatherState = WeatherManager.Instance.CurrentWeather.ToString();
        }

        if (QuestManager.Instance != null)
        {
            data.completedQuestNames = new List<string>(QuestManager.Instance.completedQuestNames);

            foreach (Quest q in QuestManager.Instance.activeQuests)
            {
                QuestSaveData qData = new QuestSaveData();
                qData.questName = q.info.questName;
                qData.currentAmount = q.currentAmount;
                qData.isCompleted = q.isCompleted;
                qData.isTracked = (QuestManager.Instance.trackedQuest == q);
                data.activeQuestDatas.Add(qData);
            }
        }

        if (HotbarManager.Instance != null && HotbarManager.Instance.hotbarSlots != null)
        {
            data.hotbarItems = new List<ItemSaveData>();

            for (int i = 0; i < HotbarManager.Instance.hotbarSlots.Length; i++)
            {
                var slot = HotbarManager.Instance.hotbarSlots[i];

                if (slot != null && slot.assignedItem != null)
                {
                    ItemSaveData hData = new ItemSaveData();
                    hData.itemID = slot.assignedItem.id;
                    hData.slotIndex = slot.slotID;
                    hData.quantity = 0;
                    data.hotbarItems.Add(hData);
                }
            }
        }

        if (InventoryManager.Instance != null)
        {
            data.playerGold = InventoryManager.Instance.currentGold;
            data.inventoryItems = new List<ItemSaveData>();

            for (int i = 0; i < InventoryManager.Instance.inventory.Count; i++)
            {
                var slot = InventoryManager.Instance.inventory[i];

                if (slot != null && slot.item != null && slot.quantity > 0)
                {
                    ItemSaveData iData = new ItemSaveData();
                    iData.itemID = slot.item.id;
                    iData.quantity = slot.quantity;
                    iData.slotIndex = i;
                    data.inventoryItems.Add(iData);
                }
            }
        }

        if (BuffUIManager.Instance != null)
        {
            data.activeBuffDatas = new List<BuffSaveData>();
            var activeBuffs = BuffUIManager.Instance.GetActiveBuffs();

            foreach (var kvp in activeBuffs)
            {
                if (kvp.Value != null)
                {
                    BuffIconUI iconScript = kvp.Value.GetComponent<BuffIconUI>();

                    if (iconScript != null)
                    {
                        var timerField = typeof(BuffIconUI).GetField("timer", BindingFlags.NonPublic | BindingFlags.Instance);
                        float remainingTime = timerField != null ? (float)timerField.GetValue(iconScript) : 0f;

                        BuffSaveData bData = new BuffSaveData();
                        bData.buffID = kvp.Key;
                        bData.remainingDuration = remainingTime;
                        data.activeBuffDatas.Add(bData);
                    }
                }
            }
        }

        data.solvedPuzzleIDs = new List<string>(solvedPuzzles);
        data.openedChestIDs = new List<string>(openedChests); 

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
    }

    public void LoadGame()
    {
        if (!File.Exists(saveFilePath))
        {
            return;
        }

        string json = File.ReadAllText(saveFilePath);
        GameData data = JsonUtility.FromJson<GameData>(json);

        if (PlayerStats.Instance != null)
            PlayerStats.Instance.LoadDataFromSave(data);

        if (TimeManager.Instance != null)
            TimeManager.Instance.LoadTimeData(data.currentDay, data.accumSeconds);

        if (WeatherManager.Instance != null)
            WeatherManager.Instance.LoadWeatherData(data.currentWeatherState);

        if (QuestManager.Instance != null)
            QuestManager.Instance.LoadQuestData(data.completedQuestNames, data.activeQuestDatas, allQuestDatabase);

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.currentGold = data.playerGold;
            InventoryManager.Instance.inventory.Clear();

            foreach (ItemSaveData iData in data.inventoryItems)
            {
                string cleanID = iData.itemID.Trim();
                ItemData originalItem = allItemDatabase.Find(item => item.id.Trim() == cleanID);

                if (originalItem != null)
                {
                    InventorySlot loadedSlot = new InventorySlot(originalItem, iData.quantity);
                    InventoryManager.Instance.inventory.Add(loadedSlot);
                }
            }

            var sortMethod = typeof(InventoryManager).GetMethod("SortInventory", BindingFlags.Public | BindingFlags.Instance);

            if (sortMethod != null)
                sortMethod.Invoke(InventoryManager.Instance, null);
        }

        if (HotbarManager.Instance != null && HotbarManager.Instance.hotbarSlots != null)
        {
            foreach (var slot in HotbarManager.Instance.hotbarSlots)
                if (slot != null)
                    slot.ClearSlot();

            foreach (ItemSaveData hData in data.hotbarItems)
            {
                string cleanID = hData.itemID.Trim();
                ItemData originalItem = allItemDatabase.Find(item => item.id.Trim() == cleanID);

                if (originalItem != null && hData.slotIndex >= 0 && hData.slotIndex < HotbarManager.Instance.hotbarSlots.Length)
                {
                    HotbarManager.Instance.hotbarSlots[hData.slotIndex].assignedItem = originalItem;
                }
            }

            HotbarManager.Instance.UpdateAllSlotsUI();
        }

        if (BuffUIManager.Instance != null && data.activeBuffDatas != null)
        {
            BuffUIManager.Instance.RemoveAllBuffs();

            bool shieldLoaded = false;

            foreach (BuffSaveData bData in data.activeBuffDatas)
            {
                Sprite buffSprite = BuffUIManager.Instance.GetBuffSprite(bData.buffID, allItemDatabase);

                if (buffSprite == null)
                {
                    continue;
                }

                if (bData.buffID == "Shield")
                {
                    if (!shieldLoaded && PlayerStats.Instance != null && data.playerShield > 0)
                    {
                        PlayerStats.Instance.AddShield(data.playerShield, buffSprite);
                        shieldLoaded = true;
                    }
                    continue;
                }

                if (bData.remainingDuration <= 0f)
                    continue;

                if (bData.buffID == "DamageBuff" && PlayerStats.Instance != null)
                {
                    ItemData potion = allItemDatabase.Find(item => item.damageBuffAmount > 0);

                    if (potion != null)
                    {
                        PlayerStats.Instance.ApplyBuffDamage(
                            potion.damageBuffAmount,
                            bData.remainingDuration,
                            buffSprite);
                    }
                    continue;
                }

                if (bData.buffID == "MaxHealthBuff" && PlayerStats.Instance != null)
                {
                    ItemData potion = allItemDatabase.Find(item => item.maxHealthPercentBonus > 0);

                    if (potion != null)
                    {
                        PlayerStats.Instance.ApplyTemporaryMaxHealth(
                            potion.maxHealthPercentBonus,
                            bData.remainingDuration,
                            buffSprite);
                    }
                    continue;
                }

                BuffUIManager.Instance.AddBuff(bData.buffID, buffSprite, bData.remainingDuration);
            }
        }

        if (data.solvedPuzzleIDs != null)
        {
            solvedPuzzles = new List<string>(data.solvedPuzzleIDs);
        }
        else
        {
            solvedPuzzles = new List<string>();
        }

        if (data.openedChestIDs != null)
        {
            openedChests = new List<string>(data.openedChestIDs);
        }
        else
        {
            openedChests = new List<string>();
        }
    }

    public bool HasSaveFile()
    {
        return File.Exists(saveFilePath);
    }

    public string GetSavedSceneName()
    {
        if(!File.Exists(saveFilePath)) return "Gameplay";
        
        string json = File.ReadAllText(saveFilePath);
        GameData data = JsonUtility.FromJson<GameData>(json);

        if (string.IsNullOrEmpty(data.currentSceneName)) return "Gameplay";

        return data.currentSceneName;
    }
}