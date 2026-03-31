using UnityEngine;

[System.Serializable]
public class CutsceneParallelBlockPiece
{
    public CutsceneActionType actionType;
    public string npcID;
    public SceneLocationEnum targetScene;
    public Vector2Int targetPosition;
    public float spd = 2.5f;
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
            spd = spd,
            targetSide = targetSide,
            dialogueKey = dialogueKey,
            emote = emote,
            waitTime = waitTime
        };
    }
}