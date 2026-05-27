using UnityEngine;

public class SeamlessScroll : MonoBehaviour
{
    public float speed = 2f;
    private float currentSpeed; // Thêm biến này để quản lý tốc độ thực tế

    [Header("Overlap Amount")]
    public float overlap = 0f;

    private Transform[] backgrounds;
    private float effectiveWidth;
    private float leftEdge;

    void Start()
    {
        currentSpeed = speed; // Gán tốc độ mặc định ban đầu

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
        // Nếu tốc độ bằng 0 thì đứng im luôn, tiết kiệm hiệu năng
        if (currentSpeed == 0f) return; 

        foreach (Transform bg in backgrounds)
        {
            // Thay speed bằng currentSpeed
            bg.localPosition += Vector3.left * currentSpeed * Time.deltaTime; 

            if (bg.localPosition.x <= leftEdge)
            {
                bg.localPosition += new Vector3(effectiveWidth * backgrounds.Length, 0, 0);
            }
        }
    }

    // ==========================================
    // CÔNG TẮC CHO THẰNG ĐẠO DIỄN BẤM VÀO
    // ==========================================
    public void SetSpeed(float newSpeed)
    {
        currentSpeed = newSpeed;
    }
}