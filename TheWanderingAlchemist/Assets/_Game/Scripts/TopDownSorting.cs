using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TopDownSorting : MonoBehaviour
{
    private SpriteRenderer sr;

    [Header("Sorting Settings")]
    public int offset = 0;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        // Object càng thấp -> hiện phía trước
        sr.sortingOrder = (int)(-transform.position.y * 100) + offset;
    }
}