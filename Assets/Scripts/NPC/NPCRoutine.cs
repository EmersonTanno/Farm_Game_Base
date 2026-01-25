using UnityEditor.Experimental.GraphView;
using UnityEngine;

[System.Serializable]
public class NPCRoutine
{
    public int startHour;
    public int startMinute;
    public SceneLocationEnum targetLocation;
    public Vector2Int targetPosition;
    public NPCSide finalSide;
}

