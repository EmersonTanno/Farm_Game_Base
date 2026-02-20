using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneController : MonoBehaviour
{
    public static CutsceneController Instance;
    [SerializeField] CutsceneDataBase cutsceneDB;
    [SerializeField] CutsceneData cutscene;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCutscene(1);
    }

    public void StartCutscene(int cutsceneId)
    {
        CutsceneData selectedCutscene = cutsceneDB.GetCutscene(cutsceneId);
        if(!selectedCutscene)
        {
            Debug.LogError($"Could not get cutscene for id - '{cutsceneId}' ");
            return;
        }
        StartCoroutine(PlayCutscene(selectedCutscene));
    }

    private IEnumerator PlayCutscene(CutsceneData data)
    {
        GameSession.Instance.SetGameState(GameState.Cutscene);

        SetNPCs(data.npcs);

        foreach(CutsceneStep step in data.steps)
        {
            while(GameSession.Instance.gameState == GameState.Paused || GameSession.Instance.gameState == GameState.PausedCutscene)
            {
                yield return null;
            }
            yield return ExecuteStep(step);
        }

        GameSession.Instance.SetGameState(GameState.Playing);
    }

    private void SetNPCs(List<CutsceneNPCData> npcs)
    {
        foreach(CutsceneNPCData npc in npcs)
        {
            NPCController.Instance.SetNPCInCutscene(npc.npcId, npc.initialPos, SceneInfo.Instance.location);
        }
    }

    private IEnumerator ExecuteStep(CutsceneStep step)
    {
        switch(step.actionType)
        {
            case CutsceneActionType.MoveNPC:
                {
                    yield return InitiateCutsceneMovement(step.npcID, step.targetPosition, step.targetScene, step.targetSide);
                    break;
                }
            case CutsceneActionType.Dialogue:
                {
                    yield return InitiateCutsceneDialogue(step.npcID, step.dialogueKey);
                    break;
                }
            case CutsceneActionType.Wait:
                {
                    yield return new WaitForSeconds(step.waitTime);
                    break;
                }
            case CutsceneActionType.ShowNPCExpression:
                {
                    yield return InitiateCutsceneShowExpression(step.npcID, step.emote);
                    break;
                }
            case CutsceneActionType.CameraFocus:
                {
                    Debug.Log("Camera Focus???");
                    break;
                }
            case CutsceneActionType.ParallelActions:
                {
                    yield return ExecuteParallel(step.parallelBlock);
                    break;
                }
        }
    }

    private IEnumerator InitiateCutsceneMovement(int npcID, Vector2Int targetPos, SceneLocationEnum targetScene, NPCSide side)
    {
        yield return NPCController.Instance.SetNpcMovementInCutscene(npcID, targetPos, targetScene, side);
    }

    private IEnumerator InitiateCutsceneDialogue(int npcId, string dialogueId)
    {
        yield return DialogueManager.Instance.SetDialogueToCutscene(npcId, dialogueId);
    }

    private IEnumerator InitiateCutsceneShowExpression(int npcId, ThoughtEmoteEnum emote)
    {
        yield return NPCController.Instance.ShowNPCReactionInCutscene(npcId, emote);
    }

    private IEnumerator ExecuteParallel(CutsceneParallelBlock block)
    {
        List<Coroutine> coroutines = new List<Coroutine>();

        foreach (CutsceneParallelBlockPiece piece in block.steps)
        {
            coroutines.Add(StartCoroutine(ExecuteStep(piece.ToStep())));
        }

        foreach (var c in coroutines)
            yield return c;
    }

}