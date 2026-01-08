using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCData
{
    public int id;

    public float hearts = 0;

    public SceneLocationEnum location;
    public Vector2Int gridPosition;

    public NPCStateEnum state;
    public int routineIndex;
    public List<NPCRoutine> routine;
}

