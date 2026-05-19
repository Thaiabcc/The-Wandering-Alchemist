using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "WorldState", menuName = "ScriptableObjects/WorldState")]
public class WorldState : ScriptableObject
{
    public List<string> collectedItems = new List<string>();

    public void RecordPickup(string id)
    {
        if (!string.IsNullOrEmpty(id) && !collectedItems.Contains(id))
            collectedItems.Add(id);
    }

    public bool IsCollected(string id)
    {
        return collectedItems.Contains(id);
    }

    public void ResetState()
    {
        collectedItems.Clear();
    }
}