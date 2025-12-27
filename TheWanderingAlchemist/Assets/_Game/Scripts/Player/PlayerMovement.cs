using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; // [QUAN TRỌNG] Thêm cái này

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
    private Animator animator;

    private float currentSpeed;
    private bool isKnockedBack = false;

    // --- CÁC HÀM CƠ BẢN ---
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        gameControls = new GameControls();
        if (animator == null) Debug.LogError("Thiếu Animator!");
    }

    private void OnEnable()
    {
        gameControls.Enable();
        // Đăng ký sự kiện: Cứ load màn xong là gọi hàm OnSceneLoaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        gameControls.Disable();
        // Hủy đăng ký để tránh lỗi
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // --- [FIX] HÀM NÀY CHẠY MỖI KHI SANG MAP MỚI ---
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Gọi hàm dịch chuyển (Delay 1 xíu để map load xong hẳn)
        StartCoroutine(TeleportRoutine());
    }

    // Dự phòng: Nếu Player không phải DontDestroyOnLoad thì chạy cái này
    private void Start()
    {
        StartCoroutine(TeleportRoutine());
    }

    // --- LOGIC DỊCH CHUYỂN ---
    private IEnumerator TeleportRoutine()
    {
        // Chờ 1 frame để đảm bảo GameManager đã sẵn sàng
        yield return null;

        if (GameManager.Instance != null && GameManager.Instance.nextSpawnPosition.HasValue)
        {
            Debug.Log("PLAYER: Phát hiện vị trí mới! Đang tele tới: " + GameManager.Instance.nextSpawnPosition.Value);

            // 1. Tắt vật lý
            if (rb != null) rb.simulated = false;

            // 2. Dịch chuyển
            transform.position = GameManager.Instance.nextSpawnPosition.Value;

            // 3. Reset dữ liệu trong GameManager (Quan trọng!)
            GameManager.Instance.nextSpawnPosition = null;

            // 4. Chờ thêm 1 frame để vị trí ăn vào hệ thống
            yield return null;

            // 5. Bật lại vật lý
            if (rb != null) rb.simulated = true;
        }
    }
    // ---------------------------------------------

    // --- CÁC HÀM XỬ LÝ DI CHUYỂN CŨ (GIỮ NGUYÊN) ---
    public void StopMoving()
    {
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        if (animator != null) animator.SetFloat("Speed", 0f);
    }

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

    private void Update()
    {
        if (isKnockedBack) return;

        moveInput = gameControls.Gameplay.Move.ReadValue<Vector2>();
        currentSpeed = walkSpeed;

        bool isMoving = moveInput.magnitude > 0;
        bool isRunPressed = Keyboard.current != null && Keyboard.current.shiftKey.isPressed;

        if (isMoving && isRunPressed)
        {
            if (PlayerStats.Instance.TryConsumeStamina(staminaDrain * Time.deltaTime)) currentSpeed = runSpeed;
            else currentSpeed = walkSpeed;
        }
        else
        {
            PlayerStats.Instance.RegenerateStamina(PlayerStats.Instance.staminaRegenRate * Time.deltaTime);
        }

        if (animator != null) animator.SetFloat("Speed", moveInput.magnitude);

        if (moveInput.x != 0)
        {
            if (moveInput.x > 0) transform.rotation = Quaternion.Euler(0, 0, 0);
            else transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        // Audio
        if (isMoving)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0)
            {
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.footstep, 0.8f);

                if (currentSpeed == runSpeed) footstepTimer = footstepDelay * runStepMultiplier;
                else footstepTimer = footstepDelay;
            }
        }
        else footstepTimer = 0;
    }

    private void FixedUpdate()
    {
        if (isKnockedBack) return;
        rb.velocity = moveInput * currentSpeed;
    }
}