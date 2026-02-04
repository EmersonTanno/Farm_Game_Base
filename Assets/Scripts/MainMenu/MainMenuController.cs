using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenuController : MonoBehaviour
{
    public static MainMenuController Instance;

    [SerializeField] GameObject mainCanvas;
    [SerializeField] GameObject buttons;
    [SerializeField] GameObject loadCanvas;
    [SerializeField] Animator mainAnimator;
    [SerializeField] Animator saveLoadAnimator;
    [SerializeField] string newGameScene;
    [SerializeField] string loadGameScene;

    [SerializeField] private RectTransform canvasRect;

    #region new game variables
    public bool startNewGame = false;
    [SerializeField] private RectTransform textTransform;
    private float startY;
    private float endY;
    [SerializeField] private float scrollSpeed = 30f;
    //[SerializeField] private float endY = 800f;
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

    #region controll
    private bool selectedNewOrLoad = false;
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
        SetupTextPositions();
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
        if(selectedNewOrLoad) return;

        BootContext.IsLoadingGame = false;
        SetLoadCanvas(true);
        selectedNewOrLoad = true;
    }

    public void LoadGame()
    {
        if(selectedNewOrLoad) return;

        BootContext.IsLoadingGame = true;
        SetLoadCanvas(true);
        selectedNewOrLoad = true;
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
        mainAnimator.SetBool("save/load", active);
        saveLoadAnimator.SetBool("save/load", active);
    }

    public void ReturnToMainFromLoad()
    {
        SetLoadCanvas(false);
        selectedNewOrLoad = false;;
    }
    #endregion

    #region Setup New Game Text
    private void SetupTextPositions()
    {
        float canvasHeight = canvasRect.rect.height;
        float textHeight = textTransform.rect.height;

        startY = -canvasHeight / 2f - textHeight / 2;

        endY = canvasHeight / 2f + textHeight / 2;

        textTransform.anchoredPosition =
            new Vector2(textTransform.anchoredPosition.x, startY);
    }
    #endregion
}