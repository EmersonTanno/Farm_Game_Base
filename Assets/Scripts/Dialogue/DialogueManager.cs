using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [SerializeField] GameObject DialogueGroup;
    [SerializeField] GameObject DialogueLeftSide;
    [SerializeField] TextMeshProUGUI LeftText;
    [SerializeField] GameObject DialogueRightSide;
    [SerializeField] TextMeshProUGUI RightText;


    public bool dialogueActive = false;
    private bool isTyping;
    private bool nextLine;
    private bool completeLine;

    void Awake()
    {
        if(Instance && Instance != this)
        {
            Destroy(this);
        }

        Instance = this;
    }

    void Start()
    {
        SetDialogue(1, 1);
    }

    private void SetDialogueCanvas(bool active)
    {
        DialogueGroup.SetActive(active);
    }

    private void SetDialogueSide(int portrait)
    {
        if(portrait == 0)
        {
            DialogueRightSide.SetActive(false);
            DialogueLeftSide.SetActive(true);
        }
        else
        {
            DialogueLeftSide.SetActive(false);
            DialogueRightSide.SetActive(true);
        }
    }

    private IEnumerator DisplayDialogueLine(string dialogueLine)
    {
        isTyping = true;
        RightText.text = "";

        foreach (char character in dialogueLine)
        {
            RightText.text += character;
            if(!completeLine)
                yield return new WaitForSecondsRealtime(0.03f);
        }

        yield return new WaitForSecondsRealtime(0.1f);
        completeLine = false;
        isTyping = false;
    }


    public void SetDialogue(int npcId, int dialogueId)
    {
        SetDialogueCanvas(true);
        dialogueActive = true;

        List<DialogueLine> dialogue = JsonManager.Instance.GetDialogue(npcId, dialogueId);

        StartCoroutine(StartDialogue(dialogue));
    }

    private IEnumerator StartDialogue(List<DialogueLine> dialogue)
    {
        foreach (DialogueLine dialogueLine in dialogue)
        {
            SetDialogueSide(dialogueLine.portrait);

            string textLine = GetLanguageLine(
                GameConfigurations.Instance.gameLanguage,
                dialogueLine
            );

            yield return StartCoroutine(DisplayDialogueLine(textLine));

            while (!nextLine)
            {
                yield return null;
            }

            nextLine = false;
            yield return new WaitForSecondsRealtime(0.1f);
        }

        while (!nextLine)
        {
            yield return null;
        }

        nextLine = false;
        
        EndDialogue();
    }


    private void EndDialogue()
    {
        dialogueActive = false;
        SetDialogueCanvas(false);
    }

    private string GetLanguageLine(LanguageEnum language, DialogueLine line)
    {
        switch(language)
        {
            case LanguageEnum.Potugues:
                {
                    return line.textLinePt;
                }
            
            case LanguageEnum.Ingles:
                {
                    return line.textLineEn;
                }
            default:
                {
                    return "";
                }
        }
    }

    public void NextLine()
    {
        if (!dialogueActive) return;

        if (isTyping)
        {
            completeLine = true;
            return;
        }

        nextLine = true;
    }
}
