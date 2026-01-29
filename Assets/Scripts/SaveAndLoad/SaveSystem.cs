using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor.Build.Content;

public class SaveSystem
{
    private static SaveData _saveData = new SaveData();

    [System.Serializable]
    public struct SaveData
    {
        public FarmSaveData FarmSaveData;
        public NPCSaveData NPCSaveData;
    }

    public static string SaveFileName()
    {
        string saveFile = Application.persistentDataPath + "/save" + ".txt";
        return saveFile;
    }

    public static void Save()
    {
        HandleSaveData();
        Debug.Log($"Saved {SaveFileName()}");
        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(_saveData, true));
    }

    private static void HandleSaveData()
    {
        if (_saveData.FarmSaveData == null)
            _saveData.FarmSaveData = new FarmSaveData();
        if (_saveData.NPCSaveData == null)
            _saveData.NPCSaveData = new NPCSaveData();

        PersistenceController.Instance.Save(ref _saveData.FarmSaveData);
        NPCController.Instance.Save(ref _saveData.NPCSaveData);
    }

    public static void Load()
    {
        string saveContent = File.ReadAllText(SaveFileName());
        _saveData = JsonUtility.FromJson<SaveData>(saveContent);
        HandleLoadData();
    }

    private static void HandleLoadData()
    {
        PersistenceController.Instance.Load(_saveData.FarmSaveData);
        NPCController.Instance.Load(_saveData.NPCSaveData);
    }
}
