using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CutsceneData : ScriptableObject
{
    public List<CutsceneStep> steps;
    public int npcId;
    public Vector2Int initialNPCPos;
}
