using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenuController : MonoBehaviour
{
    public static MainMenuController Instance;

    [SerializeField] GameObject mainCanvas;
    [SerializeField] GameObject buttons;
    [SerializeField] GameObject loadCanvas;
    [SerializeField] string newGameScene;
    [SerializeField] string loadGameScene;


    #region new game variables
    public bool startNewGame = false;
    [SerializeField] private RectTransform textTransform;
    [SerializeField] private float scrollSpeed = 30f;
    [SerializeField] private float endY = 800f;
    [SerializeField] private Image backGroundIntro;
    [SerializeField] private GameObject backGroundIntroObj;
    [SerializeField] private float fadeSpeed = 1f;
    private float bgAlpha = 0f;
    private bool canSkipIntro = false;
    private bool skipIntro = false;
    #endregion

    #region load game variables
    public bool loadGame = false;
    #endregion

    void Awake()
    {
        Instance = this;
    }

    #region Default
    void Start()
    {
        backGroundIntroObj.SetActive(false);
        SetBackgroundAlpha(0f);
    }

    void Update()
    {
        if (!startNewGame && !loadGame) return;

        backGroundIntroObj.SetActive(true);

        if (bgAlpha < 1f)
        {
            bgAlpha += fadeSpeed * Time.deltaTime;
            SetBackgroundAlpha(bgAlpha);
        }
        else
        {
            if(loadGame)
            {
                SceneManager.LoadScene(loadGameScene);
            }
            
            canSkipIntro = true;
            if(skipIntro)
            {
                SceneManager.LoadScene(newGameScene);
            }
            buttons.SetActive(false);

            textTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

            if (textTransform.anchoredPosition.y >= endY)
            {
                SceneManager.LoadScene(newGameScene);
            }
        }
    }
    #endregion

    #region Start/Load
    public void StartNewGame()
    {
        BootContext.IsLoadingGame = false;
        SetLoadCanvas(true);
    }

    public void LoadGame()
    {
        BootContext.IsLoadingGame = true;
        SetLoadCanvas(true);
    }
    #endregion

    #region Auxiliar
    private void SetBackgroundAlpha(float alpha)
    {
        Color c = backGroundIntro.color;
        c.a = Mathf.Clamp01(alpha);
        backGroundIntro.color = c;
    }
    #endregion

    #region Input
    public void Skip()
    {
        if(!startNewGame || !canSkipIntro) return;

        skipIntro = true;
    }
    #endregion

    #region Saves
    public void SetLoadCanvas(bool active)
    {
        loadCanvas.SetActive(active);
        buttons.SetActive(!active);
        mainCanvas.SetActive(!active);
    }

    public void ReturnToMainFromLoad()
    {
        SetLoadCanvas(false);
    }
    #endregion
}