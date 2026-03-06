using UnityEngine;
using UnityEngine.Rendering;

public class CameraSorting : MonoBehaviour
{
    private void Start()
    {
        GraphicsSettings.transparencySortMode = TransparencySortMode.CustomAxis;
        GraphicsSettings.transparencySortAxis = new Vector3(0, 1, 0);
    }
}