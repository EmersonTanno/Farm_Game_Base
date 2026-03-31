using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class WeatherParticleSystemController : MonoBehaviour
{
    private ParticleSystem ps;
    public WeatherEnum selectedWeather;

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
        UpdateTempest(weather, false);
    }

    void OnSceneChanged()
    {
        UpdateTempest(WeatherController.Instance.GetWeather(), true);
    }

    void UpdateTempest(WeatherEnum weather, bool scene)
    {
        bool canRain =
            weather == selectedWeather &&
            SceneInfo.Instance.sceneType != ScenesTypeEnum.inside;

        var main = ps.main;

        main.prewarm = scene;

        if (canRain && !ps.isPlaying)
            StartCoroutine(DelayRainStart());
        else if (!canRain && ps.isPlaying && scene == true)
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        else if (!canRain && ps.isPlaying && scene == false)
            ps.Stop();
    }

    private IEnumerator DelayRainStart()
    {
        yield return new WaitForSeconds(3f);
        ps.Play();
    }
}
