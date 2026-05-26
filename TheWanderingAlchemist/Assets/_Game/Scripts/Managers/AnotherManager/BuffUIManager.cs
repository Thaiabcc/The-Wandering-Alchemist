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

        activeBuffs[buffId] = newIcon;
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

    public void RemoveAllBuffs()
    {
        foreach (var kvp in new Dictionary<string, GameObject>(activeBuffs))
        {
            RemoveBuff(kvp.Key);
        }
        activeBuffs.Clear();
    }

    public Dictionary<string, GameObject> GetActiveBuffs()
    {
        return activeBuffs;
    }

    // ====================== HÀM LẤY SPRITE (ĐÃ SỬA) ======================
    public Sprite GetBuffSprite(string buffID, List<ItemData> itemDatabase)
    {
        if (itemDatabase == null) return null;

        ItemData item = itemDatabase.Find(i =>
        {
            if (i == null) return false;
            if (buffID == "DamageBuff" && i.damageBuffAmount > 0) return true;
            if (buffID == "MaxHealthBuff" && i.maxHealthPercentBonus > 0) return true;
            if (buffID == "Shield" && i.shieldAmount > 0) return true;
            if (buffID == "PoisonImmunity" && i.isPoisonImmunityPotion) return true;
            if (i.id.Trim() == buffID.Trim()) return true;
            return false;
        });

        if (item != null)
        {
            return item.buffIcon != null ? item.buffIcon : item.icon;
        }

        return null;
    }
}