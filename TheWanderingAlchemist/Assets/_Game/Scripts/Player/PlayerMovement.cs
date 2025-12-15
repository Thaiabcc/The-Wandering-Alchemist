using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;

    [Header("Thể lực")]
    [SerializeField] private float staminaDrain = 20f;

    [Header("Audio Bước Chân")]
    [SerializeField] private float footstepDelay = 0.4f;
    [SerializeField] private float runStepMultiplier = 0.6f;
    private float footstepTimer = 0;

    private Rigidbody2D rb;
    private GameControls gameControls;
    private Vector2 moveInput;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private float currentSpeed;
    private bool isKnockedBack = false;

    // --- [MỚI] HÀM DỪNG KHẨN CẤP (Gọi từ PlayerStats khi chết) ---
    public void StopMoving()
    {
        // 1. Phanh vật lý lại ngay lập tức
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic; // Dừng hẳn vật lý
        }

        // 2. Tắt animation chạy (để tránh lỗi Moonwalk)
        // Vì Animator của bạn đang dùng Float "Speed", ta set về 0
        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
        }
    }
    // -------------------------------------------------------------

    public void ApplyKnockback(Vector2 direction, float force, float duration)
    {
        isKnockedBack = true;
        rb.velocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);
        StartCoroutine(KnockbackRoutine(duration));
    }

    private IEnumerator KnockbackRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        rb.velocity = Vector2.zero;
        isKnockedBack = false;
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
        // Nếu đang bị đẩy lùi thì không nhận input
        if (isKnockedBack) return;

        moveInput = gameControls.Gameplay.Move.ReadValue<Vector2>();
        currentSpeed = walkSpeed;

        bool isMoving = moveInput.magnitude > 0;
        bool isRunPressed = Keyboard.current != null && Keyboard.current.shiftKey.isPressed;

        // Logic Chạy/Thể lực
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

        // Logic Xoay người
        if (moveInput.x != 0)
        {
            if (moveInput.x > 0) transform.rotation = Quaternion.Euler(0, 0, 0);
            else transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        // Logic Tiếng bước chân
        if (isMoving)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0)
            {
                // Nhớ gán AudioManager vào Scene nhé
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.footstep, 0.8f);

                if (currentSpeed == runSpeed) footstepTimer = footstepDelay * runStepMultiplier;
                else footstepTimer = footstepDelay;
            }
        }
        else
        {
            footstepTimer = 0;
        }
    }

    private void FixedUpdate()
    {
        if (isKnockedBack) return;
        rb.velocity = moveInput * currentSpeed;
    }
}