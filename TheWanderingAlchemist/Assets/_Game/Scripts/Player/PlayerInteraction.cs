using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float checkRadius = 1f;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private float interactDelay = 0.4f;

    private GameControls controls;
    private Animator animator;
    private PlayerMovement movement;

    private bool isInteracting;
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
        if (interactable == null)
            interactable = hit.GetComponentInParent<IInteractable>();

        if (interactable == null) return;
        StartCoroutine(InteractRoutine(interactable, hit.gameObject));
    }

    private IEnumerator InteractRoutine(IInteractable interactable, GameObject targetObj)
    {
        isInteracting = true;
        if (movement != null) movement.enabled = false;
        if (animator != null) animator.SetTrigger("Gather");
        yield return new WaitForSeconds(interactDelay);
        if (targetObj == null || interactable == null || (interactable as Object) == null)
        {
            Debug.LogWarning("⚠️ Mục tiêu đã biến mất trong lúc chờ!");
            UnlockMovement();
            yield break; 
        }

        try
        {
            interactable.Interact();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ LỖI CODE TRONG NPC: {e.Message}\nStack Trace: {e.StackTrace}");
        }
        finally
        {
            UnlockMovement();
        }
    }

    private void UnlockMovement()
    {
        if (movement != null) movement.enabled = true;
        isInteracting = false;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}