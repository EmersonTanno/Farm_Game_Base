using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuController : MonoBehaviour
{
    [SerializeField] string newGameScene;
    [SerializeField] string loadGameScene;
    public void StartNewGame()
    {
        BootContext.IsLoadingGame = false;
        BootContext.SaveSlot = null;
        SceneManager.LoadScene(newGameScene);
    }

    public void LoadGame()
    {
        BootContext.IsLoadingGame = true;
        BootContext.SaveSlot = "1";
        SceneManager.LoadScene(loadGameScene);
    }
}