using UnityEngine;

[System.Serializable]
public class CutsceneStep
{
    public CutsceneActionType actionType;

    public int npcID;
    public SceneLocationEnum targetScene;
    public Vector2Int targetPosition;
    public float spd = 2.5f;
    public NPCSide targetSide;

    public string dialogueKey;

    public ThoughtEmoteEnum emote;

    public float waitTime;

    public CutsceneParallelBlock parallelBlock;
}