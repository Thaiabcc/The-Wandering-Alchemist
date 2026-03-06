using UnityEngine;

public enum KnightColor
{
    Red,
    Blue,
    Green,
    Yellow,
    White,
    Black
}

public class PuzzleKnight : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    public KnightColor myColor;

    [Header("Visuals")]
    [SerializeField] private ParticleSystem activateEffect; 
    [SerializeField] private Animator animator;

    private KnightPuzzleManager manager;

    private void Start()
    {
        manager = FindObjectOfType<KnightPuzzleManager>();
    }

    public void Interact()
    {
        if (manager == null) return;
        if (activateEffect != null) activateEffect.Play();
        if (animator != null) animator.SetTrigger("Activate");
        manager.OnKnightActivated(this);
    }
}