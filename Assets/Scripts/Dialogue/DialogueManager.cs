using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    #region Variables
    [SerializeField] GameObject dialogueGroup;
    [SerializeField] GameObject dialogueLeftSide;
    [SerializeField] TextMeshProUGUI leftText;
    [SerializeField] GameObject dialogueRightSide;
    [SerializeField] TextMeshProUGUI rightText;

    [SerializeField] GameObject buttonGroup;
    [SerializeField] List<GameObject> buttons;


    public bool dialogueActive = false;
    private bool isTyping;
    private bool nextLine;
    private bool completeLine;
    private bool canAdvance;
    private bool optionSelected;
    private int selectedOption;
    #endregion

    #region Core
    void Awake()
    {
        if(Instance && Instance != this)
        {
            Destroy(this);
        }

        Instance = this;
    }
    #endregion

    #region Setup
    private void SetDialogueCanvas(bool active)
    {
        dialogueGroup.SetActive(active);
    }

    private void SetDialogueSide(int portrait)
    {
        if(portrait == 0)
        {
            dialogueRightSide.SetActive(false);
            dialogueLeftSide.SetActive(true);
        }
        else
        {
            dialogueLeftSide.SetActive(false);
            dialogueRightSide.SetActive(true);
        }
    }

    private IEnumerator DisplayDialogueLine(string dialogueLine)
    {
        isTyping = true;
        rightText.text = "";

        foreach (char character in dialogueLine)
        {
            rightText.text += character;
            if(!completeLine)
                yield return new WaitForSecondsRealtime(0.03f);
        }

        yield return new WaitForSecondsRealtime(0.1f);
        completeLine = false;
        isTyping = false;
    }


    public void SetDialogue(int npcId, string dialogueId)
    {
        SetDialogueCanvas(true);
        dialogueActive = true;
        canAdvance = false;

        List<DialogueLine> dialogue =
            JsonManager.Instance.GetDialogue(npcId, dialogueId);

        StartCoroutine(EnableAdvanceNextFrame());
        StartCoroutine(StartDialogue(npcId, dialogue));
    }
    #endregion


    #region Dialogue
    private IEnumerator StartDialogue(int npcId,List<DialogueLine> dialogue)
    {
        completeLine = false;
        optionSelected = false;
        selectedOption = 0;
        foreach (DialogueLine dialogueLine in dialogue)
        {
            SetDialogueSide(dialogueLine.portrait);


            if(dialogueLine.reaction != null)
            {
                NPCController.Instance.ShowNPCReaction(npcId, GetReaction(dialogueLine.reaction));
            }
            
            string textLine = GetLanguageLine(
                GameConfigurations.Instance.gameLanguage,
                dialogueLine
            );

            yield return StartCoroutine(DisplayDialogueLine(textLine));

            if (dialogueLine.options != null && dialogueLine.options.Count > 0 && !isTyping)
            {
                SetButtons(dialogueLine.options);
            }
            else
            {
                DeactivateButtons();
            }

            while (!nextLine || (!optionSelected && dialogueLine.options != null && dialogueLine.options.Count > 0))
            {
                yield return null;
            }

            nextLine = false;
            yield return new WaitForSecondsRealtime(0.1f);
        }

        if(optionSelected)
        {
            ContinueDialogue(1, dialogue[dialogue.Count - 1].options[selectedOption].toDialogueId);
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
        if (!dialogueActive || !canAdvance) return;

        if (isTyping)
        {
            completeLine = true;
            return;
        }

        nextLine = true;
    }

    private IEnumerator EnableAdvanceNextFrame()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        canAdvance = true;
    }
    #endregion

    #region Tree Dialog
    private void SetButtons(List<DialogueOptions> options)
    {
        DeactivateButtons();

        buttonGroup.SetActive(true);

        for(int i = 0; i < options.Count; i++)
        {
            buttons[i].SetActive(true);
            buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = GetLanguageOption(GameConfigurations.Instance.gameLanguage, options[i]);
        }
        
        optionSelected = false;
    }

    private void DeactivateButtons()
    {
        foreach(GameObject gameObject in buttons)
        {
            gameObject.SetActive(false);
        }

        buttonGroup.SetActive(false);
    }

    private string GetLanguageOption(LanguageEnum language, DialogueOptions option)
    {
        switch(language)
        {
            case LanguageEnum.Potugues:
                {
                    return option.answerOptionPt;
                }
            
            case LanguageEnum.Ingles:
                {
                    return option.answerOptionEn;
                }
            default:
                {
                    return "";
                }
        }
    }

    public void OnSelectButton(int choice)
    {
        selectedOption = choice;
        optionSelected = true;
        DeactivateButtons();
    }

    private void ContinueDialogue(int npcId, string dialogueId)
    {
        StopAllCoroutines();
        SetDialogue(npcId, dialogueId);
    }
    #endregion

    #region Reactions
    private ThoughtEmoteEnum GetReaction(string reaction)
    {
        if (string.IsNullOrEmpty(reaction))
            return ThoughtEmoteEnum.None;

        if (System.Enum.TryParse(
            reaction,
            true,
            out ThoughtEmoteEnum result
        ))
        {
            return result;
        }

        Debug.LogWarning($"Reaction inválida no JSON: {reaction}");
        return ThoughtEmoteEnum.None;
    }
    #endregion

}
