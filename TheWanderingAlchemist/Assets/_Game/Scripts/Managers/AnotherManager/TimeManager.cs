using UnityEngine;
using UnityEngine.Rendering.Universal; 
using System;
using Random = UnityEngine.Random; // THÊM DÒNG NÀY ĐỂ DÙNG ACTION

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    // --- SỰ KIỆN PHÁT ĐI CHO HỆ THỐNG ĐÈN NGHE (MỚI) ---
    // Trả về true nếu là trời tối, false nếu là trời sáng
    public event Action<bool> OnNightStateChanged; 

    [Header("Time Settings")]
    [SerializeField] private float timeScale = 60f; 
    [SerializeField] private int startHour = 6;    

    [Header("Light References")]
    [SerializeField] private Light2D globalLight;   
    [SerializeField] private Gradient dayNightGradient; 
    [SerializeField] private AnimationCurve intensityCurve;

    // --- CẤU HÌNH GIỜ BẬT/TẮT ĐÈN (MỚI) ---
    [Header("Light Automation Settings (MỚI)")]
    [SerializeField] private int turnOnLightsHour = 18;  // 18h tối tự động bật đèn
    [SerializeField] private int turnOffLightsHour = 6;  // 6h sáng tự động tắt đèn

    public int CurrentDay { get; private set; } = 1;
    public float CurrentHour { get; private set; }
    public float CurrentMinute { get; private set; }

    // Biến lưu trạng thái trời tối hiện tại để check sự thay đổi
    private bool isNight = false; 
    public bool IsNight => isNight; // Cho phép các script khác kiểm tra trạng thái nhanh

    private double accumSeconds;
    private int totalSecondsInDay;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        CurrentHour = startHour;
        totalSecondsInDay = startHour * 3600;
        accumSeconds = totalSecondsInDay;

        // Cập nhật trạng thái sáng tối ban đầu dựa trên startHour
        isNight = CheckIfNight(CurrentHour);
    }

    private void Update()
    {
        accumSeconds += Time.deltaTime * timeScale;
        totalSecondsInDay = (int)accumSeconds;

        CurrentHour = totalSecondsInDay / 3600;
        CurrentMinute = (totalSecondsInDay % 3600) / 60;

        UpdateDayNightLight();
        CheckLightAutomation(); // --- GỌI HÀM KIỂM TRA BẬT TẮT ĐÈN Ở ĐÂY (MỚI) ---

        if (totalSecondsInDay >= 86400)
        {
            StartNewDay();
        }
    }

    // --- HÀM TỰ ĐỘNG KIỂM TRA VÀ GÕ CHUÔNG BÁO BẬT/TẮT ĐÈN (MỚI) ---
    private void CheckLightAutomation()
    {
        bool currentNightState = CheckIfNight(CurrentHour);

        // Nếu trạng thái Ngày/Đêm vừa mới thay đổi so với khung hình trước
        if (currentNightState != isNight)
        {
            isNight = currentNightState;
            // Gõ chuông thông báo cho toàn bộ đèn trên Map biết để bật hoặc tắt
            OnNightStateChanged?.Invoke(isNight);
        }
    }

    // Hàm bổ trợ để tính toán xem một mốc giờ có phải là ban đêm không
    private bool CheckIfNight(float hour)
    {
        // Nếu giờ >= 18h tối HOẶC nhỏ hơn 6h sáng thì tính là Đêm (True)
        return (hour >= turnOnLightsHour || hour < turnOffLightsHour);
    }

    private void UpdateDayNightLight()
    {
        if (globalLight == null || dayNightGradient == null) return;
        
        float percentageOfDay = (float)totalSecondsInDay / 86400f;
        globalLight.color = dayNightGradient.Evaluate(percentageOfDay);

        if (intensityCurve != null)
        {
            globalLight.intensity = intensityCurve.Evaluate(percentageOfDay);
        }
    }

    private void StartNewDay()
    {
        accumSeconds = 0;
        totalSecondsInDay = 0;
        CurrentHour = 0;
        CurrentMinute = 0;
        CurrentDay++;
        
        if (WeatherManager.Instance != null)
        {
            float chance = Random.value;
            if (chance < 0.6f)
                WeatherManager.Instance.ChangeWeather(WeatherState.Sunny);
            else if (chance < 0.85f)
                WeatherManager.Instance.ChangeWeather(WeatherState.Rainy);
            else
                WeatherManager.Instance.ChangeWeather(WeatherState.Stormy);
        }
    }

    public string GetTimeString()
    {
        return string.Format("{0:00}:{1:00}", CurrentHour, CurrentMinute);
    }
}