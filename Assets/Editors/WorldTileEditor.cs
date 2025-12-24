using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WorldTile))]
public class WorldTileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawPropertiesExcluding(
            serializedObject,
            "m_Script",
            "id",
            "isWarp",
            "warp"
        );

        SerializedProperty id = serializedObject.FindProperty("id");
        SerializedProperty isWarp = serializedObject.FindProperty("isWarp");
        SerializedProperty warp = serializedObject.FindProperty("warp");

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(id);
        EditorGUILayout.PropertyField(isWarp);

        if (isWarp.boolValue)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Warp Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(warp, true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
