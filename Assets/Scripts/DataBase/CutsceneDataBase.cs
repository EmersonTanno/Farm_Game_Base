using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "World/Cutscene Data Base")]
public class CutsceneDataBase : ScriptableObject
{
    public List<CutsceneData> cutscenes = new();

    public CutsceneData GetCutscene(int id)
    {
        return cutscenes.Find(t => t.id == id);
    }
}
