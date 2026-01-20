using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(WorldTileDataBase))]
public class WorldTileDataBaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);

        if (GUILayout.Button("Generate Missing Tile IDs"))
        {
            GenerateIds();
        }
    }

    private void GenerateIds()
    {
        WorldTileDataBase db = (WorldTileDataBase)target;

        int maxId = 0;

        foreach (var tile in db.tiles)
        {
            if (tile != null && tile.id > maxId)
                maxId = tile.id;
        }

        bool changed = false;

        foreach (var tile in db.tiles)
        {
            if (tile == null) continue;

            if (tile.id == 0)
            {
                maxId++;
                tile.id = maxId;
                changed = true;
            }
        }

        if (changed)
        {
            EditorUtility.SetDirty(db);
            Debug.Log("IDs de tiles gerados com sucesso.");
        }
        else
        {
            Debug.Log("Nenhum tile com ID 0 encontrado.");
        }
    }
}
