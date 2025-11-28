using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private GameControls gameControls;
    private Vector2 moveInput;
    private SpriteRenderer spriteRenderer;
    private Animator animator; // <--- [BỔ SUNG 1] KHAI BÁO ANIMATOR

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>(); // <--- [BỔ SUNG 2] LẤY COMPONENT ANIMATOR
        gameControls = new GameControls();

        // Kiểm tra lỗi nếu thiếu component quan trọng
        if (animator == null) Debug.LogError("Animator Component is missing on Player!");
    }

    private void OnEnable()
    {
        gameControls.Enable();
    }

    private void OnDisable()
    {
        gameControls.Disable();
    }

    private void Update()
    {
        moveInput = gameControls.Gameplay.Move.ReadValue<Vector2>();

        // 1. Tính toán Tốc độ thực tế (Magnitude)
        float currentSpeed = moveInput.magnitude;
         
        // 2. Gửi giá trị Tốc độ vào Animator
        if (animator != null)
        {
            animator.SetFloat("Speed", currentSpeed); // <--- [BỔ SUNG 3] GỬI TỐC ĐỘ VÀO PARAMETER
        }

        // 3. Logic Lật ảnh (Flip Sprite)
        if (moveInput.x != 0 && spriteRenderer != null) // <--- [BỔ SUNG 4] Dùng logic đơn giản hơn
        {
            // Kiểm tra xem nhân vật có đang di chuyển sang trái không
            if (moveInput.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            else // moveInput.x > 0
            {
                spriteRenderer.flipX = false;
            }
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = moveInput * moveSpeed;
    }
}