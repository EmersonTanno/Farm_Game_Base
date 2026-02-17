using UnityEngine;

[System.Serializable]
public class CutsceneStep
{
    public CutsceneActionType actionType;

//npc
    public int npcID;
    public SceneLocationEnum targetScene;
    public Vector2Int targetPosition;
    public NPCSide targetSide;


    public string dialogueKey;

    public ThoughtEmoteEnum emote;

    public float waitTime;
}
