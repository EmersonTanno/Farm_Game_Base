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

    public void SetNPC(bool active)
    {
        spriteRenderer.enabled = active;
    }

    public void Interact()
    {
        npcMovement.SetNPCCanWalk(false);
        bubble.ShowBalloon(ThoughtEmoteEnum.Sweat);
        DialogueManager.Instance.SetDialogue(npcData.id, 1);
    }

    public void AddHeart(float num)
    {
        npcData.hearts += num;
        if(npcData.hearts > 10)
        {
            npcData.hearts = 10;
        }
    }

    public void RemoveHeart(float num)
    {
        npcData.hearts -= num;
        if(npcData.hearts < 0)
        {
            npcData.hearts = 0;
        }
    }
}
