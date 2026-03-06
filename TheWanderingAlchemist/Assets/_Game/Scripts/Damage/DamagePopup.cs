using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;
    private Vector3 moveVector;

    // --- CẤU HÌNH---
    [SerializeField] private float moveSpeedY = 20f;   
    [SerializeField] private float moveSpeedX = 10f;   
    [SerializeField] private float disappearSpeed = 3f; 

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }
    public void Setup(int damageAmount, bool isCriticalHit)
    {
        textMesh.text = damageAmount.ToString();
        disappearTimer = 1f;

        if (!isCriticalHit)
        {
            textMesh.fontSize = 5;          
            textMesh.color = Color.yellow;   
            textColor = Color.yellow;
            moveVector = new Vector3(Random.Range(-moveSpeedX, moveSpeedX), moveSpeedY);
            transform.localScale = Vector3.one;
        }
        else
        {
            // --- BẠO KÍCH---
            textMesh.fontSize = 6;         
            textMesh.color = Color.red;     
            textColor = Color.red;
            moveVector = new Vector3(Random.Range(-moveSpeedX, moveSpeedX) * 1.5f, moveSpeedY * 1.5f);
            transform.localScale = Vector3.one * 1.5f;
        }
    }

    private void Update()
    {
        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * 8f * Time.deltaTime;

        if (disappearTimer > 1f * 0.5f)
        {
            // Nửa đầu thời gian: Giữ nguyên size
        }
        else
        {
            // Nửa sau thời gian: Có thể cho rơi nhẹ nếu muốn
        }

        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;

            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}