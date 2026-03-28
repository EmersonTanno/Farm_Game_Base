using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCSaveData
{
    public List<NPCSaveDataData> npcs = new();
}

[System.Serializable]
public class NPCSaveDataData
{
    public string id;
    public int hearts = 0;
}
