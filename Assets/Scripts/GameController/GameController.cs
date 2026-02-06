using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    [SerializeField] private GameObject canva;
    [SerializeField] private GameObject npcs;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject weather;
    private bool canSave = false;
    private bool canSavePersistence = false;
    [SerializeField] GameObject saveIcon;
    string saveSlot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        saveSlot = BootContext.SaveSlot;
        BootContext.SaveSlot = null;

        if(BootContext.IsLoadingGame == true)
        {
            SaveSystem.Load(saveSlot);
            BootContext.IsLoadingGame = false;
        }  
    }

    void OnEnable()
    {
        Calendar_Controller.OnDayChange += SaveGameData;
        Sell_Controller.OnSellFinish += CanSave;
        SaveSystem.OnSaveStart += ShowSaveIcon;
        SaveSystem.OnSaveFinish += HideSaveIcon;
        PersistenceController.OnDayChangeFinish += CanSavePersistence;
    }

    void OnDisable()
    {
        Calendar_Controller.OnDayChange -= SaveGameData;
        Sell_Controller.OnSellFinish -= CanSave;
        SaveSystem.OnSaveStart -= ShowSaveIcon;
        SaveSystem.OnSaveFinish -= HideSaveIcon;
        PersistenceController.OnDayChangeFinish -= CanSavePersistence;
        
    }

    #region Start Game
    public void StartNewGame()
    {
        WarpTile warpTile = new WarpTile();
        warpTile.scene = SceneLocationEnum.FARM.ToString();
        warpTile.x = 32;
        warpTile.y = 30;
        warpTile.transitionType = TransitionType.Instant;
        SceneController.Instance.LoadScene(warpTile, new Vector2(32, 30));
        SetGameComponents(true);
    }
    #endregion

    #region Set Components
    private void SetGameComponents(bool active)
    {
        canva.SetActive(active);
        npcs.SetActive(active);
        player.SetActive(active);
        weather.SetActive(active);
    }
    #endregion

    #region Save Game
    private void CanSave()
    {
        canSave = true;
    }

    private void CanSavePersistence()
    {
        canSavePersistence = true;
    }

    private void SaveGameData()
    {
        StartCoroutine(SaveData());
    }

    private IEnumerator SaveData()
    {
        while(!canSave || !canSavePersistence)
        {
            yield return null;
        }

        if (string.IsNullOrEmpty(saveSlot))
        {
            Debug.LogWarning("Save skipped: no active save slot");
            yield break;
        }
        
        SaveSystem.Save(saveSlot);
        canSave = false;
        canSavePersistence = false;
    }

    private void ShowSaveIcon()
    {
        saveIcon.SetActive(true);
    }

    private void HideSaveIcon()
    {
        saveIcon.SetActive(false);
    }
    #endregion
}
