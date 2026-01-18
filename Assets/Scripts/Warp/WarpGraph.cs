using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "World/Warp Graph")]
public class WarpGraph : ScriptableObject
{
    public List<WarpNode> nodes = new();

    public WarpNode GetOrCreateNode(SceneLocationEnum scene)
    {
        WarpNode node = nodes.Find(n => n.scene == scene);

        if (node == null)
        {
            node = new WarpNode
            {
                scene = scene,
                warps = new List<WarpData>()
            };

            nodes.Add(node);
        }

        return node;
    }
}
