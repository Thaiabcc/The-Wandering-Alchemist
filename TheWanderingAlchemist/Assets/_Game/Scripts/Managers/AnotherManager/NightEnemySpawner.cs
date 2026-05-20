using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NightEnemySpawner : MonoBehaviour
{
    #region SETTINGS
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] nightEnemyPrefabs;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private float spawnRadius = 4f;

    [Header("Time Constraints")]
    [SerializeField] private int nightStartHour = 18;
    [SerializeField] private int dayStartHour = 6;
    #endregion

    #region PRIVATE VARIABLES
    private Coroutine spawnCoroutine;
    private bool isSpawningActive = false;
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    #endregion

    #region UNITY LIFECYCLE
    private void Update()
    {
        bool isNight = CheckIfNight();

        if (isNight && !isSpawningActive)
        {
            StartSpawning();
        }
        else if (!isNight && isSpawningActive)
        {
            StopSpawning();
        }
    }
    #endregion

    #region TIME CHECK
    private bool CheckIfNight()
    {
        if (TimeManager.Instance == null) return false;

        float currentHour = TimeManager.Instance.CurrentHour;

        if (nightStartHour > dayStartHour)
        {
            return currentHour >= nightStartHour || currentHour < dayStartHour;
        }
        else
        {
            return currentHour >= nightStartHour && currentHour < dayStartHour;
        }
    }
    #endregion

    #region SPAWN LOGIC
    private void StartSpawning()
    {
        isSpawningActive = true;
        if (spawnCoroutine == null)
        {
            spawnCoroutine = StartCoroutine(SpawnRoutine());
        }
    }

    private void StopSpawning()
    {
        isSpawningActive = false;
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        ClearNightEnemies();
    }

    private IEnumerator SpawnRoutine()
    {
        while (isSpawningActive)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        if (nightEnemyPrefabs == null || nightEnemyPrefabs.Length == 0) return;

        GameObject randomEnemy = nightEnemyPrefabs[Random.Range(0, nightEnemyPrefabs.Length)];
        Vector2 randomPos = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;

        GameObject enemyInstance = Instantiate(randomEnemy, randomPos, Quaternion.identity);
        spawnedEnemies.Add(enemyInstance);
    }

    private void ClearNightEnemies()
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