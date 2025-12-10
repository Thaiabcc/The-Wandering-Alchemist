using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WagonEntrance : MonoBehaviour, IInteractable
{
    [SerializeField] private string interiorSceneName = "WagonInterior";
    // Vị trí xuất hiện trong xe (thường là giữa phòng hoặc trên thảm cửa)
    [SerializeField] private Vector3 spawnPosInside = new Vector3(0, -2, 0);

    public void Interact()
    {
        // 1. Lưu tên Map hiện tại (Town_01)
        GameManager.Instance.lastWorldScene = SceneManager.GetActiveScene().name;

        // 2. Tính vị trí đứng khi quay lại (Dưới chân xe 1.5 mét)
        Vector3 returnPos = transform.position + new Vector3(0, -1.5f, 0);

        // 3. Lưu vào GameManager
        GameManager.Instance.lastWorldPosition = returnPos;

        // 4. Vào xe
        GameManager.Instance.nextSpawnPosition = spawnPosInside;
        SceneManager.LoadScene(interiorSceneName);
    }
}
