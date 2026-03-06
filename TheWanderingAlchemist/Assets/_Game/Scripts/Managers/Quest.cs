using UnityEngine;

[System.Serializable]
public class Quest
{
    public QuestData info;
    public int currentAmount;
    public bool isCompleted;

    public Quest(QuestData data)
    {
        info = data;
        currentAmount = 0;
        isCompleted = false;
    }
}