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
    // --- MỚI 1: Thêm biến chỉnh âm thanh bước chân ---
    [Header("Audio Bước Chân")]
    [SerializeField] private float footstepDelay = 0.4f; // Bao lâu thì kêu 1 lần (0.4s là vừa đi bộ)
    [SerializeField] private float runStepMultiplier = 0.6f; // Chạy thì tiếng dồn dập hơn (nhân với 0.6)
    private float footstepTimer = 0;

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
        currentSpeed = walkSpeed;

        bool isMoving = moveInput.magnitude > 0;
        bool isRunPressed = Keyboard.current != null && Keyboard.current.shiftKey.isPressed;

        // Logic Chạy/Thể lực cũ
        if (isMoving && isRunPressed)
        {
            if (PlayerStats.Instance.TryConsumeStamina(staminaDrain * Time.deltaTime))
            {
                currentSpeed = runSpeed;
            }
            else
            {
                currentSpeed = walkSpeed;
            }
        }
        else
        {
            PlayerStats.Instance.RegenerateStamina(PlayerStats.Instance.staminaRegenRate * Time.deltaTime);
        }

        if (animator != null) animator.SetFloat("Speed", moveInput.magnitude);

        // Logic Xoay người cũ
        if (moveInput.x != 0)
        {
            if (moveInput.x > 0) transform.rotation = Quaternion.Euler(0, 0, 0);
            else transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        // --- MỚI 2: LOGIC PHÁT TIẾNG BƯỚC CHÂN ---
        if (isMoving)
        {
            // Trừ dần thời gian
            footstepTimer -= Time.deltaTime;

            if (footstepTimer <= 0)
            {
                // Phát tiếng (Volume nhỏ 0.3f thôi cho đỡ đau đầu)
                AudioManager.Instance.PlaySFX(AudioManager.Instance.footstep, 0.8f);

                // Reset đồng hồ
                // Nếu đang chạy nhanh -> Thời gian delay ngắn lại (bước nhanh hơn)
                if (currentSpeed == runSpeed)
                {
                    footstepTimer = footstepDelay * runStepMultiplier;
                }
                else
                {
                    footstepTimer = footstepDelay;
                }
            }
        }
        else
        {
            // Nếu đứng im -> Reset timer về 0 để vừa nhích chân là kêu ngay
            footstepTimer = 0;
        }
        // ------------------------------------------
    }

    private void FixedUpdate()
    {
        if (isKnockedBack) return;
        rb.velocity = moveInput * currentSpeed;
    }
}