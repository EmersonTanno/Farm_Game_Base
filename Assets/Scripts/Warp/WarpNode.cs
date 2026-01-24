using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WarpNode
{
    public SceneLocationEnum scene;
    public List<WarpData> warps = new();
}
