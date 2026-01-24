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

    public List<SceneLocationEnum> GetPath(
        SceneLocationEnum start,
        SceneLocationEnum target
    )
    {
        if (start == target)
            return new List<SceneLocationEnum> { start };

        Queue<SceneLocationEnum> queue = new();
        Dictionary<SceneLocationEnum, SceneLocationEnum> cameFrom = new();

        queue.Enqueue(start);
        cameFrom[start] = start;

        while (queue.Count > 0)
        {
            SceneLocationEnum current = queue.Dequeue();

            WarpNode node = nodes.Find(n => n.scene == current);
            if (node == null)
                continue;

            foreach (WarpData warp in node.warps)
            {
                SceneLocationEnum next = warp.toScene;

                if (cameFrom.ContainsKey(next))
                    continue;

                cameFrom[next] = current;

                if (next == target)
                {
                    return ReconstructPath(cameFrom, start, target);
                }

                queue.Enqueue(next);
            }
        }

        Debug.LogWarning($"Nenhum caminho encontrado de {start} para {target}");
        return null;
    }

    private List<SceneLocationEnum> ReconstructPath(
        Dictionary<SceneLocationEnum, SceneLocationEnum> cameFrom,
        SceneLocationEnum start,
        SceneLocationEnum end
    )
    {
        List<SceneLocationEnum> path = new();
        SceneLocationEnum current = end;

        while (!current.Equals(start))
        {
            path.Add(current);
            current = cameFrom[current];
        }

        path.Add(start);
        path.Reverse();

        return path;
    }


}
