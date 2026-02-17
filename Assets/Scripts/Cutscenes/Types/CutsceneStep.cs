using System.Numerics;

[System.Serializable]
public class CutsceneStep
{
    public CutsceneActionType actionType;

    public string npcID;
    public Vector2 targetPosition;

    public string dialogueKey;
    public float waitTime;
}
