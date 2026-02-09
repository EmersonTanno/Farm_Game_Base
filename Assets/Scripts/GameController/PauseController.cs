using UnityEngine;

public class PauseController : MonoBehaviour
{
    public static PauseController Instance;
    [SerializeField] GameObject pauseCanvasGroup;
    [SerializeField] GameObject pauseCanvas;
    [SerializeField] GameObject settingsCanvas;
    [SerializeField] Animator pauseAnimator;

    private bool gamePaused = false;

    void Awake()
    {
        Instance = this;
        SetPauseCanvas(false);
        SetSettingCanvas(false);
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
        Time_Controll.Instance.UnpauseTime();
        pauseCanvasGroup.SetActive(false);
        gamePaused = false;
    }

    public void SetPauseCanvas(bool active)
    {
        pauseCanvas.SetActive(active);
    }

    public void SetSettingCanvas(bool active)
    {
        settingsCanvas.SetActive(active);
    }
}