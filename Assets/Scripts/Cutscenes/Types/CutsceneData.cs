using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CutsceneData : ScriptableObject
{
    public int id;
    public string cutsceneName;
    public List<CutsceneStep> steps;
    public List<CutsceneNPCData> npcs;
}
