using System.Collections.Generic;

// Item
[System.Serializable]
public class ItemSaveData
{
    public string itemID;    
    public int quantity;     
    public int slotIndex;    
}

// Quest
[System.Serializable]
public class QuestSaveData
{
    public string questName;   
    public int currentAmount;  
    public bool isCompleted;   
    public bool isTracked;     
}

// Buff 
[System.Serializable]
public class BuffSaveData
{
    public string buffID;
    public float remainingDuration;
}

// Game
[System.Serializable]
public class GameData
{
    // Player Stat
    public int playerGold;
    public float playerHealth;
    public float playerStamina;
    public float playerShield;
    public float playerBaseDamage;
    public bool isPoisoned;
    public float[] playerPosition; 
    public string currentSceneName;
    
    // Day & Weather
    public int currentDay;
    public double accumSeconds;       
    public string currentWeatherState; 
    
    // Items in Inven
    public List<ItemSaveData> inventoryItems = new List<ItemSaveData>(); 
    public List<ItemSaveData> hotbarItems = new List<ItemSaveData>();    
    public List<string> collectedUniqueIDs = new List<string>(); 
    
    // Quest Stat
    public List<string> completedQuestNames = new List<string>();     
    public List<QuestSaveData> activeQuestDatas = new List<QuestSaveData>(); 
    
    // Buff Potion 
    public List<BuffSaveData> activeBuffDatas = new List<BuffSaveData>();
    
    // Puzzle
    public List<string> solvedPuzzleIDs = new List<string>();
    
    // Chest
    public List<string> openedChestIDs = new List<string>();

}