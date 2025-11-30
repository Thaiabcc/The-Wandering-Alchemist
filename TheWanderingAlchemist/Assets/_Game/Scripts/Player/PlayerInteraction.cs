using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private float checkRadius = 1f;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private float gatherDelay = 0.4f;

    private GameControls gameControl;
    private Animator animator;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        gameControl = new GameControls();
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void OnEnable()
    {
        gameControl.Enable();

        // Sign event
        gameControl.Gameplay.Interact.performed += _ => TryInteract();
    }

    private void OnDisable()
    {
        gameControl.Disable();
    }

    private void TryInteract()
    {
        // Shooting circle to search item
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, checkRadius, interactLayer);

        // Search the first item
        foreach(var hit in hitColliders)
        {
            // Check the item has Interact or not 
            IInteractable interactable = hit.GetComponent<IInteractable>();
            if(interactable != null)
            {
                StartCoroutine(PerformGather(interactable));
                return;
            }
        }    
    }
    // Function wait
    IEnumerator PerformGather(IInteractable item)
    {
        // 1. Locked move
        if (playerMovement != null) playerMovement.enabled = false;

        // 2. Run ani
        if (animator != null) animator.SetTrigger("Gather");

        // 3. Time
        yield return new WaitForSeconds(gatherDelay);

        // 4. Gather
        item.Interact();

        // 5. Unlocked move
        if (playerMovement != null) playerMovement.enabled = true;
    }

    // Draw a Circle in Scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}
