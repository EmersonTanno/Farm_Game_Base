using System.Collections;
using UnityEngine;

public class CutsceneController : MonoBehaviour
{
    public static CutsceneController Instance;
    [SerializeField] CutsceneData cutscene;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(WaitAndStart());
    }

    private IEnumerator WaitAndStart()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(PlayCutscene(cutscene));
    }

    public void StartCutscene(string cutsceneId)
    {
        // buscar a data
        // StartCoroutine(PlayCutscene())
    }

    private IEnumerator PlayCutscene(CutsceneData data)
    {
        GameSession.Instance.SetGameState(GameState.Cutscene);

        NPCController.Instance.SetNPCInCutscene(data.npcId, data.initialNPCPos, SceneInfo.Instance.location);

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
            case CutsceneActionType.ShowExpression:
                {
                    yield return InitiateCutsceneShowExpression(step.npcID, step.emote);
                    break;
                }
            case CutsceneActionType.CameraFocus:
                {
                    Debug.Log("Camera Focus???");
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
}