using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSaveData
{
    public PlayerSaveDataData playerData = new();
}

[System.Serializable]
public class PlayerSaveDataData
{
    public int gold;
}
