using UnityEditor;
using UnityEngine;

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
                Undo.RecordObject(tile, "Generate Tile ID");

                maxId++;
                tile.id = maxId;

                EditorUtility.SetDirty(tile);
                changed = true;
            }
        }

        if (changed)
        {
            EditorUtility.SetDirty(db);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("IDs de tiles gerados e salvos com sucesso.");
        }
        else
        {
            Debug.Log("Nenhum tile com ID 0 encontrado.");
        }
    }
}
