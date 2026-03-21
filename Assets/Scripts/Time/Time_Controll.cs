using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Time_Controll : MonoBehaviour
{
    public static Time_Controll Instance { get; private set; }

    #region Variables
    //Min and Hr count
    public int minutes = 0;
    public int hours = 0;
    [SerializeField] TextMeshProUGUI dataText;
    public bool timerPaused = false;

    //Controll Variable
    private bool canChangeTime = true;

    //Canvas
    [SerializeField] TextMeshProUGUI timerText;

    //Bed
    [SerializeField] GameObject bedCanva;
    [SerializeField] TextMeshProUGUI questionText;
    [SerializeField] TextMeshProUGUI yesButtonText;
    [SerializeField] TextMeshProUGUI noButtonText;
    private bool canSelectOption = false;
    public bool bedActive = false;

    //Event
    public static event Action OnMidNightChange;
    public static event Action OnHourChange;
    public static event Action OnMinuteChange;
    #endregion

    #region Core
    void Awake()
    {
        Instance = this;
        UpdateCanvas();
    }

    void Start()
    {
        SetBedCanvas();
        SetTimeCanvas();
    }
    void Update()
    {
        if (canChangeTime && !bedActive)
        {
            StartCoroutine(UpdateTime());
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(timerPaused)
            {
                UnpauseTimer();
            } else
            {
                PauseTimer();
            }
        }
    }

    void OnEnable()
    {
        GameLanguageManager.OnLanguageChange += SetBedCanvas;
        GameLanguageManager.OnLanguageChange += SetTimeCanvas;
    }

    void OnDisable()
    {
        GameLanguageManager.OnLanguageChange -= SetBedCanvas;
        GameLanguageManager.OnLanguageChange -= SetTimeCanvas;
    }
    #endregion

    #region Time Functions
    private IEnumerator UpdateTime()
    {
        canChangeTime = false;

        for(int i = 0; i < 5; i++)
        {
            while(timerPaused)
            {
                yield return null;
            }
            yield return new WaitForSeconds(1f);
        }

        if (minutes < 50)
        {
            minutes += 10;
            OnMinuteChange?.Invoke();
        }
        else
        {
            hours += 1;
            minutes = 0;
            OnMinuteChange?.Invoke();
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
    #endregion

    #region Bed
    public void ActivateBedCanvas()
    {
        GameSession gameSession = GameSession.Instance;
        if(gameSession.gameState == GameState.Paused || gameSession.gameState == GameState.PausedCutscene || gameSession.gameState == GameState.Dialogue || gameSession.gameState == GameState.PausedDialogue || gameSession.gameState == GameState.Cutscene)
        {
            return;
        }
        
        bedActive = true;
        bedCanva.SetActive(bedActive);
        PauseTimer();
        StartCoroutine(SetCanSelect());
    }

    public void CancelBedCanvas()
    {
        if (canSelectOption)
        {
            bedActive = false;
            bedCanva.SetActive(bedActive);
            canSelectOption = false;
            UnpauseTimer();
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

    #region Timer Controller
    public void PauseTimer()
    {
        if(timerPaused) return;
        timerPaused = true;
    }

    public void UnpauseTimer()
    {
        if(!timerPaused) return;
        timerPaused = false;
    }
    #endregion

    #region TimeCanvas
    private void SetTimeCanvas()
    {
        dataText.text = GameLanguageManager.Instance.GetTimeMenuItemName("date");
    }
    #endregion
    #region BedCanvas
    private void SetBedCanvas()
    {
        questionText.text = GameLanguageManager.Instance.GetSleepMenuItemName("sleep");
        yesButtonText.text = GameLanguageManager.Instance.GetSleepMenuItemName("yes");
        noButtonText.text = GameLanguageManager.Instance.GetSleepMenuItemName("no");
    }
    #endregion
}
