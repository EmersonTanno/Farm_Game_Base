using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Time_Controll : MonoBehaviour
{
    #region Variables

    //Min and Hr count
    public int minutes = 0;
    public int hours = 0;

    //Controll Variable
    private bool canChangeTime = true;

    //Canvas
    [SerializeField] TextMeshProUGUI timerText;

    //Event
    public static event Action OnMidNightChange;
    #endregion

    #region Core
    void Awake()
    {
        UpdateCanvas();
    }
    void Update()
    {
        if (canChangeTime)
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

            if (hours > 23)
            {
                OnMidNightChange?.Invoke();
                hours = 0;
            }
        }

        UpdateCanvas();

        canChangeTime = true;
    }

    private void UpdateCanvas()
    {
        if (timerText != null)
        {
            timerText.text = $"{hours:D2}:{minutes:D2}";
        }
    }
}
