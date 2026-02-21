using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneController : MonoBehaviour
{
    public static CutsceneController Instance;
    private CutsceneData playingCutscene = null;
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
        playingCutscene = cutsceneDB.GetCutscene(cutsceneId);
        if(!playingCutscene)
        {
            Debug.LogError($"Could not get cutscene for id - '{cutsceneId}' ");
            return;
        }
        StartCoroutine(ProcessCutscene(playingCutscene));
    }

    private IEnumerator ProcessCutscene(CutsceneData data)
    {
        while(Time_Controll.Instance.timerPaused)
        {
            yield return null;
        }

        StartCoroutine(PlayCutscene(playingCutscene));
    }

    private IEnumerator PlayCutscene(CutsceneData data)
    {
        Time_Controll.Instance.PauseTimer();
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
        Time_Controll.Instance.UnpauseTimer();
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
                    yield return InitiateCutsceneNPCShowExpression(step.npcID, step.emote);
                    break;
                }
            case CutsceneActionType.ShowPlayerExpression:
                {
                    yield return InitiateCutscenePlayerShowExpression(step.emote);
                    break;
                }
            case CutsceneActionType.MovePlayer:
                {
                    yield return InitiateCutscenePlayerMovement(step.targetPosition, 4, step.targetSide);
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

    private IEnumerator InitiateCutsceneNPCShowExpression(int npcId, ThoughtEmoteEnum emote)
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

    private IEnumerator InitiateCutscenePlayerShowExpression(ThoughtEmoteEnum emote)
    {
        yield return Player_Controller.Instance.ShowReactionInCutscene(emote);
    }

    private IEnumerator InitiateCutscenePlayerMovement(Vector2Int targetPos, float moveSpeed, NPCSide finalSide)
    {
        yield return Player_Controller.Instance.MovePlayerInCutscene(targetPos, moveSpeed, finalSide);
    }


    public bool CheckNPCInCutscene(int npcID)
    {
        if(playingCutscene.npcs.Find(i => i.npcId == npcID) != null)
        {
            return true;
        }

        return false;
    }
}