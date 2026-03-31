using System.IO;
using UnityEngine;

public class GameConfigurations : MonoBehaviour
{
    public static GameConfigurations Instance { get; private set; }

    public LanguageEnum gameLanguage = LanguageEnum.Potugues;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadConfiguration();
    }

    public void SetLanguage(LanguageEnum lang)
    {
        gameLanguage = lang;
        SaveSystem.SaveConfigurations(gameLanguage);
    }


    private void LoadConfiguration()
    {
        gameLanguage = SaveSystem.LoadGameConfiguration();
    }
}