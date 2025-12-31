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

    void Start()
    {
        npcData = GetComponent<NPC>();
    }

    public void MoveTo(Vector2Int targetGridPos, Vector2Int originalPosition)
    {
        StopAllCoroutines();
        StartCoroutine(MoveRoutine(targetGridPos, originalPosition));
    }

    private IEnumerator MoveRoutine(Vector2Int target, Vector2Int originalPosition)
    {
        isMoving = true;

        movePointer.transform.position = transform.position;

        Vector2Int actualPosition = originalPosition;

        while (actualPosition != target)
        {
            Vector2Int dir = GetNextStep(actualPosition, target);
            Vector2Int nextPos = gridPosition + dir;

            MovePointer(dir);

            while (transform.position != movePointer.transform.position)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    movePointer.transform.position,
                    moveSpeed * Time.deltaTime
                );
                if(transform.position == movePointer.transform.position)
                {
                    NPCController.Instance.SetDataInNPCMap(actualPosition.x, actualPosition.y, 0);
                    NPCController.Instance.SetDataInNPCMap((int)movePointer.transform.position.x, (int)movePointer.transform.position.y, npcData.npcData.id);
                    actualPosition = new Vector2Int((int)movePointer.transform.position.x, (int)movePointer.transform.position.y);
                    Debug.Log($"{(int)movePointer.transform.position.x}, {(int)movePointer.transform.position.y}");
                }

                yield return null;
            }

            transform.position = movePointer.transform.position;
            gridPosition = nextPos;

            yield return null;
        }

        isMoving = false;
    }


    Vector2Int GetNextStep(Vector2Int current, Vector2Int target)
    {
        if (current.x < target.x) return Vector2Int.right;
        if (current.x > target.x) return Vector2Int.left;
        if (current.y < target.y) return Vector2Int.up;
        if (current.y > target.y) return Vector2Int.down;

        return Vector2Int.zero;
    }

    private void MovePointer(Vector2Int movement)
    {
        if(transform.position != movePointer.transform.position) return;
        movePointer.transform.position += new Vector3(movement.x, movement.y, 0);
    }

}
