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

    private Vector3 targetWorldPos;
    private bool isMoving;

    private Vector2Int finalTarget;
    List<SceneLocationEnum> sceneList;
    SceneLocationEnum finalScene;
    private List<Vector2Int> currentPath;
    private int currentStepIndex;

    void Start()
    {
        npc = GetComponent<NPC>();
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
    }

    private IEnumerator FollowSteps()
    {
        ResetMovePointer();
        isMoving = true;

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
                    isMoving = false;
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

        isMoving = false;
        if(finalScene != SceneInfo.Instance.location)
        {
            RemoveNPCFromGrid(transform.position);
            //Rever esse movimento
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
        SceneLocationEnum previousScene = SceneLocationEnum.FARM;

        for (int i = 0; i < sceneList.Count - 1; i++)
        {
            SceneLocationEnum from = sceneList[i];
            SceneLocationEnum to = sceneList[i + 1];
            previousScene = from;

            float travelTime = GetTravelTime(from, to);

            npc.npcData.state = NPCStateEnum.Traveling;
            npc.npcData.location = from;
            Debug.Log($"Começando Viagem: {npc.npcData.location}");
            yield return new WaitForSeconds(travelTime);
            
            npc.npcData.location = to;
            Debug.Log($"Finalizando viagem: {npc.npcData.location}");

            if(npc.npcData.location == SceneInfo.Instance.location)
            {
                break;
            }
        }
        
        npc.npcData.state = NPCStateEnum.Idle;

        OnOffscreenMovementFinished(previousScene);
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
    }

    private void NPCApearingAfterOffsetMove(SceneLocationEnum previousScene)
    {
        Vector2Int position = TileMapController.Instance.GetWarpLocationInScene(previousScene);

        npc.npcData.gridPosition = position;
        transform.position = new Vector3(npc.npcData.gridPosition.x, npc.npcData.gridPosition.y, 0) + NPCController.Instance.GetNPCOffset();
        movePointer.transform.position = transform.position;
        npc.SetNPC(true);
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
}
