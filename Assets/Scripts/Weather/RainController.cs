using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class RainController : MonoBehaviour
{
    private ParticleSystem ps;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void Start()
    {
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    void OnEnable()
    {
        WeatherController.OnWeatherChanged += OnWeatherChanged;
        WarpController.OnWarpEnd += OnSceneChanged;
    }

    void OnDisable()
    {
        WeatherController.OnWeatherChanged -= OnWeatherChanged;
        WarpController.OnWarpEnd -= OnSceneChanged;
    }

    void OnWeatherChanged(WeatherEnum weather)
    {
        UpdateRain(weather);
    }

    void OnSceneChanged()
    {
        UpdateRain(WeatherController.Instance.GetWeather());
    }

    void UpdateRain(WeatherEnum weather)
    {
        Debug.Log("START RAIN");
        bool canRain =
            weather == WeatherEnum.RAIN &&
            SceneInfo.Instance.sceneType != ScenesTypeEnum.inside;

        if (canRain && !ps.isPlaying)
            ps.Play();
        else if (!canRain && ps.isPlaying)
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}
