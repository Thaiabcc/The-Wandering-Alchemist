using System.Collections;
using UnityEngine;

public class PlayerDeflect : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Kéo cái Gameobject con chứa Collider phản đòn vào đây")]
    [SerializeField] private GameObject deflectHitbox;
    [SerializeField] private Animator animator;

    [Header("Settings")]
    [SerializeField] private float activeTime = 0.2f;
    [SerializeField] private float cooldown = 1f;
    [SerializeField] private float offsetDistance = 1.0f; // Khoảng cách từ người đến tấm khiên

    [Header("Stamina Cost")]
    [SerializeField] private float staminaCost = 20f;

    private bool isDeflecting;
    private bool canDeflect = true;

    private void Awake()
    {
        if (deflectHitbox) deflectHitbox.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire2") && canDeflect && !isDeflecting)
        {
            if (PlayerStats.Instance.TryConsumeStamina(staminaCost))
            {
                StartCoroutine(DeflectRoutine());
            }
            else
            {
                Debug.Log("Not enough stamina to deflict !");
            }
        }
    }

    private IEnumerator DeflectRoutine()
    {
        isDeflecting = true;
        canDeflect = false;

        // --- [MỚI] XOAY HITBOX THEO HƯỚNG CHUỘT ---
        RotateHitboxToMouse();

        // 1. Chạy Animation
        if (animator) animator.SetTrigger("Deflect");

        // 2. BẬT vùng va chạm
        if (deflectHitbox) deflectHitbox.SetActive(true);

        // 3. Giữ trong thời gian active
        yield return new WaitForSeconds(activeTime);

        // 4. TẮT vùng va chạm
        if (deflectHitbox) deflectHitbox.SetActive(false);
        isDeflecting = false;

        // 5. Hồi chiêu
        yield return new WaitForSeconds(cooldown - activeTime);
        canDeflect = true;
    }

    // --- HÀM MỚI ĐỂ XOAY VÀ ĐẶT VỊ TRÍ KHIÊN ---
    private void RotateHitboxToMouse()
    {
        if (deflectHitbox == null) return;

        // 1. Lấy vị trí chuột trong thế giới game
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0; // Đảm bảo z = 0 cho 2D

        // 2. Tính hướng từ Player -> Chuột
        Vector3 direction = (mousePos - transform.position).normalized;

        // 3. Đặt vị trí Hitbox (cách người 1 khoảng offsetDistance)
        deflectHitbox.transform.position = transform.position + (direction * offsetDistance);

        // 4. Xoay Hitbox để mặt của nó hướng về phía chuột
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        deflectHitbox.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}