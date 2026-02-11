using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    public static PauseController Instance;
    [SerializeField] GameSession gameSession;
    [SerializeField] GameObject pauseCanvasGroup;
    [SerializeField] GameObject pauseCanvas;
    [SerializeField] GameObject settingsCanvas;
    [SerializeField] GameObject backGroundCanvas;
    [SerializeField] Animator pauseAnimator;
    [SerializeField] private TMP_Dropdown languageSelector;

    //texts
    [SerializeField] TextMeshProUGUI pauseText;
    [SerializeField] TextMeshProUGUI resumeText;
    [SerializeField] TextMeshProUGUI settingsText;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI exitText;
    [SerializeField] TextMeshProUGUI languageText;
    [SerializeField] TextMeshProUGUI applyText;
    [SerializeField] TextMeshProUGUI returnText;

    private bool canSelect = false;

    public bool gamePaused = false;

    public static event Action OnLanguageChange;

    #region Core
    void Awake()
    {
        Instance = this;
        SetPauseCanvas(false);
        SetSettingCanvas(false);
        SetBackGround(false);
    }

    void Start()
    {
        SetPauseLanguage();
        SetLanguageSelector();
    }

    void OnEnable()
    {
        OnLanguageChange += SetPauseLanguage;
    }

    void OnDisable()
    {
        OnLanguageChange -= SetPauseLanguage;
    }
    #endregion

    public void EscButtonPresses()
    {
        if(gamePaused)
        {
            ContinueGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        Time_Controll.Instance.PauseTime();
        pauseCanvasGroup.SetActive(true);
        pauseAnimator.SetTrigger("pause");
        gamePaused = true;
    }

    public void ContinueGame()
    {
        if(!canSelect) return;
        SetBackGround(false);
        pauseCanvas.SetActive(false);
        Time_Controll.Instance.UnpauseTime();
        pauseCanvasGroup.SetActive(false);
        gamePaused = false;
    }

    public void SetBackGround(bool active)
    {
        backGroundCanvas.SetActive(active);
    }

    public void SetPauseCanvas(bool active)
    {
        pauseCanvas.SetActive(active);
        SetBackGround(true);
    }

    public void SetSettingCanvas(bool active)
    {
        settingsCanvas.SetActive(active);
    }

    public void SetCanSelect(bool can)
    {
        canSelect = can;
    }

    #region change pages
    public void FromPauseToSettings()
    {
        SetSettingCanvas(true);
        SetCanSelect(false);
        pauseAnimator.SetBool("settings", true);
    }

    public void FromSettingsToPause()
    {
        SetCanSelect(false);
        pauseAnimator.SetBool("settings", false);
    }
    #endregion

    #region to title page
    public void ChangeToTitlePage()
    {
        Time_Controll.Instance.UnpauseTime();
        SceneManager.LoadScene("HomeScreen");
        GameSession.Instance.KillSession();
    }
    #endregion

    #region exit game
    public void ExitGame()
    {
        Application.Quit();
    }
    #endregion

    #region settings
    public void ApplySettings()
    {
        SetCanSelect(false);
        pauseAnimator.SetTrigger("applySettings");
    }

    public void SetLanguageSelector()
    {
        if(GameConfigurations.Instance.gameLanguage == LanguageEnum.Potugues)
            languageSelector.value = 0;
        if(GameConfigurations.Instance.gameLanguage == LanguageEnum.Ingles)
            languageSelector.value = 1;
    }

    public void ChangeGameLanguage()
    {
        SetLanguage();
        OnLanguageChange?.Invoke();
    }

    private void SetLanguage()
    {
        LanguageEnum newLanguage;
        switch(languageSelector.value)
        {
            case 0:
                newLanguage = LanguageEnum.Potugues;
            break;

            case 1:
                newLanguage = LanguageEnum.Ingles;
            break;

            default:
                newLanguage = LanguageEnum.Potugues;
            break;
        }
        GameConfigurations.Instance.SetLanguage(newLanguage);
    }
    #endregion

    #region language
    private void SetPauseLanguage()
    {
        pauseText.text = GameLanguageManager.Instance.GetPauseMenuItemName("pause");
        resumeText.text = GameLanguageManager.Instance.GetPauseMenuItemName("resume");
        settingsText.text = GameLanguageManager.Instance.GetPauseMenuItemName("settings");
        titleText.text = GameLanguageManager.Instance.GetPauseMenuItemName("title");
        exitText.text = GameLanguageManager.Instance.GetPauseMenuItemName("exit");
        languageText.text = GameLanguageManager.Instance.GetPauseMenuItemName("language");
        applyText.text = GameLanguageManager.Instance.GetPauseMenuItemName("apply");
        returnText.text = GameLanguageManager.Instance.GetPauseMenuItemName("return");
    }
    #endregion

}