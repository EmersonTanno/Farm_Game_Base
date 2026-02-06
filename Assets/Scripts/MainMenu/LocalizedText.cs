using TMPro;
using UnityEngine;

public class LocalizedText : MonoBehaviour
{
    [SerializeField] private string key;
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        UpdateText();
    }

    private void OnEnable()
    {
        MainMenuController.OnConfigChange += UpdateText;
    }

    private void OnDisable()
    {
        MainMenuController.OnConfigChange -= UpdateText;
    }

    private void UpdateText()
    {
        if (MainMenuLanguageManager.Instance == null) return;

        text.text = MainMenuLanguageManager.Instance.GetText(key);
    }
}