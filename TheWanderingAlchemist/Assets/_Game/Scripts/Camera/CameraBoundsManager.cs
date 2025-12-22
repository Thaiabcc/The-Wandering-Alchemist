using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class CameraBoundsManager : MonoBehaviour
{
    private CinemachineConfiner2D confiner;

    private void Awake()
    {
        confiner = GetComponent<CinemachineConfiner2D>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (confiner == null) return;

        GameObject boundsObject = GameObject.FindGameObjectWithTag("Bounds");

        if (boundsObject != null)
        {
            Collider2D boundsCollider = boundsObject.GetComponent<Collider2D>();

            if (boundsCollider != null)
            {
                confiner.m_BoundingShape2D = boundsCollider;

                // [SỬA LẠI DÒNG NÀY]
                // Xóa bộ nhớ đệm để nó tính toán lại khung mới
                confiner.InvalidateCache();

                Debug.Log($"[Camera] Đã cập nhật biên giới cho map: {scene.name}");
            }
        }
        else
        {
            Debug.LogWarning($"[Camera] CẢNH BÁO: Map {scene.name} chưa có object nào gắn Tag 'Bounds'!");
        }
    }
}