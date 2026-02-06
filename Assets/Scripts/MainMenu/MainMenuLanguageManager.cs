using TMPro;
using UnityEngine;

public class MainMenuLanguageManager : MonoBehaviour
{
    public static MainMenuLanguageManager Instance;

    [SerializeField] TextMeshProUGUI newGameButton;
    [SerializeField] string newGameButtonTextPt;
    [SerializeField] string newGameButtonTextEn;
    [SerializeField] TextMeshProUGUI loadGameButton;
    [SerializeField] string loadGameButtonTextPt;
    [SerializeField] string loadGameButtonTextEn;
    [SerializeField] TextMeshProUGUI setingsButton;
    [SerializeField] string setingsButtonTextPt;
    [SerializeField] string setingsButtonTextEn;   
    [SerializeField] TextMeshProUGUI languageField;
    [SerializeField] string languageFieldTextPt;
    [SerializeField] string languageFieldTextEn;  
    [SerializeField] TextMeshProUGUI returnButton1;
    [SerializeField] TextMeshProUGUI returnButton2;
    [SerializeField] string returnButtonTextPt;
    [SerializeField] string returnButtonTextEn;  
    [SerializeField] TextMeshProUGUI applyButton;
    [SerializeField] string applyButtonTextPt;
    [SerializeField] string applyButtonTextEn;  

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void OnEnable()
    {
        MainMenuController.OnConfigChange += ReloadGameLanguage;
    }

    private void ReloadGameLanguage()
    {
        switch(GameConfigurations.Instance.gameLanguage)
        {
            case LanguageEnum.Potugues:
            {
                newGameButton.text = newGameButtonTextPt;
                loadGameButton.text = loadGameButtonTextPt;
                setingsButton.text = setingsButtonTextPt;
                returnButton1.text = returnButtonTextPt;
                returnButton2.text = returnButtonTextPt;
                applyButton.text = applyButtonTextPt;
                break;
            }
            case LanguageEnum.Ingles:
            {
                newGameButton.text = newGameButtonTextEn;
                loadGameButton.text = loadGameButtonTextEn;
                setingsButton.text = setingsButtonTextEn;
                returnButton1.text = returnButtonTextEn;
                returnButton2.text = returnButtonTextEn;
                applyButton.text = applyButtonTextEn;
                break;
            }
        }
    }
}
