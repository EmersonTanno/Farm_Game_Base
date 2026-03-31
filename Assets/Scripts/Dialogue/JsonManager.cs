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

        return dialogueDatabase[dialogueKey][dialogueId].dialogueLines;
    }

    public string GetBestDialogueId(string npcId, int hearts, int currentHour, bool hasAlreadyInteractToday)
    {
        if (!dialogueDatabase.ContainsKey(npcId))
            return null;

        var dialogues = dialogueDatabase[npcId].Values;

        Dialogue best = null;

        foreach (var dialogue in dialogues)
        {
            // Hearts
            if (hearts < dialogue.minHearts || hearts > dialogue.maxHearts)
                continue;

            // Hora
            if (dialogue.startHour != -1 && dialogue.startHour != currentHour)
                continue;

            // Interação
            if (dialogue.hasAlreadyInteracted && !hasAlreadyInteractToday)
                continue;

            if (best == null)
            {
                best = dialogue;
                continue;
            }

            bool currentHasHour = dialogue.startHour != -1;
            bool bestHasHour = best.startHour != -1;

            bool currentIsRepeat = dialogue.hasAlreadyInteracted;
            bool bestIsRepeat = best.hasAlreadyInteracted;

            // Prioridade 1: diálogo de repetição (mais específico)
            if (currentIsRepeat && !bestIsRepeat)
            {
                best = dialogue;
                continue;
            }

            // Prioridade 2: horário específico
            if (currentIsRepeat == bestIsRepeat)
            {
                if (currentHasHour && !bestHasHour)
                {
                    best = dialogue;
                    continue;
                }

                // Prioridade 3: maior minHearts
                if (currentHasHour == bestHasHour &&
                    dialogue.minHearts > best.minHearts)
                {
                    best = dialogue;
                }
            }
        }

        return best?.dialogueId;
    }
}
