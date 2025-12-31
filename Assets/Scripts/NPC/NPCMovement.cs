using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    private NPC npcData;
    [SerializeField] private GameObject movePointer;

    public Vector2Int gridPosition;
    public float moveSpeed = 3f;

    private Vector3 targetWorldPos;
    private bool isMoving;

    private Vector2Int finalTarget;
    private List<Vector2Int> currentPath;
    private int currentStepIndex;

    void Start()
    {
        npcData = GetComponent<NPC>();
    }

    public void MoveTo(Vector2Int targetGridPos, Vector2Int originalPosition)
    {
        StopAllCoroutines();
        finalTarget = targetGridPos;
        StartMovement(originalPosition);
    }


    private void StartMovement(Vector2Int startPosition)
    {
        currentPath = TileMapController.Instance.FindPath(startPosition, finalTarget);
        currentStepIndex = 0;

        if (currentPath == null || currentPath.Count == 0)
            return;

        StartCoroutine(FollowSteps());
    }

    private void ResetMovePointer()
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

                currentPath = TileMapController.Instance.FindPath(actualPos, finalTarget);
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

            UpdateNPCLocationInGrid(originalPosition, movePointer.transform.position, npcData.npcData.id);

            transform.position = movePointer.transform.position;
            currentStepIndex++;
        }

        isMoving = false;
    }


}
