using System.Collections.Generic;

[System.Serializable]
public class DialogueLine
{
    public int portrait;
    public string textLinePt;
    public string textLineEn;
    public List<DialogueOptions> options;
}