using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class PauseController : MonoBehaviour
{
    public static PauseController Instance;
    [SerializeField] GameObject pauseCanvasGroup;
    [SerializeField] GameObject pauseCanvas;
    [SerializeField] GameObject settingsCanvas;
    [SerializeField] Image backGroundCanvas;
    [SerializeField] Animator pauseAnimator;

    private bool canSelect = false;

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
        StartCoroutine(SetBackGround());
        pauseCanvasGroup.SetActive(true);
        pauseAnimator.SetTrigger("pause");
        gamePaused = true;
    }

    public void ContinueGame()
    {
        if(!canSelect) return;
        SetBackGroundOpacity(0);
        pauseCanvas.SetActive(false);
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

    public void SetCanSelect(bool can)
    {
        canSelect = can;
    }

    public void SetBackGroundOpacity(float value)
    {
        Color c = backGroundCanvas.color;
        c.a = Mathf.Clamp01(value);
        backGroundCanvas.color = c;
    }

    private IEnumerator SetBackGround()
    {
        for(float i = 0; i < 0.7f; i+=0.1f)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            SetBackGroundOpacity(i);
        }
    }
}