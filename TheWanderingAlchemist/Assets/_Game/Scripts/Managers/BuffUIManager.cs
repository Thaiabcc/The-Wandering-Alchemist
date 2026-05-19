using UnityEngine;
using System.Collections.Generic;

public class BuffUIManager : MonoBehaviour
{
    public static BuffUIManager Instance { get; private set; }

    [SerializeField] private GameObject buffIconPrefab;
    [SerializeField] private Transform buffContainer;

    private Dictionary<string, GameObject> activeBuffs = new Dictionary<string, GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    public void AddBuff(string buffId, Sprite icon, float duration)
    {
        RemoveBuff(buffId);

        if (buffIconPrefab == null || buffContainer == null || icon == null) return;

        GameObject newIcon = Instantiate(buffIconPrefab, buffContainer);
        BuffIconUI iconScript = newIcon.GetComponent<BuffIconUI>();
        
        if (iconScript != null)
        {
            iconScript.Setup(buffId, icon, duration, this);
        }
        
        activeBuffs.Add(buffId, newIcon);
    }

    public void RemoveBuff(string buffId)
    {
        if (activeBuffs.ContainsKey(buffId))
        {
            if (activeBuffs[buffId] != null) 
            {
                Destroy(activeBuffs[buffId]);
            }
            activeBuffs.Remove(buffId);
        }
    }
}