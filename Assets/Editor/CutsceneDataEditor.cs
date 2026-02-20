using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CutsceneData))]
public class CutsceneDataEditor : Editor
{
    SerializedProperty idProp;
    SerializedProperty nameProp;
    SerializedProperty npcsProp;
    SerializedProperty stepsProp;

    void OnEnable()
    {
        idProp = serializedObject.FindProperty("id");
        nameProp = serializedObject.FindProperty("cutsceneName");
        npcsProp = serializedObject.FindProperty("npcs");
        stepsProp = serializedObject.FindProperty("steps");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(idProp);
        EditorGUILayout.PropertyField(nameProp);
        EditorGUILayout.PropertyField(npcsProp, true);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Steps", EditorStyles.boldLabel);

        for (int i = 0; i < stepsProp.arraySize; i++)
        {
            SerializedProperty step = stepsProp.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.PropertyField(step.FindPropertyRelative("actionType"));

            SerializedProperty actionType = step.FindPropertyRelative("actionType");

            switch ((CutsceneActionType)actionType.enumValueIndex)
            {
                case CutsceneActionType.MoveNPC:
                    EditorGUILayout.PropertyField(step.FindPropertyRelative("npcID"));
                    EditorGUILayout.PropertyField(step.FindPropertyRelative("targetScene"));
                    EditorGUILayout.PropertyField(step.FindPropertyRelative("targetPosition"));
                    EditorGUILayout.PropertyField(step.FindPropertyRelative("targetSide"));
                    break;

                case CutsceneActionType.Dialogue:
                    EditorGUILayout.PropertyField(step.FindPropertyRelative("npcID"));
                    EditorGUILayout.PropertyField(step.FindPropertyRelative("dialogueKey"));
                    break;

                case CutsceneActionType.ShowNPCExpression:
                    EditorGUILayout.PropertyField(step.FindPropertyRelative("npcID"));
                    EditorGUILayout.PropertyField(step.FindPropertyRelative("emote"));
                    break;

                case CutsceneActionType.Wait:
                    EditorGUILayout.PropertyField(step.FindPropertyRelative("waitTime"));
                    break;
                
                case CutsceneActionType.MovePlayer:
                    EditorGUILayout.PropertyField(step.FindPropertyRelative("targetScene"));
                    EditorGUILayout.PropertyField(step.FindPropertyRelative("targetPosition"));
                    EditorGUILayout.PropertyField(step.FindPropertyRelative("targetSide"));
                    break;

                case CutsceneActionType.ShowPlayerExpression:
                    EditorGUILayout.PropertyField(step.FindPropertyRelative("emote"));
                    break;

                case CutsceneActionType.CameraFocus:
                    EditorGUILayout.PropertyField(step.FindPropertyRelative("targetPosition"));
                    break;

                case CutsceneActionType.ParallelActions:
                    EditorGUILayout.PropertyField(step.FindPropertyRelative("parallelBlock"), true);
                    break;
                
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("↑") && i > 0)
            {
                stepsProp.MoveArrayElement(i, i - 1);
            }

            if (GUILayout.Button("↓") && i < stepsProp.arraySize - 1)
            {
                stepsProp.MoveArrayElement(i, i + 1);
            }
            
            if (GUILayout.Button("Remove Step"))
            {
                stepsProp.DeleteArrayElementAtIndex(i);
            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        if (GUILayout.Button("Add Step"))
        {
            stepsProp.InsertArrayElementAtIndex(stepsProp.arraySize);
        }

        serializedObject.ApplyModifiedProperties();
    }

}
