using UnityEngine;
using System.Collections; 

public class GoldPickup : MonoBehaviour
{
    public int goldValue = 100;
    private bool isCollectable = false;

    void Start()
    {
        StartCoroutine(EnablePickupAfterDelay());
    }

    IEnumerator EnablePickupAfterDelay()
    {
        isCollectable = false;
        yield return new WaitForSeconds(0.5f); 
        isCollectable = true; 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCollectable == false) return;

        if (collision.CompareTag("Player"))
        {
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.UpdateGold(goldValue);
                Destroy(gameObject);
            }
        }
    }
}