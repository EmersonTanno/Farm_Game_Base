using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor.Build.Content;

public class SaveSystem
{
    private static SaveData _saveData = new SaveData();


    public static event System.Action OnSaveStart;
    public static event System.Action OnSaveFinish;

    public static event System.Action OnLoadStart;
    public static event System.Action OnLoadFinish;


    [System.Serializable]
    public struct SaveData
    {
        public PlayerSaveData PlayerSaveData;
        public InventorySaveData InventorySaveData;
        public CalendarSaveData CalendarSaveData;
        public FarmSaveData FarmSaveData;
        public NPCSaveData NPCSaveData;
        public TaxSaveData TaxSaveData;
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

        Debug.Log($"Saved {SaveFileName(saveName)}");

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

        Status_Controller.Instance.Save(ref _saveData.PlayerSaveData);
        InventoryManager.Instance.Save(ref _saveData.InventorySaveData);
        Calendar_Controller.Instance.Save(ref _saveData.CalendarSaveData);
        PersistenceController.Instance.Save(ref _saveData.FarmSaveData);
        NPCController.Instance.Save(ref _saveData.NPCSaveData);
        Tax_System.Instance.Save(ref _saveData.TaxSaveData);
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

         Debug.Log($"Loading {SaveFileName(saveName)}");

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
    }
    #endregion
}
