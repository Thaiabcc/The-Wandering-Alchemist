using UnityEngine;
using System.Collections;

public class PlayerPenalty : MonoBehaviour
{
    [Header("Debuff Settings")]
    public float duration = 10f;
    public float speedMultiplier = 0.5f;
    public Color debuffColor = new Color(0.6f, 0.2f, 1f, 1f);

    private PlayerMovement movement;
    private SpriteRenderer sprite;

    private float defaultWalkSpeed;
    private float defaultRunSpeed;
    private Color defaultColor;

    private void Start()
    {
        movement = GetComponent<PlayerMovement>();
        sprite = GetComponent<SpriteRenderer>();

        if (movement != null)
        {
            defaultWalkSpeed = movement.walkSpeed;
            defaultRunSpeed = movement.runSpeed;
        }

        if (sprite != null) defaultColor = sprite.color;
    }

    public void ApplyPenalty()
    {
        StopAllCoroutines();
        StartCoroutine(PenaltyRoutine());
    }

    private IEnumerator PenaltyRoutine()
    {
        Debug.Log("💀 Bị tác dụng phụ: Giảm cả đi bộ và chạy!");

        if (movement != null)
        {
            movement.walkSpeed = defaultWalkSpeed * speedMultiplier;
            movement.runSpeed = defaultRunSpeed * speedMultiplier;
        }

        if (sprite != null) sprite.color = debuffColor;
        yield return new WaitForSeconds(duration);
        if (movement != null)
        {
            movement.walkSpeed = defaultWalkSpeed;
            movement.runSpeed = defaultRunSpeed;
        }

        if (sprite != null) sprite.color = defaultColor;

        Debug.Log("✨ Đã hết tác dụng phụ!");
    }
}