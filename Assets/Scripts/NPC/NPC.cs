using System.Collections;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public NPCData npcData;
    private SpriteRenderer spriteRenderer;
    private NPCMovement npcMovement;
    private ThoughtBubbleController bubble;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        npcMovement = GetComponent<NPCMovement>();
        bubble = GetComponentInChildren<ThoughtBubbleController>();
    }

    void OnEnable()
    {
        DialogueManager.OnDialogueFinish += EndInteraction;
    }

    public void SetNPC(bool active)
    {
        spriteRenderer.enabled = active;
    }

    public void Interact(Vector2 side)
    {
        npcMovement.SetNPCCanWalk(false);
        NPCSide npcSide;
        
        if(side == Vector2.up)
        {
            npcSide = NPCSide.FRONT;
        } else if (side == Vector2.down)
        {
            npcSide = NPCSide.BACK;
        } else if (side == Vector2.left)
        {
            npcSide = NPCSide.RIGHT;
        } else
        {
            npcSide = NPCSide.LEFT;
        }

        npcMovement.SetIdle(npcSide);
        DialogueManager.Instance.SetDialogue(npcData.id, "1");
    }

    public void AddHeart(int num)
    {
        npcData.hearts += num;
        if(npcData.hearts > 10)
        {
            npcData.hearts = 10;
        }
        if(npcData.hearts < 0)
        {
            npcData.hearts = 0;
        }

        Debug.Log("Corações adicionados " + npcData.hearts);
    }

    public void ShowReaction(ThoughtEmoteEnum reaction)
    {
        bubble.ShowBalloon(reaction);
    }

    private void EndInteraction(int npcId)
    {
        if(npcId == npcData.id)
        {
            StartCoroutine(ResetStates());
        }
    }

    private IEnumerator ResetStates()
    {
        yield return new WaitForSeconds(1f);
        npcMovement.SetNPCCanWalk(true);
    }

    public void StartRoutine(NPCRoutine routine)
    {
        npcMovement.SetupMoveTo(
            routine.targetPosition,
            routine.targetLocation,
            routine.finalSide
        );
    }
}
