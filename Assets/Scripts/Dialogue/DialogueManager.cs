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

        DialogueGroup.SetActive(true);
        if(dialogue[0].portrait != 0)
        {
            DialogueRightSide.SetActive(true);
            RightText.text = dialogue[0].textLinePt;
        }
    }

}
