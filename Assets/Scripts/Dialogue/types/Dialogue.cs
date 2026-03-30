using System.Collections.Generic;

[System.Serializable]
public class Dialogue
{
    public string dialogueId;
    public List<DialogueLine> dialogueLines;

    public int minHearts;
    public int maxHearts;

    public int startHour;
}