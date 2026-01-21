using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TileSpriteAssignerWindow : EditorWindow
{
    private List<WorldTile> tiles = new();
    private List<Sprite> sprites = new();

    private Rect tileDropArea;
    private Rect spriteDropArea;

    [MenuItem("Tools/World/Tile Sprite Assigner")]
    public static void Open()
    {
        GetWindow<TileSpriteAssignerWindow>("Tile Sprite Assigner");
    }

    private void OnGUI()
    {
        GUILayout.Label("Tile Sprite Assigner", EditorStyles.boldLabel);
        GUILayout.Space(10);

        DrawDropAreas();
        GUILayout.Space(15);

        DrawPreviewLists();
        GUILayout.Space(15);

        DrawButtons();
    }

    #region UI

    private void DrawDropAreas()
    {
        GUILayout.Label("Drop WorldTiles Here");
        tileDropArea = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));
        DrawDropBox(tileDropArea, "WorldTiles");

        GUILayout.Space(10);

        GUILayout.Label("Drop Sprites Here");
        spriteDropArea = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));
        DrawDropBox(spriteDropArea, "Sprites");

        HandleDragAndDrop();
    }

    private void DrawDropBox(Rect rect, string label)
    {
        GUI.Box(rect, label, EditorStyles.helpBox);
    }

    private void DrawPreviewLists()
    {
        GUILayout.Label($"Tiles ({tiles.Count})", EditorStyles.boldLabel);
        foreach (var tile in tiles)
        {
            EditorGUILayout.ObjectField(tile, typeof(WorldTile), false);
        }

        GUILayout.Space(10);

        GUILayout.Label($"Sprites ({sprites.Count})", EditorStyles.boldLabel);
        foreach (var sprite in sprites)
        {
            EditorGUILayout.ObjectField(sprite, typeof(Sprite), false);
        }
    }

    private void DrawButtons()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Assign Sprites To Tiles"))
        {
            AssignSprites();
        }

        GUI.enabled = tiles.Count > 0 || sprites.Count > 0;
        if (GUILayout.Button("Clear Lists"))
        {
            ClearLists();
        }
        GUI.enabled = true;

        GUILayout.EndHorizontal();
    }

    #endregion

    #region Drag & Drop

    private void HandleDragAndDrop()
    {
        Event evt = Event.current;

        if (evt.type != EventType.DragUpdated &&
            evt.type != EventType.DragPerform)
            return;

        if (!tileDropArea.Contains(evt.mousePosition) &&
            !spriteDropArea.Contains(evt.mousePosition))
            return;

        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

        if (evt.type == EventType.DragPerform)
        {
            DragAndDrop.AcceptDrag();

            if (tileDropArea.Contains(evt.mousePosition))
            {
                tiles = DragAndDrop.objectReferences
                    .OfType<WorldTile>()
                    .Distinct()
                    .ToList();
            }
            else if (spriteDropArea.Contains(evt.mousePosition))
            {
                sprites = DragAndDrop.objectReferences
                    .OfType<Sprite>()
                    .Distinct()
                    .ToList();
            }
        }

        evt.Use();
    }

    #endregion

    #region Logic

    private void AssignSprites()
    {
        if (tiles.Count == 0 || sprites.Count == 0)
        {
            Debug.LogError("As listas estão vazias.");
            return;
        }

        if (tiles.Count != sprites.Count)
        {
            Debug.LogError(
                $"Quantidade diferente: Tiles ({tiles.Count}) | Sprites ({sprites.Count})"
            );
            return;
        }

        bool changed = false;

        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i] == null || sprites[i] == null)
                continue;

            Undo.RecordObject(tiles[i], "Assign Tile Sprite");
            tiles[i].sprite = sprites[i];
            EditorUtility.SetDirty(tiles[i]);

            changed = true;
        }

        if (changed)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        Debug.Log($"Sprites atribuídos com sucesso ({tiles.Count}).");
    }


    private void ClearLists()
    {
        tiles.Clear();
        sprites.Clear();
        Repaint();

        Debug.Log("Listas limpas.");
    }

    #endregion
}
