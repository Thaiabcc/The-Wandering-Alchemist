using UnityEngine;

public class BossPersistence : MonoBehaviour
{
    [SerializeField] private string bossID;

    private void Start()
    {
        if (SaveManager.Instance != null &&
            SaveManager.Instance.defeatedBosses.Contains(bossID))
        {
            Destroy(gameObject);
        }
    }

    public void MarkAsDefeated()
    {
        if (SaveManager.Instance == null) return;

        if (!SaveManager.Instance.defeatedBosses.Contains(bossID))
        {
            SaveManager.Instance.defeatedBosses.Add(bossID);
        }
    }
}