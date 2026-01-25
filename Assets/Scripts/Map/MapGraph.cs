using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "World/Map Graph")]
public class MapGraph : ScriptableObject
{
    public List<MapData> maps = new();

    static readonly Vector2Int[] directions =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    public MapData GetOrCreateMap(SceneLocationEnum scene)
    {
        MapData map = maps.Find(n => n.scene == scene);

        if (map == null)
        {
            map = new MapData
            {
                scene = scene
            };

            maps.Add(map);
        }

        return map;
    }

    public int GetTile(SceneLocationEnum scene, int x, int y)
    {
        MapData map = maps.Find(m => m.scene == scene);
        if (map == null) return 0;

        if (x < 0 || y < 0 || x >= map.width || y >= map.height)
            return 0;

        return map.Get(x, y);
    }

    private bool IsWalkable(SceneLocationEnum scene, Vector2Int pos)
    {
        int tile = GetTile(scene, pos.x, pos.y);
        return tile == 1 || tile == 2;
    }

    private int GetMoveCost(SceneLocationEnum scene, Vector2Int pos)
    {
        return GetTile(scene, pos.x, pos.y) == 2 ? 1 : 10;
    }

    public List<Vector2Int> GetPath(
        SceneLocationEnum scene,
        Vector2Int start,
        Vector2Int target
    )
    {
        List<Node> openSet = new();
        HashSet<Vector2Int> closedSet = new();

        Node startNode = new(start);
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node current = openSet[0];

            foreach (Node n in openSet)
                if (n.fCost < current.fCost)
                    current = n;

            openSet.Remove(current);
            closedSet.Add(current.pos);

            if (current.pos == target)
                return RetracePath(current);

            foreach (Vector2Int dir in directions)
            {
                Vector2Int nextPos = current.pos + dir;

                if (!IsWalkable(scene, nextPos) || closedSet.Contains(nextPos))
                    continue;

                int newCost = current.gCost + GetMoveCost(scene, nextPos);

                Node neighbor = openSet.Find(n => n.pos == nextPos);
                if (neighbor == null)
                {
                    neighbor = new Node(nextPos);
                    neighbor.gCost = newCost;
                    neighbor.hCost = Manhattan(nextPos, target);
                    neighbor.parent = current;
                    openSet.Add(neighbor);
                }
                else if (newCost < neighbor.gCost)
                {
                    neighbor.gCost = newCost;
                    neighbor.parent = current;
                }
            }
        }

        return null;
    }


    private int Manhattan(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private List<Vector2Int> RetracePath(Node endNode)
    {
        List<Vector2Int> path = new();
        Node current = endNode;

        while (current.parent != null)
        {
            path.Add(current.pos);
            current = current.parent;
        }

        path.Reverse();
        return path;
    }



}
