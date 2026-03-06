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
    [SerializeField] public float walkSpeed = 5f;
    [SerializeField] public float runSpeed = 8f;

    // ==============================
    // Stamina
    // ==============================
    [Header("Stamina")]
    [SerializeField] private float staminaDrain = 20f;
    [SerializeField] private float staminaRegen = 10f;

    // ==============================
    // Audio Settings
    // ==============================
    [Header("Footstep Loop Audio")]
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private float runPitchMultiplier = 1.5f;

    // ==============================
    // Components
    // ==============================
    private Rigidbody2D rb;
    private Animator animator;
    private GameControls controls;
    private SpriteRenderer spriteRenderer;

    // ==============================
    // State
    // ==============================
    public Vector2 MoveInput { get; private set; } 
    private float currentSpeed;
    private bool isKnockedBack;
    private bool canMove = true; 
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

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

    private void Update()
    {
        if (isKnockedBack || !canMove) return;

        ReadInput();
        HandleStamina();
        UpdateAnimation();
        HandleFootstepAudioLoop();
    }

    private void FixedUpdate()
    {
        if (isKnockedBack || !canMove) return;
        rb.velocity = MoveInput * currentSpeed;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryTeleportToSpawn();
    }

    private void TryTeleportToSpawn()
    {
        if (GameManager.Instance != null && GameManager.Instance.nextSpawnPosition.HasValue)
        {
            Vector3 spawnPos = GameManager.Instance.nextSpawnPosition.Value;
            transform.position = spawnPos; 
            GameManager.Instance.nextSpawnPosition = null;
        }
    }

    // ==============================
    // Input & Movement
    // ==============================
    private void ReadInput()
    {
        MoveInput = controls.Gameplay.Move.ReadValue<Vector2>();
        currentSpeed = walkSpeed;

        bool isRunning = Keyboard.current != null && Keyboard.current.shiftKey.isPressed;
        bool isMoving = MoveInput.sqrMagnitude > 0;

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
        if (MoveInput.sqrMagnitude == 0 && PlayerStats.Instance != null)
        {
            PlayerStats.Instance.RegenerateStamina(staminaRegen * Time.deltaTime);
        }
    }

    // ==============================
    // ANIMATION
    // ==============================
    private void UpdateAnimation()
    {
        if (animator == null) return;
        animator.SetFloat("Speed", MoveInput.sqrMagnitude);
        if (MoveInput.sqrMagnitude > 0.01f)
        {
            animator.SetFloat("Horizontal", MoveInput.x);
            animator.SetFloat("Vertical", MoveInput.y);
            if (spriteRenderer != null)
            {
                // Nếu đi sang trái (x < 0) -> Lật hình (FlipX = true)
                // Nếu đi sang phải (x > 0) -> Hủy lật (FlipX = false)
                if (MoveInput.x < -0.01f) spriteRenderer.flipX = true;
                else if (MoveInput.x > 0.01f) spriteRenderer.flipX = false;
            }
        }
    }

    // ==============================
    // Audio Logic
    // ==============================
    private void HandleFootstepAudioLoop()
    {
        if (footstepSource == null) return;

        bool isMoving = MoveInput.sqrMagnitude > 0;

        if (isMoving)
        {
            if (!footstepSource.isPlaying)
            {
                footstepSource.Play();
            }
            footstepSource.pitch = (currentSpeed == runSpeed) ? runPitchMultiplier : 1f;
        }
        else
        {
            if (footstepSource.isPlaying)
            {
                footstepSource.Stop();
            }
        }
    }
    public void SetCanMove(bool status)
    {
        canMove = status;
        if (!status)
        {
            rb.velocity = Vector2.zero;
            if (animator) animator.SetFloat("Speed", 0);
            if (footstepSource && footstepSource.isPlaying) footstepSource.Stop();
        }
    }

    public void ApplyKnockback(Vector2 direction, float force, float duration)
    {
        if (isKnockedBack) return;

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