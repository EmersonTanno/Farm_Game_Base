using UnityEngine;
using System.IO;

public class SaveSystem
{
    private static SaveData _saveData = new SaveData();


    public static event System.Action OnSaveStart;
    public static event System.Action OnSaveFinish;

    public static event System.Action OnLoadStart;
    public static event System.Action OnLoadFinish;


    //public ConfigurationSaveData ConfigurationSaveData = new ConfigurationSaveData();


    [System.Serializable]
    public struct SaveData
    {
        public PlayerSaveData PlayerSaveData;
        public InventorySaveData InventorySaveData;
        public CalendarSaveData CalendarSaveData;
        public FarmSaveData FarmSaveData;
        public NPCSaveData NPCSaveData;
        public TaxSaveData TaxSaveData;
        public WeatherSaveData WeatherSaveData;
    }

    #region File Names
    public static string SaveFileName(string saveName)
    {
        string dir = Path.Combine(Application.persistentDataPath, "save");

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        return Path.Combine(dir, saveName + ".txt");
    }

    public static string MetaFileName(string saveName)
    {
        string dir = Path.Combine(Application.persistentDataPath, "save");

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        return Path.Combine(dir, saveName + ".meta");
    }

    public static string ConfigurationFileName()
    {
        string dir = Path.Combine(Application.persistentDataPath, "save");

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        return Path.Combine(dir, "configuration" + ".txt");
    }
    #endregion

    #region Save Game
    public static void Save(string saveName)
    {
        if (string.IsNullOrEmpty(saveName))
        {
            Debug.LogError("Save failed: saveName is null or empty");
            return;
        }

        OnSaveStart?.Invoke();

        HandleSaveData();

        WriteMainSave(saveName);
        WriteMetaSave(saveName);

        OnSaveFinish?.Invoke();
    }


    private static void HandleSaveData()
    {
        if(_saveData.PlayerSaveData == null)
            _saveData.PlayerSaveData = new PlayerSaveData();
        if(_saveData.InventorySaveData == null)
            _saveData.InventorySaveData = new InventorySaveData();
        if(_saveData.CalendarSaveData == null)
            _saveData.CalendarSaveData = new CalendarSaveData();
        if (_saveData.FarmSaveData == null)
            _saveData.FarmSaveData = new FarmSaveData();
        if (_saveData.NPCSaveData == null)
            _saveData.NPCSaveData = new NPCSaveData();
        if (_saveData.TaxSaveData == null)
            _saveData.TaxSaveData = new TaxSaveData();
        if(_saveData.WeatherSaveData == null)
            _saveData.WeatherSaveData = new WeatherSaveData();

        Status_Controller.Instance.Save(ref _saveData.PlayerSaveData);
        InventoryManager.Instance.Save(ref _saveData.InventorySaveData);
        Calendar_Controller.Instance.Save(ref _saveData.CalendarSaveData);
        PersistenceController.Instance.Save(ref _saveData.FarmSaveData);
        NPCController.Instance.Save(ref _saveData.NPCSaveData);
        Tax_System.Instance.Save(ref _saveData.TaxSaveData);
        WeatherController.Instance.Save(ref _saveData.WeatherSaveData);
    }

    private static void WriteMainSave(string saveName)
    {
        File.WriteAllText(
            SaveFileName(saveName),
            JsonUtility.ToJson(_saveData, true)
        );
    }

    private static void WriteMetaSave(string saveName)
    {
        SaveMetaData meta = new SaveMetaData
        {
            gold = _saveData.PlayerSaveData.playerData.gold,
            day = _saveData.CalendarSaveData.calendar.day,
            month = _saveData.CalendarSaveData.calendar.month,
            year = _saveData.CalendarSaveData.calendar.year,
            season = _saveData.CalendarSaveData.calendar.season,
            lastPlayed = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm")
        };

        string path = MetaFileName(saveName);
        File.WriteAllText(path, JsonUtility.ToJson(meta, true));
    }
    #endregion

    #region Load Game
    public static void Load(string saveName)
    {
        if (string.IsNullOrEmpty(saveName))
        {
            Debug.LogError("Load failed: saveName is null or empty");
            return;
        }

        OnLoadStart?.Invoke();

        if (!File.Exists(SaveFileName(saveName)))
        {
            Debug.LogWarning("Save file not found!");
            OnLoadFinish?.Invoke();
            return;
        }

        string saveContent = File.ReadAllText(SaveFileName(saveName));
        _saveData = JsonUtility.FromJson<SaveData>(saveContent);

        HandleLoadData();

        OnLoadFinish?.Invoke();
    }

    private static void HandleLoadData()
    {
        Status_Controller.Instance.Load(_saveData.PlayerSaveData);
        InventoryManager.Instance.Load(_saveData.InventorySaveData);
        Calendar_Controller.Instance.Load(_saveData.CalendarSaveData);
        PersistenceController.Instance.Load(_saveData.FarmSaveData);
        NPCController.Instance.Load(_saveData.NPCSaveData);
        Tax_System.Instance.Load(_saveData.TaxSaveData);
        WeatherController.Instance.Load(_saveData.WeatherSaveData);
    }
    #endregion

    #region Load Meta Data
    public static SaveMetaData LoadMetaData(string saveId)
    {
        if (string.IsNullOrEmpty(saveId))
        {
            Debug.LogError("Load failed: saveId is null or empty");
            return null;
        }

        if (!File.Exists(MetaFileName(saveId)))
        {
            OnLoadFinish?.Invoke();
            return null;
        }

        string saveContent = File.ReadAllText(MetaFileName(saveId));
        SaveMetaData metaData = JsonUtility.FromJson<SaveMetaData>(saveContent);

        return metaData;
    }
    #endregion

    #region Save Configuration
    public static void SaveConfigurations(LanguageEnum language)
    {
        WriteConfigurationSave(language);
    }


    private static void WriteConfigurationSave(LanguageEnum language)
    {
        ConfigurationSaveData configurationSaveData = new ConfigurationSaveData
        {
            gameLanguage= language,
        };

        File.WriteAllText(
            ConfigurationFileName(),
            JsonUtility.ToJson(configurationSaveData, true)
        );
    }

    public static LanguageEnum LoadGameConfiguration()
    {
        if (File.Exists(ConfigurationFileName()))
        {
            string json = File.ReadAllText(SaveSystem.ConfigurationFileName());
            ConfigurationSaveData data =
                JsonUtility.FromJson<ConfigurationSaveData>(json);

            return data.gameLanguage;
        }
        return LanguageEnum.Ingles;
    }
    #endregion
}
