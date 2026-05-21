using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class JuicyTree : MonoBehaviour
{
    #region SETTINGS - ĐUNG ĐƯA (SWAYING)
    [Header("Đung Đưa - Swaying")]
    [SerializeField] private float swaySpeed = 1.2f; 
    [SerializeField] private float swayAmount = 2.5f; 
    [SerializeField] private float windTurbulence = 0.5f; 
    #endregion

    #region SETTINGS - LÁ RỤNG (LEAF FALL)
    [Header("Lá Rụng - Leaf Fall")]
    [SerializeField] private ParticleSystem leafParticles;
    [SerializeField] private float baseLeafEmission = 5f;
    [SerializeField] private float stormLeafEmission = 50f;
    #endregion

    #region PRIVATE VARIABLES
    private float originalRotationZ;
    private float currentWindStrength = 1f;
    private float timeOffset;
    #endregion

    #region UNITY LIFECYCLE
    private void Start()
    {
        originalRotationZ = transform.localEulerAngles.z;
        timeOffset = Random.Range(0f, 100f);
    }

    private void Update()
    {
        HandleSwaying();
    }
    #endregion

    #region MAIN LOGIC
    private void HandleSwaying()
    {
        currentWindStrength = CheckIfNight() ? 1.5f : 1f;

        float dynamicSpeed = swaySpeed * currentWindStrength;
        float dynamicAmount = swayAmount * currentWindStrength;

        float noise = Mathf.PerlinNoise(Time.time * dynamicSpeed * 0.5f + timeOffset, 0f) - 0.5f;
        float dynamicTime = Time.time * dynamicSpeed;
        float sinValue = Mathf.Sin(dynamicTime + noise * windTurbulence);

        float newRotationZ = originalRotationZ + (sinValue * dynamicAmount);

        transform.localRotation = Quaternion.Euler(0, 0, newRotationZ);
    }

    private bool CheckIfNight()
    {
        if (TimeManager.Instance == null) return false;
        float currentHour = TimeManager.Instance.CurrentHour;
        return currentHour >= 18f || currentHour < 6f;
    }
    #endregion
}