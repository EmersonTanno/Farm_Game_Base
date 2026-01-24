using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    #region Instance Variables
    private NPC npc;
    [SerializeField] private GameObject movePointer;
    [SerializeField] private Animator nPCAnimator;
    [SerializeField] private WarpGraph warpGraph;
    #endregion

    #region Variable
    private float moveSpeed = 2.5f;
    private Vector2Int finalTargetPosition;
    private SceneLocationEnum finalTargetScene;
    private List<SceneLocationEnum> sceneList;
    private List<Vector2Int> movementPath;
    private int currentStepIndex;
    private bool canWalk = true;
    private float travelTimeBtweenScenes;
    private float timeTraveled;
    #endregion

    #region Core
    private void Start()
    {
        npc = GetComponent<NPC>();
    }
    #endregion

    #region Setups
    public void SetupMoveTo(Vector2Int targetGridPos, SceneLocationEnum targetScene)
    {
        StopAllCoroutines();
        finalTargetPosition = targetGridPos;
        finalTargetScene = targetScene;
        SetUpPath();
    }

    private void SetUpPath()
    {
        SetSceneList();

        if(npc.npcData.location == SceneInfo.Instance.location)
        {
            SetNewPath();

            if(movementPath == null || movementPath.Count == 0) return;

            StartCoroutine(MoveOnScreen());
        } else
        {
            StartCoroutine(MoveOffScreen());
        }
    }
    #endregion

    #region Move on Screen
    private IEnumerator MoveOnScreen()
    {
        ResetMovePointer();

        SetState(NPCStateEnum.Walking);
        npc.SetNPC(true);

        while (currentStepIndex < movementPath.Count)
        {
            yield return WaitIfPaused();

            Vector2Int nextPosition = movementPath[currentStepIndex];

            SetMovePointer(nextPosition);
            SetNPCAnimation();

            //Movimenta o npc se a posição do pointer for diferente da dele
            if(npc.npcData.location == SceneInfo.Instance.location)
            {
                while (transform.position != movePointer.transform.position)
                {
                    transform.position = Vector3.MoveTowards(
                        transform.position,
                        movePointer.transform.position,
                        moveSpeed * Time.deltaTime
                    );
                    yield return null;
                }
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }

            UpdateNPCGridPosition(movePointer.transform.position);
            CentralizeNPCInMovePointer();
            currentStepIndex++;
        }

        if(finalTargetScene != SceneInfo.Instance.location || npc.npcData.location != finalTargetScene)
        {
            RemoveNPCFromScene(npc.npcData.gridPosition);
            StartCoroutine(MoveOffScreen());
        }
        else
        {
            SetState(NPCStateEnum.Idle);
        }

        ResetNPCAnimation();
    }
    #endregion

    #region Move off Screen
    private IEnumerator MoveOffScreen()
    {
        bool canChangeScene = true;
        
        SceneLocationEnum fromScene = sceneList[0];
        SceneLocationEnum toScene = sceneList[1];
        SceneLocationEnum lastScene = fromScene;
        while(sceneList.Count > 1)
        {
            fromScene = sceneList[0];
            toScene = sceneList[1];
            travelTimeBtweenScenes = GetTravelTimeBtweenScenes(fromScene, toScene, npc.npcData.location);
            timeTraveled = 0;

            if(npc.npcData.state != NPCStateEnum.Walking)
            {
                while(timeTraveled < travelTimeBtweenScenes)
                {
                    yield return new WaitForSeconds(0.1f);
                    timeTraveled += 0.1f;
                    if(fromScene == SceneInfo.Instance.location)
                    {
                        canChangeScene = false;
                        break;
                    }
                }
            } 
            else
            {
                SetState(NPCStateEnum.Traveling);
            }

            if(canChangeScene)
            {
                npc.npcData.location = toScene;
                sceneList.Remove(fromScene);
            }

            if(npc.npcData.location == SceneInfo.Instance.location)
            {
                break;
            }

            if(npc.npcData.location == finalTargetScene)
            {
                Debug.Log($"De: {lastScene}");
                Debug.Log($"Para: {toScene}");
                Debug.Log($"Local Atual: {npc.npcData.location}");
                Debug.Log("ACHOU");
                WarpNode node = warpGraph.nodes.Find(n => n.scene == npc.npcData.location);
                Vector2Int initialWarpPosition = node.warps.Find(n => n.toScene == lastScene).fromGridPosition;
                Debug.Log(initialWarpPosition);
                Debug.Log($"TIME: {GetTravelTimeInScene(initialWarpPosition, finalTargetPosition)}");
                travelTimeBtweenScenes = GetTravelTimeInScene(initialWarpPosition, finalTargetPosition);
                timeTraveled = 0;

                while(timeTraveled < travelTimeBtweenScenes)
                {
                    yield return new WaitForSeconds(0.1f);
                    timeTraveled += 0.1f;
                    if(npc.npcData.location == SceneInfo.Instance.location)
                    {
                        break;
                    }
                }
                Debug.Log("Chegou");
            }

            lastScene = fromScene;
        }

        ProccessOffScreenMovementFinish(SceneInfo.Instance.location == fromScene ? lastScene : fromScene);
    }

    private void LogSceneList()
    {
        for(int i = 0; i < sceneList.Count; i++)
        {
            Debug.Log(sceneList[i]);
        }
    }


    private void ProccessOffScreenMovementFinish(SceneLocationEnum fromScene)
    {
        if(npc.npcData.location == SceneInfo.Instance.location)
        {
            NPCAppearInSceneAfterTravel(fromScene);
        } 
        else
        {
            OnOffScreenMovementFinish();
        }
    }

    private void OnOffScreenMovementFinish()
    {
        SetState(NPCStateEnum.Idle);
        npc.npcData.gridPosition = finalTargetPosition; 
    }

    private void NPCAppearInSceneAfterTravel(SceneLocationEnum fromScene)
    {
        Vector2Int startPosition = TileMapController.Instance.GetWarpLocationInScene(fromScene);

        npc.npcData.gridPosition = startPosition;

        if(timeTraveled < travelTimeBtweenScenes)
        {
            List<Vector2Int> path = TileMapController.Instance.FindPath(npc.npcData.gridPosition, finalTargetPosition, finalTargetScene, npc.npcData.location, sceneList);

            if (path == null || path.Count == 0)
            {
                Debug.LogWarning("Path vazio no mid travel");
                return;
            }

            float progress = timeTraveled / travelTimeBtweenScenes;
            progress = Mathf.Clamp01(progress);

            int stepsDone = Mathf.FloorToInt(progress * path.Count);
            stepsDone = Mathf.Clamp(stepsDone, 0, path.Count - 1);

            npc.npcData.gridPosition = path[stepsDone];
        }

        transform.position = new Vector3(npc.npcData.gridPosition.x, npc.npcData.gridPosition.y, 0) + NPCController.Instance.GetNPCOffset();
        ResetMovePointer();

        SetupMoveTo(finalTargetPosition, finalTargetScene);
    }
    #endregion

    #region Set Path and Scene List
    private void SetNewPath()
    {
        movementPath = TileMapController.Instance.FindPath(npc.npcData.gridPosition, finalTargetPosition, finalTargetScene, npc.npcData.location, sceneList);
        currentStepIndex = 0;
    }

    private void SetSceneList()
    {
        sceneList = warpGraph.GetPath(npc.npcData.location, finalTargetScene);

        if (sceneList == null || sceneList.Count == 0)
        {
            Debug.LogWarning("NPC não encontrou caminho.");
        }
    }
    #endregion

    #region Remove NPC from Scene
    private void RemoveNPCFromScene(Vector2 position)
    {
        NPCController.Instance.SetDataInNPCMap((int)position.x, (int)position.y, 0);
        transform.position = new Vector2(-10, -10);
        movePointer.transform.position = transform.position;
        npc.SetNPC(false);
    }
    #endregion

    #region Update NPC Grid
    private void UpdateNPCGridPosition(Vector2 newPosition)
    {
        NPCController.Instance.SetDataInNPCMap(npc.npcData.gridPosition.x, npc.npcData.gridPosition.y, 0);
        npc.npcData.gridPosition = new Vector2Int((int)newPosition.x, (int)newPosition.y);
        NPCController.Instance.SetDataInNPCMap((int)newPosition.x, (int)newPosition.y, npc.npcData.id);
    }
    #endregion

    #region Move Pointer
    public void ResetMovePointer()
    {
        if(movePointer.transform.position == transform.position) return;
        
        movePointer.transform.position = transform.position;
    }

    private bool SetMovePointer(Vector2Int movement)
    {
        movePointer.transform.position += GetNextStep(new Vector2Int((int)movePointer.transform.position.x, (int)movePointer.transform.position.y), movement);

        return true;
    }

    private void CentralizeNPCInMovePointer()
    {
        transform.position = movePointer.transform.position;
    }
    #endregion

    #region Pause
    private IEnumerator WaitIfPaused()
    {
        while (!canWalk)
        {
            ResetNPCAnimation();
            yield return null;
        }
    }
    #endregion

    #region Checks
    Vector3 GetNextStep(Vector2Int current, Vector2Int target)
    {
        if (current.x < target.x) return Vector3.right;
        if (current.x > target.x) return Vector3.left;
        if (current.y < target.y) return Vector3.up;
        if (current.y > target.y) return Vector3.down;

        return Vector3.zero;
    }
    #endregion

    #region Travel Time
    private float GetTravelTimeBtweenScenes(SceneLocationEnum from, SceneLocationEnum to, SceneLocationEnum actualScene)
    {
        WarpNode node = warpGraph.nodes.Find(n => n.scene == actualScene);
        Vector2Int initialPos;
        Vector2Int targetPos;
        WarpData initialWarpData = node.warps.Find(n => n.toScene == from);
        if(initialWarpData == null)
        {
            initialPos = npc.npcData.gridPosition;
        }
        else
        {
            initialPos = initialWarpData.fromGridPosition;
        }
        WarpData finalWarpData = node.warps.Find(n => n.toScene == to);
        targetPos = finalWarpData.fromGridPosition;

        int distance =
            Mathf.Abs(initialPos.x - targetPos.x) +
            Mathf.Abs(initialPos.y - targetPos.y);
        
        return distance * (1f / moveSpeed);
    }

    private float GetTravelTimeInScene(Vector2Int initialPos, Vector2Int targetPos)
    {
        int distance =  Mathf.Abs(initialPos.x - targetPos.x) + Mathf.Abs(initialPos.y - targetPos.y);
        return distance * (1f / moveSpeed);
    }
    #endregion

    #region Animation
    private void SetNPCAnimation()
    {
        ResetNPCAnimation();
        
        if(movePointer.transform.position.x > transform.position.x)
        {
            nPCAnimator.SetBool("WalkRight", true);
            return;
        }
        if(movePointer.transform.position.x < transform.position.x)
        {
            nPCAnimator.SetBool("WalkLeft", true);
            return;
        }
        if(movePointer.transform.position.y < transform.position.y)
        {
            nPCAnimator.SetBool("WalkFront", true);
            return;
        }
        if(movePointer.transform.position.y > transform.position.y)
        {
            nPCAnimator.SetBool("WalkBack", true);
            return;
        }
    }

    private void ResetNPCAnimation()
    {
        nPCAnimator.SetBool("WalkFront", false);
        nPCAnimator.SetBool("WalkBack", false);
        nPCAnimator.SetBool("WalkLeft", false);
        nPCAnimator.SetBool("WalkRight", false);
    }
    #endregion

    #region Set NPC State
    private void SetState(NPCStateEnum newState)
    {
        npc.npcData.state = newState;
    }
    #endregion

    #region Set NPC CanWalk
    public void SetNPCCanWalk(bool state)
    {
        canWalk = state;
    }
    #endregion
}
