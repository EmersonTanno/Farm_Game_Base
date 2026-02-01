using System.Collections;
using UnityEngine;


public class GameController : MonoBehaviour
{
    public static GameController Instance;
    [SerializeField] private GameObject canva;
    [SerializeField] private GameObject npcs;
    private bool canSave = false;
    [SerializeField] GameObject saveIcon;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(canva);
        DontDestroyOnLoad(npcs);
    }

    void OnEnable()
    {
        Calendar_Controller.OnDayChange += SaveGameData;
        Sell_Controller.OnSellFinish += CanSave;
        SaveSystem.OnSaveStart += ShowSaveIcon;
        SaveSystem.OnSaveFinish += HideSaveIcon;
    }

    void OnDisable()
    {
        Calendar_Controller.OnDayChange -= SaveGameData;
        Sell_Controller.OnSellFinish -= CanSave;
    }

    private void CanSave()
    {
        canSave = true;
    }

    private void SaveGameData()
    {
        StartCoroutine(SaveData());
    }

    private IEnumerator SaveData()
    {
        while(!canSave)
        {
            yield return null;
        }
        
        SaveSystem.Save();
        canSave = false;
    }

    private void ShowSaveIcon()
    {
        saveIcon.SetActive(true);
    }

    private void HideSaveIcon()
    {
        saveIcon.SetActive(false);
    }
}
