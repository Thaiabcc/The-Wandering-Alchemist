using UnityEngine;

public class SeamlessScroll : MonoBehaviour
{
    public float speed = 2f;

    [Header("Overlap Amount")]
    public float overlap = 0f;

    private Transform[] backgrounds;
    private float effectiveWidth;
    private float leftEdge;

    void Start()
    {
        backgrounds = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            backgrounds[i] = transform.GetChild(i);
        }

        float originalWidth = backgrounds[0].GetComponent<SpriteRenderer>().bounds.size.x;

        effectiveWidth = originalWidth - overlap;

        float startX = backgrounds[0].localPosition.x;

        leftEdge = startX - effectiveWidth;

        for (int i = 0; i < backgrounds.Length; i++)
        {
            float originalY = backgrounds[i].localPosition.y;
            float originalZ = backgrounds[i].localPosition.z;

            backgrounds[i].localPosition = new Vector3(startX + (i * effectiveWidth), originalY, originalZ);
        }
    }

    void Update()
    {
        foreach (Transform bg in backgrounds)
        {
            bg.localPosition += Vector3.left * speed * Time.deltaTime;

            if (bg.localPosition.x <= leftEdge)
            {
                bg.localPosition += new Vector3(effectiveWidth * backgrounds.Length, 0, 0);
            }
        }
    }
}