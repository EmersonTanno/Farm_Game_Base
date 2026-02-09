using System.Collections;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    public static PauseController Instance;
    [SerializeField] GameObject pauseCanvas;

    private bool gamePaused = false;

    void Awake()
    {
        Instance = this;
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
        pauseCanvas.SetActive(true);
        gamePaused = true;
    }

    public void ContinueGame()
    {
        Time_Controll.Instance.UnpauseTime();
        pauseCanvas.SetActive(false);
        gamePaused = false;
    }
}