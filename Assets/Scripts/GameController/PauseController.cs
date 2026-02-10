using UnityEngine;

public class PauseController : MonoBehaviour
{
    public static PauseController Instance;
    [SerializeField] GameObject pauseCanvasGroup;
    [SerializeField] GameObject pauseCanvas;
    [SerializeField] GameObject settingsCanvas;
    [SerializeField] GameObject backGroundCanvas;
    [SerializeField] Animator pauseAnimator;

    private bool canSelect = false;

    public bool gamePaused = false;

    void Awake()
    {
        Instance = this;
        SetPauseCanvas(false);
        SetSettingCanvas(false);
        SetBackGround(false);
    }

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

}