using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "World/Dialogue Data Base")]
public class DialoguesDataBase : ScriptableObject
{
    public List<TextAsset> dialogues = new();
}
