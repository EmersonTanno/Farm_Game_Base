using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Time_Controll : MonoBehaviour
{
    public static Time_Controll Instance { get; private set; }

    #region Variables
    //Min and Hr count
    public int minutes = 0;
    public int hours = 0;

    //Controll Variable
    private bool canChangeTime = true;

    //Canvas
    [SerializeField] TextMeshProUGUI timerText;

    //Bed
    [SerializeField] GameObject bedCanva;
    private bool canSelectOption = false;
    public bool bedActive = false;

    //Event
    public static event Action OnMidNightChange;
    public static event Action OnHourChange;
    #endregion

    #region Core
    void Awake()
    {
        Instance = this;
        UpdateCanvas();
    }
    void Update()
    {
        if (canChangeTime && !bedActive)
        {
            StartCoroutine(UpdateTime());
        }
    }
    #endregion

    private IEnumerator UpdateTime()
    {
        canChangeTime = false;
        yield return new WaitForSeconds(5f);

        if (minutes < 50)
        {
            minutes += 10;
        }
        else
        {
            hours += 1;
            minutes = 0;
            OnHourChange?.Invoke();

            if (hours > 23)
            {
                OnMidNightChange?.Invoke();
                hours = 0;
            }
        }

        UpdateCanvas();

        canChangeTime = true;
    }

    public void ChangeDay()
    {
        if (hours < 23)
        {
            OnMidNightChange?.Invoke();
        }
        OnHourChange?.Invoke();
        hours = 8;
        minutes = 0;
        UpdateCanvas();
    }

    private void UpdateCanvas()
    {
        if (timerText != null)
        {
            timerText.text = $"{hours:D2}:{minutes:D2}";
        }
    }

    #region Bed
    public void ActivateBedCanvas()
    {
        bedActive = true;
        bedCanva.SetActive(bedActive);
        PauseTime();
        StartCoroutine(SetCanSelect());
    }

    public void CancelBedCanvas()
    {
        if (canSelectOption)
        {
            bedActive = false;
            bedCanva.SetActive(bedActive);
            canSelectOption = false;
            UnpauseTime();
        }
    }

    public void Sleep()
    {
        if (canSelectOption)
        {
            bedActive = false;
            ChangeDay();
            bedCanva.SetActive(bedActive);
            canSelectOption = false;
            UnpauseTime();
        }
    }

    private IEnumerator SetCanSelect()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        canSelectOption = true;
    }

    public void PauseTime()
    {
        Time.timeScale = 0f;
    }

    public void UnpauseTime()
    {
        Time.timeScale = 1f;
    }
    #endregion
}
