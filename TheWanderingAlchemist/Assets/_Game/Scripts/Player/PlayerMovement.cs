using UnityEngine;
using UnityEngine.InputSystem; // Bắt buộc để dùng Keyboard.current

public class PlayerMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float walkSpeed = 5f; // Đổi tên từ moveSpeed thành walkSpeed cho rõ
    [SerializeField] private float runSpeed = 8f;  // <--- [BỔ SUNG 1] Tốc độ chạy

    private Rigidbody2D rb;
    private GameControls gameControls;
    private Vector2 moveInput;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    // Biến lưu tốc độ hiện tại để dùng cho FixedUpdate
    private float currentSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        gameControls = new GameControls();

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
        // 1. Đọc hướng di chuyển
        moveInput = gameControls.Gameplay.Move.ReadValue<Vector2>();

        // 2. [BỔ SUNG 2] Kiểm tra phím Shift
        // Mặc định là tốc độ đi bộ
        currentSpeed = walkSpeed;

        // Nếu bàn phím tồn tại VÀ phím Shift đang được giữ
        if (Keyboard.current != null && Keyboard.current.shiftKey.isPressed)
        {
            currentSpeed = runSpeed;
        }

        // 3. Gửi thông số vào Animator
        // Lưu ý: moveInput.magnitude chỉ trả về 0 hoặc 1 (đứng im hoặc di chuyển)
        // Nếu bạn muốn Animator phân biệt đi bộ/chạy, bạn có thể cần logic khác.
        // Hiện tại giữ nguyên logic cũ của bạn.
        if (animator != null)
        {
            animator.SetFloat("Speed", moveInput.magnitude);
        }

        // 4. Logic Lật ảnh (Flip Sprite)
        if (moveInput.x != 0 && spriteRenderer != null)
        {
            if (moveInput.x < 0) spriteRenderer.flipX = true;
            else spriteRenderer.flipX = false;
        }
    }

    private void FixedUpdate()
    {
        // 5. [BỔ SUNG 3] Di chuyển với tốc độ đã tính toán (currentSpeed)
        rb.velocity = moveInput * currentSpeed;
    }
}