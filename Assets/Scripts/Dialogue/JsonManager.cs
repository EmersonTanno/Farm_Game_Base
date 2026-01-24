using System.Collections.Generic;
using UnityEngine;

public class JsonManager : MonoBehaviour
{
    public static JsonManager Instance;

    [Header("Dialogue JSON Files")]
    [SerializeField] private List<TextAsset> dialogueJsonFiles;

    private Dictionary<int, Dictionary<string, Dialogue>> dialogueDatabase;

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
        dialogueDatabase = new Dictionary<int, Dictionary<string, Dialogue>>();

        foreach (TextAsset json in dialogueJsonFiles)
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

    public List<DialogueLine> GetDialogue(int npcId, string dialogueId)
    {
        if (!dialogueDatabase.ContainsKey(npcId))
        {
            Debug.LogWarning($"NPC {npcId} não encontrado");
            return null;
        }

        if (!dialogueDatabase[npcId].ContainsKey(dialogueId))
        {
            Debug.LogWarning(
                $"Diálogo {dialogueId} não encontrado para NPC {npcId}"
            );
            return null;
        }

        return dialogueDatabase[npcId][dialogueId].dialogue;
    }
}
