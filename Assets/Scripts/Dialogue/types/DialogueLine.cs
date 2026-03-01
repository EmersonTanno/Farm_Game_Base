using System.Collections.Generic;

[System.Serializable]
public class DialogueLine
{
    public string portrait;
    public string textLinePt;
    public string textLineEn;
    public string reaction;
    public int addHearts;
    public string request;
    public List<DialogueOptions> options;
}