using UnityEngine;
using System;

public class DayNightCycle : MonoBehaviour
{
    public static event Action OnNewDay;

    [Header("Thời gian")]
    [Tooltip("Thời gian (giây) cho một ngày 24h trong game.")]
    public float dayDurationInSeconds = 120f; 

    [Range(0, 1)]
    public float currentTimeOfDay = 0.5f; 

    [Header("Thiết lập Ánh sáng")]
    [Tooltip("Kéo Directional Light (Mặt trời) vào đây.")]
    public Light sunLight;

    [Tooltip("Màu của Mặt trời (ví dụ: sáng ban ngày, cam ban đêm).")]
    public Gradient sunColor;

    [Tooltip("Cường độ sáng của Mặt trời.")]
    public AnimationCurve sunIntensity;

    [Tooltip("Màu của ánh sáng môi trường (bầu trời).")]
    public Gradient ambientLightColor;

    [Header("Thiết lập Skybox")]
    [Tooltip("Độ sáng của bầu trời (Exposure).")]
    public AnimationCurve skyboxExposure;
    
    private void Update()
    {
        if (sunLight == null)
            return;

        float previousTimeOfDay = currentTimeOfDay;

        currentTimeOfDay += Time.deltaTime / dayDurationInSeconds;

        if (previousTimeOfDay < 1.0f && currentTimeOfDay >= 1.0f)
        {
            OnNewDay?.Invoke();
        }
        
        currentTimeOfDay = Mathf.Repeat(currentTimeOfDay, 1);

        UpdateSun();
    }

    void UpdateSun()
    {
        float sunRotation = (currentTimeOfDay - 0.25f) * 360f;
        sunLight.transform.rotation = Quaternion.Euler(sunRotation, -30f, 0f);

        float timeValue = currentTimeOfDay;

        Color currentSunColor = sunColor.Evaluate(timeValue);
        Color currentAmbientColor = ambientLightColor.Evaluate(timeValue);
        float currentSunIntensity = sunIntensity.Evaluate(timeValue);
        
        float currentSkyExposure = skyboxExposure.Evaluate(timeValue);
       
        sunLight.color = currentSunColor;
        sunLight.intensity = currentSunIntensity;
        RenderSettings.ambientLight = currentAmbientColor;

        if (RenderSettings.skybox != null)
        {
            RenderSettings.skybox.SetFloat("_Exposure", currentSkyExposure);
        }
    }
}