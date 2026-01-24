using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "World/Map Graph")]
public class MapGraph : ScriptableObject
{
    public List<MapData> maps = new();

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

        if (map == null)
        {
            Debug.LogError($"Mapa da cena {scene} não encontrado.");
            return -1;
        }

        if (x < 0 || y < 0 || x >= map.width || y >= map.height)
        {
            Debug.LogError($"Posição inválida ({x},{y}) no mapa {scene}.");
            return -1;
        }

        return map.Get(x, y);
    }

}
