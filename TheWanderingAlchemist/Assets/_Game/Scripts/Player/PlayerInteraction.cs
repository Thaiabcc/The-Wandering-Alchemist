using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private float checkRadius = 1f;
    [SerializeField] private LayerMask interactLayer;

    private GameControls gameControl;

    private void Awake()
    {
        gameControl = new GameControls();
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
                interactable.Interact();
                return;
            }
        }    
    }
    // Draw a Circle in Scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}
