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
    // Footstep Audio
    // ==============================
    [Header("Footstep")]
    [SerializeField] private float footstepDelay = 0.4f;
    [SerializeField] private float runStepMultiplier = 0.6f;

    private float footstepTimer;

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
        UpdateFacing();
        HandleFootstepAudio();
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
            if (PlayerStats.Instance.TryConsumeStamina(staminaDrain * Time.deltaTime))
                currentSpeed = runSpeed;
        }
    }

    // ==============================
    // Stamina
    // ==============================
    private void HandleStamina()
    {
        if (moveInput.sqrMagnitude == 0)
        {
            PlayerStats.Instance.RegenerateStamina(
                PlayerStats.Instance.staminaRegenRate * Time.deltaTime
            );
        }
    }

    // ==============================
    // Animation & Facing
    // ==============================
    private void UpdateAnimation()
    {
        if (animator != null)
            animator.SetFloat("Speed", moveInput.magnitude);
    }

    private void UpdateFacing()
    {
        if (moveInput.x == 0) return;

        transform.rotation = moveInput.x > 0
            ? Quaternion.Euler(0, 0, 0)
            : Quaternion.Euler(0, 180, 0);
    }

    // ==============================
    // Footstep Audio
    // ==============================
    private void HandleFootstepAudio()
    {
        if (moveInput.sqrMagnitude == 0)
        {
            footstepTimer = 0;
            return;
        }

        footstepTimer -= Time.deltaTime;
        if (footstepTimer > 0) return;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.footstep, 0.8f);

        footstepTimer = currentSpeed == runSpeed
            ? footstepDelay * runStepMultiplier
            : footstepDelay;
    }

    // ==============================
    // External Control
    // ==============================
    public void StopMoving()
    {
        rb.velocity = Vector2.zero;
        animator?.SetFloat("Speed", 0f);
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
}
