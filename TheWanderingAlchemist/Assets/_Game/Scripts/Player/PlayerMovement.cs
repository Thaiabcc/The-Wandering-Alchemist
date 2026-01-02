using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    // ==============================
    // Movement Settings
    // ==============================
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;

    // ==============================
    // Stamina
    // ==============================
    [Header("Stamina")]
    [SerializeField] private float staminaDrain = 20f;

    // ==============================
    // [MỚI] Audio Settings (Dành cho file loop dài)
    // ==============================
    [Header("Footstep Loop Audio")]
    [SerializeField] private AudioSource footstepSource; // Kéo cái AudioSource trên người Player vào đây
    [SerializeField] private float runPitchMultiplier = 1.5f; // Chạy thì tiếng nhanh gấp 1.5 lần

    // ==============================
    // Components
    // ==============================
    private Rigidbody2D rb;
    private Animator animator;
    private GameControls controls;

    // ==============================
    // State
    // ==============================
    private Vector2 moveInput;
    private float currentSpeed;
    private bool isKnockedBack;

    // ==============================
    // Unity Lifecycle
    // ==============================
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Tự động lấy AudioSource nếu quên kéo (miễn là đã Add Component)
        if (footstepSource == null) footstepSource = GetComponent<AudioSource>();

        controls = new GameControls();
    }

    private void OnEnable()
    {
        controls.Enable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        controls.Disable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        TryTeleportToSpawn();
    }

    // ==============================
    // Scene / Spawn
    // ==============================
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryTeleportToSpawn();
    }

    private void TryTeleportToSpawn()
    {
        StartCoroutine(TeleportRoutine());
    }

    private IEnumerator TeleportRoutine()
    {
        yield return null;
        if (GameManager.Instance == null) yield break;
        if (!GameManager.Instance.nextSpawnPosition.HasValue) yield break;

        rb.simulated = false;
        transform.position = GameManager.Instance.nextSpawnPosition.Value;
        GameManager.Instance.nextSpawnPosition = null;
        yield return null;
        rb.simulated = true;
    }

    // ==============================
    // Update Loop
    // ==============================
    private void Update()
    {
        if (isKnockedBack) return;

        ReadInput();
        HandleStamina();
        UpdateAnimation();

        // 👇 GỌI HÀM XỬ LÝ ÂM THANH MỚI
        HandleFootstepAudioLoop();
    }

    private void FixedUpdate()
    {
        if (isKnockedBack) return;
        rb.velocity = moveInput * currentSpeed;
    }

    // ==============================
    // Input & Movement
    // ==============================
    private void ReadInput()
    {
        moveInput = controls.Gameplay.Move.ReadValue<Vector2>();
        currentSpeed = walkSpeed;

        bool isRunning = Keyboard.current != null && Keyboard.current.shiftKey.isPressed;
        bool isMoving = moveInput.sqrMagnitude > 0;

        if (isMoving && isRunning)
        {
            if (PlayerStats.Instance != null && PlayerStats.Instance.TryConsumeStamina(staminaDrain * Time.deltaTime))
            {
                currentSpeed = runSpeed;
            }
        }
    }

    private void HandleStamina()
    {
        if (moveInput.sqrMagnitude == 0 && PlayerStats.Instance != null)
        {
            PlayerStats.Instance.RegenerateStamina(
                PlayerStats.Instance.staminaRegenRate * Time.deltaTime
            );
        }
    }

    private void UpdateAnimation()
    {
        if (animator != null)
            animator.SetFloat("Speed", moveInput.magnitude);
    }

    // ==============================
    // 👇 [QUAN TRỌNG] LOGIC ÂM THANH MỚI (CHO FILE LOOP)
    // ==============================
    private void HandleFootstepAudioLoop()
    {
        if (footstepSource == null) return;

        // 1. Kiểm tra xem có đang di chuyển không
        bool isMoving = moveInput.sqrMagnitude > 0;

        if (isMoving)
        {
            // Nếu đang đi mà loa chưa bật -> BẬT LOA
            if (!footstepSource.isPlaying)
            {
                footstepSource.Play();
            }

            // 2. Điều chỉnh tốc độ âm thanh (Pitch)
            // Nếu chạy -> Pitch cao (tiếng nhanh). Nếu đi bộ -> Pitch bình thường.
            if (currentSpeed == runSpeed)
            {
                footstepSource.pitch = runPitchMultiplier; // Ví dụ: 1.5
            }
            else
            {
                footstepSource.pitch = 1f; // Bình thường
            }
        }
        else
        {
            // Nếu dừng lại mà loa vẫn đang kêu -> TẮT LOA
            if (footstepSource.isPlaying)
            {
                footstepSource.Stop();
                // Hoặc dùng footstepSource.Pause() nếu muốn giữ vị trí file âm thanh
            }
        }
    }

    // ==============================
    // External Control
    // ==============================
    public void StopMoving()
    {
        rb.velocity = Vector2.zero;
        if (animator != null) animator.SetFloat("Speed", 0f);

        // Dừng luôn âm thanh nếu bị ép dừng
        if (footstepSource != null && footstepSource.isPlaying)
            footstepSource.Stop();
    }

    public void ApplyKnockback(Vector2 direction, float force, float duration)
    {
        isKnockedBack = true;
        rb.velocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);

        // Khi bị đẩy lùi thì không phát tiếng bước chân
        if (footstepSource != null && footstepSource.isPlaying)
            footstepSource.Stop();

        StartCoroutine(KnockbackRoutine(duration));
    }

    private IEnumerator KnockbackRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        rb.velocity = Vector2.zero;
        isKnockedBack = false;
    }
}