using UnityEditor;
using UnityEngine;

public class WarpGraphBuilder : EditorWindow
{
    private WarpGraph warpGraph;
    private SceneLocationEnum currentScene;

    [MenuItem("Tools/World/Warp Graph Builder")]
    public static void Open()
    {
        GetWindow<WarpGraphBuilder>("Warp Graph Builder");
    }

    private void OnGUI()
    {
        GUILayout.Label("Warp Graph Builder", EditorStyles.boldLabel);

        warpGraph = (WarpGraph)EditorGUILayout.ObjectField(
            "Warp Graph",
            warpGraph,
            typeof(WarpGraph),
            false
        );

        currentScene = (SceneLocationEnum)EditorGUILayout.EnumPopup(
            "Current Scene",
            currentScene
        );

        GUILayout.Space(10);

        if (GUILayout.Button("Scan Scene Warps"))
        {
            if (warpGraph == null)
            {
                Debug.LogError("WarpGraph não atribuído.");
                return;
            }

            ScanScene();
        }
    }

    private void ScanScene()
    {
        Warp[] sceneWarps = FindObjectsOfType<Warp>();

        Undo.RecordObject(warpGraph, "Update WarpGraph");

        WarpNode node = warpGraph.GetOrCreateNode(currentScene);

        node.warps.Clear();

        foreach (Warp warp in sceneWarps)
        {
            Vector3 pos = warp.transform.position;

            WarpData data = new WarpData
            {
                scene = currentScene,
                fromGridPosition = new Vector2Int(
                    Mathf.RoundToInt(pos.x),
                    Mathf.RoundToInt(pos.y)
                ),
                toScene = warp.toScene,
                toGridPosition = new Vector2Int(
                    warp.toX,
                    warp.toY
                ),
                transitionType = warp.transitionType
            };

            node.warps.Add(data);
        }

        EditorUtility.SetDirty(warpGraph);

        Debug.Log(
            $"Cena {currentScene} salva com {node.warps.Count} warps."
        );
    }
}
