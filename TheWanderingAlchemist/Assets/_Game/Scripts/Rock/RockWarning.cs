using UnityEngine;

public class RockWarning : MonoBehaviour
{
    [Header("--- Setting ---")]
    public GameObject[] rockPrefabs;

    public float warningDuration = 0.8f;
    public float spawnHeight = 10f;

    private float timer;

    void Start()
    {
        timer = warningDuration;
        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        float scale = 1f - (timer / warningDuration);
        transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(1.5f, 0.5f, 1f), scale);

        if (timer <= 0)
        {
            SpawnRock();
            Destroy(gameObject);
        }
    }

    void SpawnRock()
    {
        if (rockPrefabs != null && rockPrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, rockPrefabs.Length);
            GameObject rockToSpawn = rockPrefabs[randomIndex];

            if (rockToSpawn != null)
            {
                Vector3 dropPos = transform.position + Vector3.up * spawnHeight;
                Instantiate(rockToSpawn, dropPos, Quaternion.identity);
            }
        }
    }
}