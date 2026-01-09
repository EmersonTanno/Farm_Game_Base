using UnityEngine;

public class JsonManager : MonoBehaviour
{
    public TextAsset textFile;

    [System.Serializable]
    public class JsonData
    {
        public int id;
        public string Text;
    }

    [System.Serializable]
    public class JsonDataList
    {
        public JsonData[] jsonData;
    }

    public JsonData[] data;

    private void Start()
    {
        JsonDataList jsonDataList =
            JsonUtility.FromJson<JsonDataList>(textFile.text);

        data = jsonDataList.jsonData;

        Debug.Log("Quantidade de diálogos: " + data.Length);
    }
}
