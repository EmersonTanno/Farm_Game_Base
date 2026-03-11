using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class IlluminationController : MonoBehaviour
{
    public static IlluminationController Instance;
    [SerializeField] Light2D globalLight;
    private Coroutine illuminationRoutine;

    private float timeIntensity = 1f;
    private float weatherIntensity = 1f;
    private float locationIntensity = 1f;


    #region Core
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        SetIllumination();
    }
    #endregion

    #region Events
    void OnEnable()
    {
        WarpController.OnWarpEnd += SetIllumination;
        Calendar_Controller.OnDayChange += ResetIlluminationIntensity;
        Calendar_Controller.OnMonthChange += SetIllumination;
        Time_Controll.OnHourChange += UpdateDayLight;
        WeatherController.OnWeatherChanged += SetIlluminationToWeather;
        WeatherController.OnThunderFall += FlashIllumination;
    }

    void OnDisable()
    {
        WarpController.OnWarpEnd -= SetIllumination;
        Calendar_Controller.OnDayChange -= ResetIlluminationIntensity;
        Calendar_Controller.OnMonthChange -= SetIllumination;
        Time_Controll.OnHourChange -= UpdateDayLight;
        WeatherController.OnWeatherChanged -= SetIlluminationToWeather;
        WeatherController.OnThunderFall -= FlashIllumination;
    }

    #endregion

    private void SetIllumination()
    {
        ScenesTypeEnum scene = SceneInfo.Instance.sceneType;

        switch (scene)
        {
            case ScenesTypeEnum.inside:
                {
                    InnerIllumination();
                    break;
                }
            case ScenesTypeEnum.outside:
                {
                    Season season = Calendar_Controller.Instance.season;
                    OutsideIllumination(season);
                    break;
                }
        }
        ApplyFinalIntensity();
    }

    public void ChangeIllumination(Color newColor)
    {
        globalLight.color = newColor;
    }

    public void InnerIllumination()
    {
        ChangeIllumination(Color.white);

        locationIntensity = 1.2f;
        weatherIntensity = 1f;
    }

    public void OutsideIllumination(Season season)
    {
        Color newColor = Color.white;
        switch(season)
        {
            case Season.Verao:
                {
                    ColorUtility.TryParseHtmlString("#FFCEBF", out newColor);
                    break;
                } 
            case Season.Outono:
                {
                    ColorUtility.TryParseHtmlString("#FFC899", out newColor);
                    break;
                } 
            case Season.Inverno:
                {
                    ColorUtility.TryParseHtmlString("#99DCFF", out newColor);
                    break;
                } 
            case Season.Primavera:
                {
                    ColorUtility.TryParseHtmlString("#C2FFCA", out newColor);
                    break;
                } 
        }
        ChangeIllumination(newColor);
    }

    public void SetIlluminationIntensity(float intensity)
    {
        globalLight.intensity = intensity;
    }

    private void ResetIlluminationIntensity()
    {
        timeIntensity = 1f;
        weatherIntensity = 1f;
        locationIntensity = 1f;

        ApplyFinalIntensity();
    }

    public void ChangeIlluminationIntensitySmooth(float targetIntensity, float duration = 1f)
    {
        if (illuminationRoutine != null)
            StopCoroutine(illuminationRoutine);

        illuminationRoutine = StartCoroutine(ChangeIlluminationIntensityRoutine(targetIntensity, duration));
    }

    private IEnumerator ChangeIlluminationIntensityRoutine(float targetIntensity, float duration)
    {
        float startIntensity = globalLight.intensity;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            globalLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, t);
            yield return null;
        }

        globalLight.intensity = targetIntensity;
        illuminationRoutine = null;
    }


    #region Update Day Light to Hour
    private void UpdateDayLight()
    {
        Time_Controll time = Time_Controll.Instance;

        if (time.hours < 16)
        {
            timeIntensity = 1f;
        }
        else
        {
            float intensity = (24 - time.hours) / 10f;

            if (intensity < 0.2f)
            {
                intensity = 0.15f;
            }

            timeIntensity = intensity;
        }

        ApplyFinalIntensitySmooth();
    }
    #endregion

    #region Update Day Light to Weather
    private void SetIlluminationToWeather(WeatherEnum weather)
    {
        switch(weather)
        {
            case WeatherEnum.SUN:
                weatherIntensity = 1f;
                break;

            case WeatherEnum.RAIN:
            case WeatherEnum.SNOW:
                weatherIntensity = 0.85f;
                break;

            case WeatherEnum.TEMPEST:
            case WeatherEnum.BLIZZARD:
                weatherIntensity = 0.65f;
                break;
        }

        ApplyFinalIntensitySmooth();
    }

    private void FlashIllumination()
    {
        StartCoroutine(Flash());
    }

    private IEnumerator Flash()
    {
        if (illuminationRoutine != null)
        {
            StopCoroutine(illuminationRoutine);
        }

        float baseIntensity = GetFinalIntensity();

        int flashes = Random.Range(2, 4);

        for (int i = 0; i < flashes; i++)
        {
            globalLight.intensity = baseIntensity + Random.Range(0.6f, 1.0f);
            yield return new WaitForSeconds(Random.Range(0.03f, 0.08f));

            globalLight.intensity = baseIntensity;
            yield return new WaitForSeconds(Random.Range(0.04f, 0.1f));
        }

        ApplyFinalIntensity();
    }
    #endregion

    #region Apply Final Intensity
    private void ApplyFinalIntensity()
    {
        globalLight.intensity = GetFinalIntensity();
    }

    private void ApplyFinalIntensitySmooth()
    {
        if (illuminationRoutine != null)
            StopCoroutine(illuminationRoutine);

        illuminationRoutine = StartCoroutine(ChangeIlluminationIntensityRoutine(GetFinalIntensity(), 5));
    }

    private float GetFinalIntensity()
    {
        return timeIntensity * weatherIntensity * locationIntensity;
    }
    #endregion

}
