using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObjectDatabase : MonoBehaviour
{
    public List<ObjectPrefabPair> prefabs;

    private Dictionary<WorldObjectID, GameObject> prefabDict;

    void Awake()
    {
        prefabDict = new Dictionary<WorldObjectID, GameObject>();

        foreach (var pair in prefabs)
            prefabDict[pair.id] = pair.prefab;
    }

    public GameObject GetPrefab(WorldObjectID id)
    {
        return prefabDict[id];
    }
}
