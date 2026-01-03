using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    private NPC npc;
    [SerializeField] private GameObject movePointer;
    [SerializeField] private Animator nPCAnimator;

    public Vector2Int gridPosition;
    public float moveSpeed = 3f;

    // private Vector3 targetWorldPos;
    // private bool isMoving;

    private Vector2Int finalTarget;
    List<SceneLocationEnum> sceneList;
    SceneLocationEnum lastScene;
    SceneLocationEnum finalScene;
    private List<Vector2Int> currentPath;
    private int currentStepIndex;

    void Start()
    {
        npc = GetComponent<NPC>();
    }

    void OnEnable()
    {
        WarpController.OnWarpEnd += SpawnNPCPositionMidTravel;
    }

    void OnDisable()
    {
        WarpController.OnWarpEnd -= SpawnNPCPositionMidTravel;
    }

    public void MoveTo(Vector2Int targetGridPos, Vector2Int originalPosition, SceneLocationEnum targetScene)
    {
        StopAllCoroutines();
        finalTarget = targetGridPos;
        finalScene = targetScene;
        StartMovement(originalPosition);
    }

    public void MoveOffScreen(Vector2Int targetGridPos, Vector2Int originalPosition, SceneLocationEnum targetScene)
    {
        StopAllCoroutines();
        finalTarget = targetGridPos;
        finalScene = targetScene;
        StartMovementOffScreen();
    }


    #region Screen Movement
    private void StartMovement(Vector2Int startPosition)
    {
        sceneList =
            ScenesConections.Instance.GetPathToLocation(
                SceneInfo.Instance.location,
                finalScene
            );
        currentPath = TileMapController.Instance.FindPath(startPosition, finalTarget, finalScene, sceneList);
        currentStepIndex = 0;

        if (currentPath == null || currentPath.Count == 0)
            return;

        StartCoroutine(FollowSteps());
    }

    public void ResetMovePointer()
    {
        if(movePointer.transform.position == transform.position) return;
        
        movePointer.transform.position = transform.position;
    }

    private bool SetMovePointer(Vector2Int movement)
    {
        movePointer.transform.position += GetNextStep(new Vector2Int((int)movePointer.transform.position.x, (int)movePointer.transform.position.y), movement);
        if(PlayerOnWay(movement))
        {
            ResetMovePointer();
            return false;
        }
        return true;
    }

    private bool PlayerOnWay(Vector2Int pos)
    {

        if(new Vector2Int((int)Player_Controller.Instance.transform.position.x, (int)Player_Controller.Instance.transform.position.y) ==  pos)
        {
            return true;
        }
        return false;
    }

    Vector3 GetNextStep(Vector2Int current, Vector2Int target)
    {
        if (current.x < target.x) return Vector3.right;
        if (current.x > target.x) return Vector3.left;
        if (current.y < target.y) return Vector3.up;
        if (current.y > target.y) return Vector3.down;

        return Vector3.zero;
    }

    private void UpdateNPCLocationInGrid(Vector2Int originalPosition, Vector2 newPosition, int id)
    {
        NPCController.Instance.SetDataInNPCMap(originalPosition.x, originalPosition.y, 0);
        NPCController.Instance.SetDataInNPCMap((int)newPosition.x, (int)newPosition.y, id);
        UpdateNPCGridPosition(newPosition);
    }

    private IEnumerator FollowSteps()
    {
        ResetMovePointer();
        npc.npcData.state = NPCStateEnum.Traveling;

        while (currentStepIndex < currentPath.Count)
        {
            Vector2Int nextTile = currentPath[currentStepIndex];

            if (PlayerOnWay(nextTile))
            {
                Vector2Int actualPos = new Vector2Int(
                    (int)transform.position.x,
                    (int)transform.position.y
                );

                currentPath = TileMapController.Instance.FindPath(actualPos, finalTarget, finalScene, sceneList);
                currentStepIndex = 0;

                if (currentPath == null || currentPath.Count == 0)
                {
                    npc.npcData.state = NPCStateEnum.Idle;
                    yield break;
                }

                yield return null;
                continue;
            }

            SetMovePointer(nextTile);
            SetNPCAnimation();

            Vector2Int originalPosition = new Vector2Int(
                (int)transform.position.x,
                (int)transform.position.y
            );

            while (transform.position != movePointer.transform.position)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    movePointer.transform.position,
                    moveSpeed * Time.deltaTime
                );
                yield return null;
            }

            UpdateNPCLocationInGrid(originalPosition, movePointer.transform.position, npc.npcData.id);

            transform.position = movePointer.transform.position;
            currentStepIndex++;
        }

        npc.npcData.state = NPCStateEnum.Idle;
        if(finalScene != SceneInfo.Instance.location)
        {
            RemoveNPCFromGrid(transform.position);
            MoveOffScreen(finalTarget, npc.npcData.gridPosition, finalScene);
        }

        ResetNPCAnimation();
    }
    #endregion

    #region Off Screen Movement
    private void StartMovementOffScreen()
    {
        sceneList =
            ScenesConections.Instance.GetPathToLocation(
                npc.npcData.location,
                finalScene
            );

        StartCoroutine(FollowScenesOffScreen());
    }

    private IEnumerator FollowScenesOffScreen()
    {
        RemoveNPCFromGrid(transform.position);

        for (int i = 0; i < sceneList.Count - 1; i++)
        {
            SceneLocationEnum from = sceneList[i];
            SceneLocationEnum to = sceneList[i + 1];

            float travelTime = GetTravelTime(from, to);

            npc.npcData.state = NPCStateEnum.Traveling;
            npc.npcData.location = from;
            lastScene = from;
            npc.npcData.location = to;
            if(npc.npcData.location == SceneInfo.Instance.location)
            {
                break;
            }

            yield return new WaitForSeconds(travelTime);
            npc.npcData.state = NPCStateEnum.Idle;
        }

        OnOffscreenMovementFinished(lastScene);
    }

    private float GetTravelTime(SceneLocationEnum from, SceneLocationEnum to)
    {
        return 2f;
    }

    private void OnOffscreenMovementFinished(SceneLocationEnum previousScene)
    {
        if(npc.npcData.location == SceneInfo.Instance.location)
        {
            NPCApearingAfterOffsetMove(previousScene);
            MoveTo(finalTarget, npc.npcData.gridPosition, finalScene);
        } 
        else
        {
            UpdateNPCGridPosition(finalTarget);
        }
    }

    private void NPCApearingAfterOffsetMove(SceneLocationEnum previousScene)
    {
        Vector2Int position = TileMapController.Instance.GetWarpLocationInScene(previousScene);

        npc.npcData.gridPosition = position;
        transform.position = new Vector3(npc.npcData.gridPosition.x, npc.npcData.gridPosition.y, 0) + NPCController.Instance.GetNPCOffset();
        movePointer.transform.position = transform.position;
        npc.SetNPC(true);
    }

    public void SpawnNPCPositionMidTravel()
    {
        if(npc.npcData.location != SceneInfo.Instance.location || finalScene == SceneInfo.Instance.location) return;

        StopCoroutine(FollowScenesOffScreen());

        TravelAfterSpawn();
    }

    private void TravelAfterSpawn()
    {
        Vector2Int startPosition = TileMapController.Instance.GetWarpLocationInScene(lastScene);

        UpdateNPCGridPosition(startPosition);
        transform.position = new Vector3(startPosition.x, startPosition.y,  0) + NPCController.Instance.GetNPCOffset();
        ResetMovePointer();

        sceneList =
            ScenesConections.Instance.GetPathToLocation(
                SceneInfo.Instance.location,
                finalScene
            );
        currentPath = TileMapController.Instance.FindPath(startPosition, finalTarget, finalScene, sceneList);
        currentStepIndex = 0;

        if (currentPath == null || currentPath.Count == 0)
            return;

        StartCoroutine(FollowSteps());
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

    #region Remove from scene
    private void RemoveNPCFromGrid(Vector2 position)
    {
        NPCController.Instance.SetDataInNPCMap((int)position.x, (int)position.y, 0);
        transform.position = new Vector2(-10, -10);
        movePointer.transform.position = transform.position;
        npc.SetNPC(false);
    }
    #endregion

    private void UpdateNPCGridPosition(Vector2 position)
    {
        npc.npcData.gridPosition = new Vector2Int((int)position.x, (int)position.y);
    }
}
