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
        //implementação futura
        //LoadConfig();
    }

    public void SetLanguage(LanguageEnum lang)
    {
        gameLanguage = lang;
        //implementação futura
        //SaveConfig();
    }
}