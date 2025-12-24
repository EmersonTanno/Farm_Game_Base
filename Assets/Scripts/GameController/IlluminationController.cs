using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class IlluminationController : MonoBehaviour
{
    public static IlluminationController Instance;
    [SerializeField] Light2D globalLight;
    private Coroutine illuminationRoutine;


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
    }

    void OnDisable()
    {
        WarpController.OnWarpEnd -= SetIllumination;
        Calendar_Controller.OnDayChange -= ResetIlluminationIntensity;
        Calendar_Controller.OnMonthChange -= SetIllumination;
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
    }

    public void ChangeIllumination(Color newColor)
    {
        globalLight.color = newColor;
    }

    public void InnerIllumination()
    {
        ChangeIllumination(Color.white);
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
        SetIlluminationIntensity(1);
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


}
