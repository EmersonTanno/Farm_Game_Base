using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Time_Controll : MonoBehaviour
{
    #region Variables
    public int minutes = 0;
    public int hours = 0;
    private bool canChangeTime = true;

    //Canvas
    [SerializeField] TextMeshProUGUI timerText;

    // Evento público que outros scripts podem escutar
    public static event Action<int, int> OnTimeChanged;
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
        yield return new WaitForSeconds(10f);

        if (minutes < 50)
        {
            minutes += 10;
        }
        else
        {
            hours += 1;
            minutes = 0;
        }

        UpdateCanvas();
        // Dispara o evento passando horas e minutos
        OnTimeChanged?.Invoke(hours, minutes);

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
