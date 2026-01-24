using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SceneConnection
{
    public SceneLocationEnum scene;
    public List<SceneLocationEnum> warps;
}

public class ScenesConections : MonoBehaviour
{
    public static ScenesConections Instance; 

    [SerializeField]
    public List<SceneConnection> connections = new List<SceneConnection>();

    void Awake()
    {
        Instance = this;
    }

    public List<SceneLocationEnum> GetPathToLocation(SceneLocationEnum actualLocation, SceneLocationEnum target)
    {
        if (actualLocation == target)
        {
            return new List<SceneLocationEnum> { actualLocation };
        }

        Queue<SceneLocationEnum> queue = new Queue<SceneLocationEnum>();
        Dictionary<SceneLocationEnum, SceneLocationEnum> cameFrom =
            new Dictionary<SceneLocationEnum, SceneLocationEnum>();

        queue.Enqueue(actualLocation);
        cameFrom[actualLocation] = actualLocation;

        while (queue.Count > 0)
        {
            SceneLocationEnum current = queue.Dequeue();

            SceneConnection connection = FindConnection(current);
            if (connection == null) continue;

            foreach (SceneLocationEnum next in connection.warps)
            {
                if (cameFrom.ContainsKey(next))
                    continue;

                cameFrom[next] = current;

                if (next == target)
                {
                    return ReconstructPath(cameFrom, actualLocation, target);
                }

                queue.Enqueue(next);
            }
        }

        Debug.LogWarning(
            $"Nenhum caminho encontrado de {actualLocation} para {target}"
        );
        return null;
    }

    private List<SceneLocationEnum> ReconstructPath(
        Dictionary<SceneLocationEnum, SceneLocationEnum> cameFrom,
        SceneLocationEnum start,
        SceneLocationEnum target)
    {
        List<SceneLocationEnum> path = new List<SceneLocationEnum>();
        SceneLocationEnum current = target;

        while (current != start)
        {
            path.Add(current);
            current = cameFrom[current];
        }

        path.Add(start);
        path.Reverse();

        return path;
    }

    private SceneConnection FindConnection(SceneLocationEnum target)
    {
        foreach (SceneConnection connection in connections)
        {
            if (connection.scene == target)
            {
                return connection;
            }
        }
        return null;
    }
}
