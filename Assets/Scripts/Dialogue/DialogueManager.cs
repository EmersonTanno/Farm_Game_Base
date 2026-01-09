using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

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
        List<DialogueLine> dialogue = JsonManager.Instance.GetDialogue(2, 1);
        Debug.Log(dialogue[0].textLinePt);
    }

}
