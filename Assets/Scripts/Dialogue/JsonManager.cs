using System.Collections.Generic;
using UnityEngine;

public class JsonManager : MonoBehaviour
{
    public static JsonManager Instance;

    [Header("Dialogue JSON Files")]
    [SerializeField] DialoguesDataBase dialogueDB;

    private Dictionary<string, Dictionary<string, Dialogue>> dialogueDatabase;

    private void Awake()
    {
        if(Instance && Instance != this)
        {
            Destroy(this);
        }

        Instance = this;
        LoadAllDialogues();
    }

    private void LoadAllDialogues()
    {
        dialogueDatabase = new Dictionary<string, Dictionary<string, Dialogue>>();

        foreach (TextAsset json in dialogueDB.dialogues)
        {
            if (json == null) continue;

            NpcDialogues npcDialogues =
                JsonUtility.FromJson<NpcDialogues>(json.text);

            if (npcDialogues == null)
            {
                Debug.LogWarning($"Erro ao ler JSON: {json.name}");
                continue;
            }

            if (!dialogueDatabase.ContainsKey(npcDialogues.npcId))
            {
                dialogueDatabase[npcDialogues.npcId] =
                    new Dictionary<string, Dialogue>();
            }

            foreach (Dialogue dialogue in npcDialogues.npcDialogueList)
            {
                dialogueDatabase[npcDialogues.npcId][dialogue.dialogueId] =
                    dialogue;
            }
        }
    }

    public List<DialogueLine> GetDialogue(string dialogueKey, string dialogueId)
    {
        if (!dialogueDatabase.ContainsKey(dialogueKey))
        {
            Debug.LogWarning($"DIALOGUE {dialogueKey} não encontrado");
            return null;
        }

        if (!dialogueDatabase[dialogueKey].ContainsKey(dialogueId))
        {
            Debug.LogWarning(
                $"Diálogo {dialogueId} não encontrado para NPC {dialogueKey}"
            );
            return null;
        }

        return dialogueDatabase[dialogueKey][dialogueId].dialogue;
    }
}
