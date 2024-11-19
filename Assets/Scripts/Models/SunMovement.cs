using UnityEngine;

public class SunMovement : MonoBehaviour
{
    [SerializeField] private float dayLengthInMinutes = 1f;
    [SerializeField] private float startTime = 0.25f;
    private float currentTime = 0f;

    [SerializeField] private Transform sunTransform;
    [SerializeField] private float maxSunIntensity = 1f;
    [SerializeField] private Light sunLight;

    private float secondsInDay;
    private const float DayRatio = 2f / 3f;
    private const float NightRatio = 1f / 3f;

    private void Start()
    {
        secondsInDay = dayLengthInMinutes * 60f;
        currentTime = startTime * secondsInDay;

        if (sunTransform == null) sunTransform = transform;
        if (sunLight == null) sunLight = GetComponent<Light>();
    }

    private void Update()
    {
        UpdateTime();
        UpdateSunPosition();
        UpdateSunIntensity();
    }

    private void UpdateTime()
    {
        currentTime += Time.deltaTime;
        if (currentTime > secondsInDay) currentTime = 0f;
    }

    private void UpdateSunPosition()
    {
        float dayProgress = currentTime / secondsInDay;

        float sunAngle;
        if (dayProgress <= DayRatio)
        {
            float dayProgressNormalized = dayProgress / DayRatio;
            sunAngle = Mathf.Lerp(-90f, 180f, dayProgressNormalized);
        }
        else
        {
            float nightProgressNormalized = (dayProgress - DayRatio) / NightRatio;
            sunAngle = Mathf.Lerp(180f, 270f, nightProgressNormalized);
        }

        sunTransform.localRotation = Quaternion.Euler(sunAngle, 0f, 0f);
    }

    private void UpdateSunIntensity()
    {
        float dayProgress = currentTime / secondsInDay;

        float intensityFactor;
        if (dayProgress <= DayRatio)
        {
            float dayProgressNormalized = dayProgress / DayRatio;
            intensityFactor = Mathf.Clamp01(Mathf.SmoothStep(0f, 1f, dayProgressNormalized));
        }
        else
        {
            float nightProgressNormalized = (dayProgress - DayRatio) / NightRatio;
            intensityFactor = Mathf.Clamp01(Mathf.SmoothStep(1f, 0f, nightProgressNormalized));
        }

        sunLight.intensity = intensityFactor * maxSunIntensity;
    }
}
