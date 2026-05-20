using UnityEngine;

public class SpriteSorterY : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [SerializeField] private int baseOrder = 10;
    [SerializeField] private float precision = 100f;
    [SerializeField] private bool isStatic = true;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSortingOrder();
    }

    void LateUpdate()
    {
        if (isStatic && Application.isPlaying) return;
        UpdateSortingOrder();
    }

    private void UpdateSortingOrder()
    {
        if (spriteRenderer == null) return;
        int calculatedOrder = Mathf.RoundToInt(-transform.position.y * precision);
        spriteRenderer.sortingOrder = baseOrder + calculatedOrder;
    }
}