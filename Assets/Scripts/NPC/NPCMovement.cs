using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    #region Instance Variables
    private NPC npc;
    [SerializeField] private GameObject movePointer;
    [SerializeField] private Animator nPCAnimator;
    [SerializeField] private WarpGraph warpGraph;
    [SerializeField] private MapGraph mapGraph;
    #endregion

    #region Variable
    private float moveSpeed = 2.5f;
    private Vector2Int finalTargetPosition;
    private SceneLocationEnum finalTargetScene;
    private NPCSide finalSide;
    private List<SceneLocationEnum> sceneList;
    private List<Vector2Int> movementPath;
    private int currentStepIndex;
    private bool canWalk = true;
    #endregion

    #region Core
    private void Start()
    {
        npc = GetComponent<NPC>();
    }
    #endregion

    #region Setups
    public void SetupMoveTo(Vector2Int targetGridPos, SceneLocationEnum targetScene, NPCSide finalSide)
    {
        StopAllCoroutines();
        finalTargetPosition = targetGridPos;
        finalTargetScene = targetScene;
        this.finalSide = finalSide;
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
            StartCoroutine(MoveOffScreen2());
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

            if(npc.npcData.location != SceneInfo.Instance.location)
            {
                break;
            }
        }

        if(finalTargetScene != SceneInfo.Instance.location || npc.npcData.location != finalTargetScene)
        {
            while(!canWalk)
            {
                yield return null;
            }
            NPCController.Instance.SetDataInNPCMap(npc.npcData.gridPosition.x, npc.npcData.gridPosition.y, 0);
            if(npc.npcData.gridPosition == movementPath[movementPath.Count - 1])
            {
                SetNPCNextScene();
            }
            RemoveNPCFromScene(npc.npcData.gridPosition);
            StartCoroutine(MoveOffScreen2());
        }
        else
        {
            SetState(NPCStateEnum.Idle);
        }

        if(npc.npcData.location == finalTargetScene && npc.npcData.gridPosition == finalTargetPosition)
        {
            SetIdle(finalSide);
        }
    }
    #endregion

    #region Move off Screen

    private IEnumerator MoveOffScreen2()
    {  
        while(npc.npcData.location != finalTargetScene || npc.npcData.gridPosition != finalTargetPosition)
        {
            Vector2Int targetPos;
            if(finalTargetScene != npc.npcData.location)
            {
                WarpNode node = warpGraph.nodes.Find(n => n.scene == npc.npcData.location);
                targetPos = node.warps.Find(n => n.toScene == sceneList[1]).fromGridPosition;
            }
            else
            {
                targetPos = finalTargetPosition;
            }

            List<Vector2Int> positions = mapGraph.GetPath(npc.npcData.location, npc.npcData.gridPosition, targetPos);

            if (positions == null || positions.Count == 0)
            {
                Debug.LogWarning($"NPC {npc.name} sem path offscreen.");
                yield break;
            }

            foreach(Vector2Int position in positions)
            {
                yield return new WaitForSeconds(1f / moveSpeed);
                npc.npcData.gridPosition = position;
                if(npc.npcData.location == SceneInfo.Instance.location)
                {
                    break;
                }
            }

            if(npc.npcData.gridPosition == targetPos && npc.npcData.location != finalTargetScene)
            {
                SetNPCNextScene();
            }

            if(npc.npcData.location == SceneInfo.Instance.location)
            {
                break;
            }
        }

        ProccessOffScreenMovementFinish();

    }

    private void LogSceneList()
    {
        for(int i = 0; i < sceneList.Count; i++)
        {
            Debug.Log(sceneList[i]);
        }
    }

    private void ProccessOffScreenMovementFinish()
    {
        if(npc.npcData.location == SceneInfo.Instance.location)
        {
            NPCAppearInSceneAfterTravel();
        } 
        else
        {
            OnOffScreenMovementFinish();
        }
    }

    private void NPCAppearInSceneAfterTravel()
    {
        npc.SetNPC(true);
        
        List<Vector2Int> path = TileMapController.Instance.FindPath(npc.npcData.gridPosition, finalTargetPosition, finalTargetScene, npc.npcData.location, sceneList);

        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("Path vazio no mid travel");
            return;
        }

        transform.position = new Vector3(npc.npcData.gridPosition.x, npc.npcData.gridPosition.y, 0) + NPCController.Instance.GetNPCOffset();
        ResetMovePointer();

        SetupMoveTo(finalTargetPosition, finalTargetScene, finalSide);
    }


    private void OnOffScreenMovementFinish()
    {
        SetState(NPCStateEnum.Idle);
        npc.npcData.gridPosition = finalTargetPosition; 
         SetIdle(finalSide);
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

    #region Set Next Scene
    private void SetNPCNextScene()
    {
        SceneLocationEnum previousScene = sceneList[0];
        sceneList.RemoveAt(0);
        npc.npcData.location = sceneList[0];
        WarpNode node = warpGraph.nodes.Find(n => n.scene == npc.npcData.location);
        npc.npcData.gridPosition = node.warps.Find(n => n.toScene == previousScene).fromGridPosition;
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

    public void SetIdle(NPCSide side)
    {
        ResetNPCAnimation();
        switch(side)
        {
            case NPCSide.FRONT:
                nPCAnimator.SetBool("IdleFront", true);
                break;
            case NPCSide.BACK:
                nPCAnimator.SetBool("IdleBack", true);
                break;
            case NPCSide.LEFT:
                nPCAnimator.SetBool("IdleLeft", true);
                break;
            case NPCSide.RIGHT:
                nPCAnimator.SetBool("IdleRight", true);
                break;
        }
    }

    private void ResetNPCAnimation()
    {
        nPCAnimator.SetBool("WalkFront", false);
        nPCAnimator.SetBool("WalkBack", false);
        nPCAnimator.SetBool("WalkLeft", false);
        nPCAnimator.SetBool("WalkRight", false);
        nPCAnimator.SetBool("IdleFront", false);
        nPCAnimator.SetBool("IdleBack", false);
        nPCAnimator.SetBool("IdleLeft", false);
        nPCAnimator.SetBool("IdleRight", false);
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
