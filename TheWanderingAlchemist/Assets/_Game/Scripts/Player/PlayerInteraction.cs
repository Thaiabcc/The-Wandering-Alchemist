using System.Collections;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    // ==============================
    // Settings
    // ==============================
    [Header("Interaction")]
    [SerializeField] private float checkRadius = 1f;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private float interactDelay = 0.4f;

    // ==============================
    // Components
    // ==============================
    private GameControls controls;
    private Animator animator;
    private PlayerMovement movement;

    // ==============================
    // State
    // ==============================
    private bool isInteracting;

    // ==============================
    // Unity Lifecycle
    // ==============================
    private void Awake()
    {
        controls = new GameControls();
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Gameplay.Interact.performed += _ => TryInteract();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    // ==============================
    // Interaction
    // ==============================
    private void TryInteract()
    {
        if (isInteracting) return;

        Collider2D hit = Physics2D.OverlapCircle(
            transform.position,
            checkRadius,
            interactLayer
        );

        if (hit == null) return;

        IInteractable interactable = hit.GetComponent<IInteractable>();
        if (interactable == null) return;

        StartCoroutine(InteractRoutine(interactable));
    }

    private IEnumerator InteractRoutine(IInteractable interactable)
{
    isInteracting = true;

    if (movement != null)
        movement.enabled = false; 

    if (animator != null)
        animator.SetTrigger("Gather");

    yield return new WaitForSeconds(interactDelay);

    try
    {
        interactable.Interact();
    }
    catch (System.Exception e)
    {
        Debug.LogError("Lỗi khi tương tác: " + e.Message);
    }
    finally
    {
        if (movement != null)
            movement.enabled = true;
        
        isInteracting = false;
    }
}

    // ==============================
    // Gizmos
    // ==============================
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}
