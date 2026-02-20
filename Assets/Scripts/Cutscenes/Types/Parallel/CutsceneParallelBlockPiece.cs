using UnityEngine;

[System.Serializable]
public class CutsceneParallelBlockPiece
{
    public CutsceneActionType actionType;
    public int npcID;
    public SceneLocationEnum targetScene;
    public Vector2Int targetPosition;
    public NPCSide targetSide;
    public string dialogueKey;
    public ThoughtEmoteEnum emote;
    public float waitTime;

    public CutsceneStep ToStep()
    {
        return new CutsceneStep
        {
            actionType = actionType,
            npcID = npcID,
            targetScene = targetScene,
            targetPosition = targetPosition,
            targetSide = targetSide,
            dialogueKey = dialogueKey,
            emote = emote,
            waitTime = waitTime
        };
    }
}