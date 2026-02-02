using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenuController : MonoBehaviour
{
    [SerializeField] GameObject buttons;
    [SerializeField] string newGameScene;
    [SerializeField] string loadGameScene;


    #region new game variables
    private bool startNewGame = false;
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
    private bool loadGame = false;
    #endregion


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
        if(startNewGame) return;
        BootContext.IsLoadingGame = false;
        BootContext.SaveSlot = null;
        startNewGame = true;
    }

    public void LoadGame()
    {
        if(startNewGame) return;
        BootContext.IsLoadingGame = true;
        BootContext.SaveSlot = "1";
        loadGame = true;
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
}