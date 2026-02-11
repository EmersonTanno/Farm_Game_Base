using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotMenu : MonoBehaviour
{
    [SerializeField] string saveId;
    [SerializeField] Button button;
    [SerializeField] GameObject nullField;
    [SerializeField] GameObject metaDataField;
    [SerializeField] TextMeshProUGUI goldText;
    [SerializeField] TextMeshProUGUI dayText;
    [SerializeField] TextMeshProUGUI yearText;
    [SerializeField] TextMeshProUGUI seasonText;
    [SerializeField] TextMeshProUGUI lastPlayedText;

    private SaveMetaData metaData;

    private void Start() {
        metaData = SaveSystem.LoadMetaData(saveId);

        if(metaData == null)
        {
            metaDataField.SetActive(false);
            nullField.SetActive(true);

            if(BootContext.IsLoadingGame)
            {
                button.enabled = false;
            }
            else
            {
                button.enabled = true;
            }
        }
        else
        {
            metaDataField.SetActive(true);
            nullField.SetActive(false);

            button.enabled = true;

            //SetData();
        }
    }

    void OnEnable()
    {
        MainMenuController.OnConfigChange += StartSetDataLate;
    }

    void OnDisable()
    {
        MainMenuController.OnConfigChange -= StartSetDataLate;
    }

    public void SelectSlot()
    {
        if(MainMenuController.Instance.startNewGame || MainMenuController.Instance.loadGame) return;

        BootContext.SaveSlot = saveId;
        if(BootContext.IsLoadingGame)
        {
            MainMenuController.Instance.loadGame = true;
        }
        else
        {
            MainMenuController.Instance.startNewGame = true;
            MainMenuController.Instance.SetupTextPositions();
        }
    }

    private void SetData()
    {
        goldText.text += metaData.gold;
        dayText.text += metaData.day;
        yearText.text += metaData.year;
        seasonText.text += metaData.season;
        lastPlayedText.text += metaData.lastPlayed;
    }

    private void StartSetDataLate()
    {
        StartCoroutine(SetDataLate());
    }

    private IEnumerator SetDataLate()
    {
        yield return null;
        SetData();
    }
}