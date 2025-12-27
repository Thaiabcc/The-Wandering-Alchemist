using UnityEngine;
using System.Collections; // Cần cái này để dùng Coroutine

public class GoldPickup : MonoBehaviour
{
    public int goldValue = 100;

    // Biến này để khóa, mặc định là false (chưa được nhặt)
    private bool isCollectable = false;

    void Start()
    {
        // Vừa sinh ra, gọi hàm đếm ngược để mở khóa
        StartCoroutine(EnablePickupAfterDelay());
    }

    // Hàm đếm ngược: Chờ 0.5s rồi mới cho phép nhặt
    IEnumerator EnablePickupAfterDelay()
    {
        isCollectable = false;
        yield return new WaitForSeconds(0.5f); // Thời gian vàng bay lơ lửng
        isCollectable = true; // Hết giờ -> Cho phép nhặt
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Nếu chưa hết thời gian chờ -> Dừng lại ngay, không làm gì cả
        if (isCollectable == false) return;

        // 2. Logic nhặt bình thường
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