using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CutsceneParallelBlockPiece))]
public class CutsceneParallelBlockPieceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = 4f;

        Rect rect = new Rect(position.x, position.y, position.width, lineHeight);

        SerializedProperty actionType = property.FindPropertyRelative("actionType");

        EditorGUI.PropertyField(rect, actionType);
        rect.y += lineHeight + spacing;

        switch ((CutsceneActionType)actionType.enumValueIndex)
        {
            case CutsceneActionType.MoveNPC:
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("npcID"));
                rect.y += lineHeight + spacing;

                EditorGUI.PropertyField(rect, property.FindPropertyRelative("targetScene"));
                rect.y += lineHeight + spacing;

                EditorGUI.PropertyField(rect, property.FindPropertyRelative("targetPosition"));
                rect.y += lineHeight + spacing;

                EditorGUI.PropertyField(rect, property.FindPropertyRelative("targetSide"));
                break;

            case CutsceneActionType.Dialogue:
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("npcID"));
                rect.y += lineHeight + spacing;

                EditorGUI.PropertyField(rect, property.FindPropertyRelative("dialogueKey"));
                break;

            case CutsceneActionType.Wait:
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("waitTime"));
                break;

            case CutsceneActionType.ShowNPCExpression:
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("npcID"));
                rect.y += lineHeight + spacing;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("emote"));
                break;

            case CutsceneActionType.MovePlayer:
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("targetScene"));
                rect.y += lineHeight + spacing;

                EditorGUI.PropertyField(rect, property.FindPropertyRelative("targetPosition"));
                rect.y += lineHeight + spacing;

                EditorGUI.PropertyField(rect, property.FindPropertyRelative("targetSide"));
                break;
            
            case CutsceneActionType.ShowPlayerExpression:
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("emote"));
                break;
            
            case CutsceneActionType.CameraFocus:
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("targetPosition"));
                break;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = 7f;

        int lines = 1; // actionType sempre aparece

        SerializedProperty actionType = property.FindPropertyRelative("actionType");

        switch ((CutsceneActionType)actionType.enumValueIndex)
        {
            case CutsceneActionType.MoveNPC:
                lines += 4;
                break;

            case CutsceneActionType.Dialogue:
                lines += 2;
                break;

            case CutsceneActionType.Wait:
                lines  += 1; 
                break;

            case CutsceneActionType.ShowNPCExpression:
                lines += 2;
                break;
            
            case CutsceneActionType.MovePlayer:
                lines += 3;
                break;
            
            case CutsceneActionType.ShowPlayerExpression:
                lines += 1;
                break;
            
            case CutsceneActionType.ParallelActions:
                lines = 1;
                break;
            
            case CutsceneActionType.CameraFocus:
                lines += 1;
                break;
        }

        return lines * (lineHeight + spacing);
    }
}
