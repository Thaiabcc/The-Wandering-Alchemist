using UnityEngine;

public class ChestInteract : MonoBehaviour
{
    [Header("Save Settings")]
    public string chestID = "Chest_01";

    private Animator anim;
    private bool isPlayerNearby = false;
    private bool isOpened = false;

    [Header("Reward")]
    public GameObject goldPrefab; 
    public int goldAmount = 1;    
    public float popForce = 3f;  

    void Start()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.openedChests.Contains(chestID))
        {
            Destroy(gameObject);
            return; 
        }

        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E) && !isOpened)
        {
            OpenChest();
        }
    }

    void OpenChest()
    {
        isOpened = true;
        
        if (SaveManager.Instance != null && !SaveManager.Instance.openedChests.Contains(chestID))
        {
            SaveManager.Instance.openedChests.Add(chestID);
        }

        if (anim != null)
        {
            anim.SetTrigger("Open");
        }
        
        if (goldPrefab != null)
        {
            SpawnLoot();
        }
        
        Destroy(gameObject, 0.5f);
    }

    void SpawnLoot()
    {
        for (int i = 0; i < goldAmount; i++)
        {
            GameObject loot = Instantiate(goldPrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = loot.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                Vector2 dropDirection = Random.insideUnitCircle.normalized;
                rb.AddForce(dropDirection * popForce, ForceMode2D.Impulse);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }
}