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
        SetDialogueTest();
    }

    private void SetDialogueTest()
    {
        List<DialogueLine> dialogue = JsonManager.Instance.GetDialogue(1, 1);

        SetDialogueCanvas(true);
        SetDialogueSide(dialogue[0].portrait);
        StartCoroutine(DisplayDialogueLine(dialogue[0].textLinePt));
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
        string line = "";
        foreach(char character in dialogueLine)
        {
            line += character;
            RightText.text = line;
            yield return new WaitForSecondsRealtime(0.03f);
        }
    }

}
