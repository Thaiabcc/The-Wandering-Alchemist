using UnityEngine;

public class ChestInteract : MonoBehaviour
{
    private Animator anim;
    private bool isPlayerNearby = false;
    private bool isOpened = false;

    void Start()
    {
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
        anim.SetTrigger("Open");
        Debug.Log("Rương đã mở!");

        // ---------------------------------------------------------
        // PHẦN DÀNH CHO VẬT PHẨM (SAU NÀY BẠN SẼ VIẾT VÀO ĐÂY)
        // Ví dụ: Instantiate(itemPrefab, transform.position, Quaternion.identity);
        // ---------------------------------------------------------

        // Xóa rương sau 0.5 giây (đủ để thấy animation mở rương)
        Destroy(gameObject, 0.5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log("Nhấn E để mở rương");
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