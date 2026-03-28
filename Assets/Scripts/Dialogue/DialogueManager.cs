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
    [SerializeField] PortraitDataBase portraitDB;

    [SerializeField] GameObject dialogueGroup;
    [SerializeField] GameObject dialogueRightSide;
    [SerializeField] TextMeshProUGUI rightText;
    [SerializeField] Image portrait;

    [SerializeField] GameObject buttonGroup;
    [SerializeField] List<GameObject> buttons;


    private DialogueLine actualLine;

    public bool dialogueActive = false;
    private bool isTyping;
    private bool nextLine;
    private bool completeLine;
    private bool jumpLine;
    private bool canAdvance;
    private bool optionSelected;
    private int selectedOption;
    private bool canPassLine = true;

    //controll
    private bool shopOrDebtActive = false;

    //triggers
    public static event Action<string> OnDialogueFinish;
    public static event Action OnDialogueShopRequest;
    public static event Action<DebtTypeEnum, string> OnDialogueDebtRequest;
    public static event Action<string> OnDebtPaymentRequest;
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

    void OnEnable()
    {
        GameLanguageManager.OnLanguageChange += ChangeLanguage;
        Shop_Manager.OnShopClose += DeactivateShop;
        DebtManager.OnDebtWindowClose += DeactivateShop;
    }

    void OnDisable()
    {
        GameLanguageManager.OnLanguageChange -= ChangeLanguage;
        Shop_Manager.OnShopClose -= DeactivateShop;
        DebtManager.OnDebtWindowClose -= DeactivateShop;
    }
    #endregion

    #region Setup
    private void SetDialogueCanvas(bool active)
    {
        dialogueGroup.SetActive(active);
    }

    private IEnumerator DisplayDialogueLine(string dialogueLine)
    {
        isTyping = true;
        rightText.text = "";

        while(shopOrDebtActive)
        {
            yield return null;
        }

        foreach (char character in dialogueLine)
        {
            while(PauseController.Instance.gamePaused)
            {
                yield return null;
            }
            if(jumpLine)
            {
                break;
            }
            rightText.text += character;
            if(!completeLine)
                yield return new WaitForSecondsRealtime(0.03f);

        }

        yield return new WaitForSecondsRealtime(0.1f);
        jumpLine = false;
        completeLine = false;
        isTyping = false;
    }


    public void SetDialogue(string npcId, string dialogueId)
    {
        SetDialogueCanvas(true);
        dialogueActive = true;
        canAdvance = false;

        List<DialogueLine> dialogue =
            JsonManager.Instance.GetDialogue(npcId, dialogueId);

        StartCoroutine(EnableAdvanceNextFrame());
        StartCoroutine(StartDialogue(npcId, dialogue));
    }

    public IEnumerator SetDialogueToCutscene(string npcId, string dialogueId)
    {
        SetDialogueCanvas(true);
        dialogueActive = true;
        canAdvance = false;

        List<DialogueLine> dialogue =
            JsonManager.Instance.GetDialogue(npcId, dialogueId);

        StartCoroutine(EnableAdvanceNextFrame());
        yield return StartDialogue(npcId, dialogue);
    }
    #endregion


    #region Dialogue
    private IEnumerator StartDialogue(string npcId,List<DialogueLine> dialogue)
    {
        if(GameSession.Instance.gameState != GameState.Cutscene && GameSession.Instance.gameState != GameState.PausedCutscene)
        {
            GameSession.Instance.SetGameState(GameState.Dialogue);
        }
        Time_Controll.Instance.PauseTimer();
        completeLine = false;
        optionSelected = false;
        selectedOption = 0;
        foreach (DialogueLine dialogueLine in dialogue)
        {
            dialogueRightSide.SetActive(true);

            if(dialogueLine.reaction != null)
            {
                NPCController.Instance.ShowNPCReaction(npcId, GetReaction(dialogueLine.reaction));
            }
            AddHearts(npcId, dialogueLine.addHearts);
            
            string textLine = GetLanguageLine(
                GameConfigurations.Instance.gameLanguage,
                dialogueLine
            );

            actualLine = dialogueLine;

            if (dialogueLine.request != null)
            {
                CheckRequest(dialogueLine.request, npcId);
            }

            SetPortrait(dialogueLine);
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
            ContinueDialogue(npcId, dialogue[dialogue.Count - 1].options[selectedOption].toDialogueId);
            yield break;
        }

        nextLine = false;

        EndDialogue(npcId);
    }


    private void EndDialogue(string npcId)
    {
        dialogueActive = false;
        SetDialogueCanvas(false);
        OnDialogueFinish?.Invoke(npcId);

        if(GameSession.Instance.gameState != GameState.Cutscene && GameSession.Instance.gameState != GameState.PausedCutscene)
        {
            GameSession.Instance.SetGameState(GameState.Playing);
            Time_Controll.Instance.UnpauseTimer();
        }
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
        if (!dialogueActive || !canAdvance || PauseController.Instance.gamePaused || !canPassLine) return;

        if (isTyping)
        {
            completeLine = true;
            return;
        }

        canPassLine = false;
        nextLine = true;
        StartCoroutine(EnableNextLine());
    }

    private IEnumerator EnableNextLine()
    {
        yield return new WaitForSeconds(.3f);
        canPassLine = true;
    }

    private IEnumerator EnableAdvanceNextFrame()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        canAdvance = true;
    }

    private void SetPortrait(DialogueLine dialogue)
    {
        if (portraitDB == null)
        {
            Debug.LogError("PortraitDataBase não atribuído no DialogueManager!");
            return;
        }

        Sprite sprite = portraitDB.GetPortrait(dialogue.portrait);
        portrait.sprite = sprite;
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

    private void ContinueDialogue(string npcId, string dialogueId)
    {
        StopAllCoroutines();
        canPassLine = true;
        SetDialogue(npcId, dialogueId);
    }
    #endregion

    #region Reactions
    private ThoughtEmoteEnum GetReaction(string reaction)
    {
        if (string.IsNullOrEmpty(reaction))
            return ThoughtEmoteEnum.None;

        if (Enum.TryParse(
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


    #region Hearts
    private void AddHearts(string npcId, int hearts)
    {
        if(hearts == 0) return;
        NPCController.Instance.AddNPCHearts(npcId, hearts);
    }
    #endregion

    #region Request / Shop / Debt
    private void CheckRequest(string request, string npcId)
    {
        switch(request)
        {
            case "default_shop":
                {
                    RequestShop();
                    break;
                }
            case "shark_debt":
                {
                    RequestDebt(DebtTypeEnum.SHARK, npcId);
                    break;
                }
            case "bank_debt":
                {
                    RequestDebt(DebtTypeEnum.BANK);
                    break;
                }
            case "city_debt":
                {
                    RequestDebt(DebtTypeEnum.CITY);
                    break;
                }
            case "pay_debt":
                {
                    RequestPayDebt(npcId);
                    break;
                }
            default:
                {
                    return;
                }
        }
    }

    private void RequestShop()
    {
        OnDialogueShopRequest?.Invoke();
        shopOrDebtActive = true;
        PauseDialogueUI();
    }

    private void RequestDebt(DebtTypeEnum debtType, string npcId = "-1")
    {
        OnDialogueDebtRequest?.Invoke(debtType, npcId);
        shopOrDebtActive = true;
        PauseDialogueUI();
    }

    private void DeactivateShop()
    {
        if(!shopOrDebtActive) return;
        
        shopOrDebtActive = false;
        ResumeDialogueUI();
    }

    private void RequestPayDebt(string npcId)
    {
        DebtController debtController = DebtController.Instance;
        DebtData debt = debtController.GetDebtByNpcId(npcId);

        if(debt == null) return;

        OnDebtPaymentRequest?.Invoke(debt.id);
    }
    #endregion

    #region PauseDialogue
    public void PauseDialogueUI()
    {
        dialogueGroup.SetActive(false);
        canAdvance = false;
    }

    public void ResumeDialogueUI()
    {
        dialogueGroup.SetActive(true);
        canAdvance = true;
    }
    #endregion

    #region change language
    private void ChangeLanguage()
    {
        if(!dialogueActive) return;

        if (isTyping)
        {
            completeLine = true;
        }
        jumpLine = true;
        rightText.text = GetLanguageLine(
            GameConfigurations.Instance.gameLanguage,
            actualLine
        );
    }
    #endregion
}
