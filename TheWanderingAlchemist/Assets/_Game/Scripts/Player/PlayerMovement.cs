using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; // Bắt buộc để dùng Keyboard.current

public class PlayerMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;

    // --- [MỚI] Dòng này sẽ hiện ra trong Inspector sau khi bạn lưu code ---
    [Header("Thể lực")]
    [SerializeField] private float staminaDrain = 20f; // Tốc độ trừ (20 điểm/giây)
    // ---------------------------------------------------------------------

    private Rigidbody2D rb;
    private GameControls gameControls;
    private Vector2 moveInput;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private float currentSpeed;
    private bool isKnockedBack = false;
    public void ApplyKnockback(Vector2 direction, float force, float duration)
    {
        // 1. Đánh dấu đang bị đẩy -> Ngắt điều khiển
        isKnockedBack = true;

        // 2. Reset vận tốc cũ để lực đẩy có tác dụng 100%
        rb.velocity = Vector2.zero;

        // 3. Đẩy! (ForceMode2D.Impulse là lực tức thì)
        rb.AddForce(direction * force, ForceMode2D.Impulse);

        // 4. Chờ một chút rồi trả lại quyền điều khiển
        StartCoroutine(KnockbackRoutine(duration));
    }

    private IEnumerator KnockbackRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);

        // Hết thời gian choáng -> Reset vận tốc về 0 để không trượt tiếp
        rb.velocity = Vector2.zero;
        isKnockedBack = false; // Trả lại quyền điều khiển
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        gameControls = new GameControls();

        if (animator == null) Debug.LogError("Thiếu Animator!");
    }

    private void OnEnable() => gameControls.Enable();
    private void OnDisable() => gameControls.Disable();

    private void Update()
    {
        moveInput = gameControls.Gameplay.Move.ReadValue<Vector2>();

        // Mặc định là đi bộ
        currentSpeed = walkSpeed;

        // 1. Kiểm tra có đang di chuyển không
        bool isMoving = moveInput.magnitude > 0;

        // 2. Kiểm tra có giữ phím Shift không
        bool isRunPressed = Keyboard.current != null && Keyboard.current.shiftKey.isPressed;

        // --- [LOGIC QUAN TRỌNG NHẤT] ---
        if (isMoving && isRunPressed)
        {
            // Hỏi PlayerStats xem còn sức không?
            // Nếu còn (TryConsumeStamina trả về true) -> Cho phép chạy nhanh
            if (PlayerStats.Instance.TryConsumeStamina(staminaDrain * Time.deltaTime))
            {
                currentSpeed = runSpeed;
            }
            else
            {
                // Hết hơi -> Về đi bộ dù vẫn giữ Shift
                currentSpeed = walkSpeed;
            }
        }
        else
        {
            // Nếu đứng im hoặc nhả Shift -> Hồi phục
            PlayerStats.Instance.RegenerateStamina(PlayerStats.Instance.staminaRegenRate * Time.deltaTime);
        }
        // -------------------------------

        // Gửi speed vào Animator
        if (animator != null) animator.SetFloat("Speed", moveInput.magnitude);

        // Xoay nhân vật
        if (moveInput.x != 0)
        {
            if (moveInput.x > 0)
            {
                // Quay mặt sang phải (Góc 0 độ)
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                // Quay mặt sang trái (Góc 180 độ quanh trục Y)
                // Nó sẽ xoay nhân vật úp mặt vào trong, tạo hiệu ứng lật gương hoàn hảo
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }

    private void FixedUpdate()
    {
        if (isKnockedBack) return;

        // Logic di chuyển cũ của bạn
        rb.velocity = moveInput * currentSpeed;
    }
}