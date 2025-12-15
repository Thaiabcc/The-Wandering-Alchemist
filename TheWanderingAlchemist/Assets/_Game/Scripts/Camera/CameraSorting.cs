using UnityEngine;
using UnityEngine.Rendering; // <-- QUAN TRỌNG: Phải thêm thư viện này

public class CameraSorting : MonoBehaviour
{
    private void Start()
    {
        // Sửa từ "Graphics" thành "GraphicsSettings"
        GraphicsSettings.transparencySortMode = TransparencySortMode.CustomAxis;
        GraphicsSettings.transparencySortAxis = new Vector3(0, 1, 0);
    }
}