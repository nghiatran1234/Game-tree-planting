using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class EnvironmentManager : MonoBehaviour
{
    [Header("Hiển thị Ngày và Thời tiết")]
    public Text dayText; // Kéo Text hiển thị ngày vào đây
    public Text weatherText;
    public static EnvironmentManager Instance { get; set; }

    [Header("Thiết lập Mùa (Seasons)")]
    public Season currentSeason = Season.Spring;
    public int dayCounter = 1;
    public int daysPerSeason = 10; // 10 ngày cho mỗi mùa

    [Header("Thiết lập Thời tiết (Weather)")]
    public Weather currentWeather = Weather.Sunny;
    public ParticleSystem rainParticleSystem; // Kéo "Mưa" vào đây
    public AudioSource rainAudioSource;       // Kéo "Tiếng mưa" vào đây
    
    // (Tùy chọn) Thêm ParticleSystem cho Tuyết
    public ParticleSystem snowParticleSystem; 

    public float weatherCheckInterval = 30f; // 30 giây kiểm tra thời tiết 1 lần
    private float weatherTimer = 0f;

    public enum Season { Spring, Summer, Autumn, Winter }
    public enum Weather { Sunny, Raining, Snowing }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void OnEnable()
    {
        // Đăng ký nhận tín hiệu "Ngày mới" từ DayNightCycle
        DayNightCycle.OnNewDay += AdvanceDay;
    }

    private void OnDisable()
    {
        // Hủy đăng ký khi tắt
        DayNightCycle.OnNewDay -= AdvanceDay;
    }

    // Hàm này được gọi mỗi khi qua nửa đêm
    void AdvanceDay()
    {
        dayCounter++;
        if (dayCounter % daysPerSeason == 0)
        {
            AdvanceSeason();
        }
        Debug.Log("Một ngày mới đã bắt đầu. Ngày thứ " + dayCounter);
        UpdateUI();
    }

    void AdvanceSeason()
    {
        currentSeason++;
        if (currentSeason > Season.Winter)
        {
            currentSeason = Season.Spring;
        }
        Debug.Log("Mùa đã đổi thành: " + currentSeason.ToString());
        UpdateUI();
    }

    void Update()
    {
        // Đếm giờ để kiểm tra thời tiết
        weatherTimer += Time.deltaTime;
        if (weatherTimer >= weatherCheckInterval)
        {
            weatherTimer = 0;
            UpdateWeather();
        }
        UpdateUI();
    }

    void UpdateWeather()
    {
        // Logic quyết định thời tiết (bạn có thể làm phức tạp hơn)
        float rand = Random.value; // Số ngẫu nhiên từ 0.0 đến 1.0

        switch (currentSeason)
        {
            case Season.Spring:
                if (rand < 0.3f) SetWeather(Weather.Raining); // 30% mưa
                else SetWeather(Weather.Sunny);
                break;
            case Season.Summer:
                if (rand < 0.1f) SetWeather(Weather.Raining); // 10% mưa
                else SetWeather(Weather.Sunny);
                break;
            case Season.Autumn:
                if (rand < 0.4f) SetWeather(Weather.Raining); // 40% mưa
                else SetWeather(Weather.Sunny);
                break;
            case Season.Winter:
                // (Tùy chọn) Thêm logic Tuyết
                // if (rand < 0.5f) SetWeather(Weather.Snowing); 
                SetWeather(Weather.Sunny); // Tạm thời
                break;
        }
    }

    void SetWeather(Weather newWeather)
    {
        if (currentWeather == newWeather) return; // Vẫn như cũ
        currentWeather = newWeather;
        UpdateUI();

        // Tắt hết
        if (rainParticleSystem != null) rainParticleSystem.Stop();
        if (rainAudioSource != null) rainAudioSource.Stop();
        // snowParticleSystem.Stop();
        if (PlayerState.Instance != null)
            PlayerState.Instance.isGettingWet = false;

        // Bật cái cần bật
        switch (newWeather)
        {
            case Weather.Sunny:
                break;
            case Weather.Raining:
                if (rainParticleSystem != null) rainParticleSystem.Play();
                if (rainAudioSource != null) rainAudioSource.Play();
                if (PlayerState.Instance != null)
                    PlayerState.Instance.isGettingWet = true;
                break;
            case Weather.Snowing:
                snowParticleSystem.Play();
                break;
        }
    }
    void UpdateUI()
    {
        if (dayText != null)
        {
            dayText.text = "Day: " + dayCounter;
            // Ví dụ hiển thị cả mùa: dayText.text = $"Ngày: {dayCounter} (Mùa {currentSeason})";
        }

        if (weatherText != null)
        {
            weatherText.text = "Weather: " + GetWeatherString(currentWeather);
        }
    }

    // --- THÊM HÀM CHUYỂN ĐỔI THỜI TIẾT ---
    private string GetWeatherString(EnvironmentManager.Weather weather)
    {
        switch (weather)
        {
            case EnvironmentManager.Weather.Sunny:
                return "Sunny";
            case EnvironmentManager.Weather.Raining:
                return "Raining";
            case EnvironmentManager.Weather.Snowing:
                return "Snowing";
            default:
                return "Không xác định";
        }
    }
}