using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class TempestController : MonoBehaviour
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
        UpdateTempest(weather);
    }

    void OnSceneChanged()
    {
        UpdateTempest(WeatherController.Instance.GetWeather());
    }

    void UpdateTempest(WeatherEnum weather)
    {
        bool canRain =
            weather == WeatherEnum.TEMPEST &&
            SceneInfo.Instance.sceneType != ScenesTypeEnum.inside;

        if (canRain && !ps.isPlaying)
            ps.Play();
        else if (!canRain && ps.isPlaying)
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}
