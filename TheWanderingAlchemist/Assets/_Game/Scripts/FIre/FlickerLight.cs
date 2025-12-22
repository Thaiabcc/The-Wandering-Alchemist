using UnityEngine;
using UnityEngine.Rendering.Universal; 

public class FlickerLight2D : MonoBehaviour
{
    public Light2D fireLight;

    public float minIntensity = 0.5f; 
    public float maxIntensity = 1.5f;
    public float speed = 10f;

    void Start()
    {
        if (fireLight == null)
        {
            fireLight = GetComponent<Light2D>();
        }
    }

    void Update()
    {
        if (fireLight != null)
        {
            float noise = Mathf.PerlinNoise(Time.time * speed, 0);
            fireLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
        }
    }
}