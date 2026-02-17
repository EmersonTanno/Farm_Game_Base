using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class CutsceneController : MonoBehaviour
{
    public static CutsceneController Instance;

    void Awake()
    {
        Instance = this;
    }

    public void StartCutscene(string cutsceneId)
    {
        //buscar a data
        //StartCoroutine(PlayCutscene())
    }

    private IEnumerator PlayCutscene(CutsceneData data)
    {
        GameSession.Instance.SetGameState(GameState.Cutscene);

        foreach(CutsceneStep step in data.steps)
        {
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
                    break;
                }
            case CutsceneActionType.Dialogue:
                {
                    break;
                }
            case CutsceneActionType.Wait:
                {
                    yield return new WaitForSeconds(step.waitTime);
                    break;
                }
            case CutsceneActionType.ShowExpression:
                {
                    break;
                }
            case CutsceneActionType.CameraFocus:
                {
                    break;
                }
        }
    }
}