using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NightEnemySpawner : MonoBehaviour
{
    #region NORMAL SPAWN SETTINGS
    [Header("Normal Enemies (18h - 6h)")]
    [SerializeField] private GameObject[] nightEnemyPrefabs;
    [SerializeField] private float normalSpawnInterval = 5f;
    [SerializeField] private float normalSpawnRadius = 4f;
    [SerializeField] private int nightStartHour = 18;
    [SerializeField] private int dayStartHour = 6;
    #endregion

    #region VIP SPAWN SETTINGS
    [Header("Special VIP Enemies (Blood Moon)")]
    [SerializeField] private GameObject[] vipEnemyPrefabs;
    [SerializeField] private int specialNightInterval = 4; // Cứ 4 ngày 1 lần
    
    [Tooltip("Giờ thả VIP (Thường là 0h nửa đêm)")]
    [SerializeField] private int vipSpawnHour = 0; 
    
    [Tooltip("Số lượng VIP thả ra (Ít thôi cho nó nguy hiểm)")]
    [SerializeField] private int vipMaxAmount = 3; 
    
    [Tooltip("Bán kính thả VIP (To hơn quái thường)")]
    [SerializeField] private float vipSpawnRadius = 10f; 
    
    [Tooltip("Khoảng cách thời gian thả từng con VIP (Giây)")]
    [SerializeField] private float vipSpawnDelay = 1f;
    #endregion

    #region PRIVATE VARIABLES
    private Coroutine normalSpawnCoroutine;
    private bool isNightActive = false;
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    
    private bool isSpecialNightNow = false; 
    private bool hasSpawnedVIPsTonight = false; // Chốt an toàn: Đảm bảo chỉ thả 1 đợt VIP mỗi đêm
    #endregion

    #region UNITY LIFECYCLE
    private void Update()
    {
        bool isNight = CheckIfNight();

        // 1. Quản lý chu kỳ Đêm/Ngày chung
        if (isNight)
        {
            if (!isNightActive)
            {
                StartNight();
            }

            // 2. Canh me đúng nửa đêm để thả VIP
            if (isSpecialNightNow && !hasSpawnedVIPsTonight)
            {
                // Ép kiểu giờ hiện tại về số nguyên (vd: 0.5h -> 0h) để kích hoạt
                if (Mathf.FloorToInt(TimeManager.Instance.CurrentHour) == vipSpawnHour)
                {
                    StartCoroutine(SpawnVIPRoutine());
                    hasSpawnedVIPsTonight = true; // Đóng chốt! Đêm nay đã thả VIP rồi, cấm thả nữa
                }
            }
        }
        else if (!isNight && isNightActive)
        {
            EndNight();
        }
    }
    #endregion

    #region TIME CHECK
    private bool CheckIfNight()
    {
        if (TimeManager.Instance == null) return false;

        float currentHour = TimeManager.Instance.CurrentHour;

        if (nightStartHour > dayStartHour)
            return currentHour >= nightStartHour || currentHour < dayStartHour;
        else
            return currentHour >= nightStartHour && currentHour < dayStartHour;
    }

    private bool CheckIfSpecialNight()
    {
        if (TimeManager.Instance == null) return false;

        int currentDay = TimeManager.Instance.CurrentDay;
        if (currentDay == 0) return false; // Bỏ qua ngày 0

        return currentDay % specialNightInterval == 0;
    }
    #endregion

    #region SPAWN LOGIC
    private void StartNight()
    {
        isNightActive = true;
        isSpecialNightNow = CheckIfSpecialNight();
        hasSpawnedVIPsTonight = false; // Reset chốt an toàn cho đêm mới

        // Bắt đầu rặn quái thường
        if (normalSpawnCoroutine == null)
        {
            normalSpawnCoroutine = StartCoroutine(NormalSpawnRoutine());
        }
    }

    private void EndNight()
    {
        isNightActive = false;
        
        if (normalSpawnCoroutine != null)
        {
            StopCoroutine(normalSpawnCoroutine);
            normalSpawnCoroutine = null;
        }
        ClearAllEnemies();
    }

    // ================== LUỒNG 1: QUÁI THƯỜNG ==================
    private IEnumerator NormalSpawnRoutine()
    {
        while (isNightActive)
        {
            SpawnSingleEnemy(nightEnemyPrefabs, normalSpawnRadius);
            yield return new WaitForSeconds(normalSpawnInterval);
        }
    }

    // ================== LUỒNG 2: QUÁI VIP ==================
    private IEnumerator SpawnVIPRoutine()
    {
        Debug.Log("Nửa đêm cmnr! Bắt đầu thả " + vipMaxAmount + " con VIP!");
        
        for (int i = 0; i < vipMaxAmount; i++)
        {
            SpawnSingleEnemy(vipEnemyPrefabs, vipSpawnRadius);
            yield return new WaitForSeconds(vipSpawnDelay); // Rặn từng con một cho mượt, không bị lag giật cục
        }
    }

    // HÀM DÙNG CHUNG ĐỂ ĐẺ QUÁI
    private void SpawnSingleEnemy(GameObject[] enemyPool, float radius)
    {
        if (enemyPool == null || enemyPool.Length == 0) return;

        GameObject randomEnemy = enemyPool[Random.Range(0, enemyPool.Length)];
        
        // Tính toán tọa độ thả (xung quanh Spawner)
        Vector2 randomPos = (Vector2)transform.position + Random.insideUnitCircle * radius;

        GameObject enemyInstance = Instantiate(randomEnemy, randomPos, Quaternion.identity);
        spawnedEnemies.Add(enemyInstance);
    }

    private void ClearAllEnemies()
    {
        for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
        {
            if (spawnedEnemies[i] != null)
            {
                Destroy(spawnedEnemies[i]);
            }
        }
        spawnedEnemies.Clear();
    }
    #endregion
}