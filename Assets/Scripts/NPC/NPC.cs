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
        Debug.Log("Interagiu");
        npcMovement.SetNPCCanWalk(false);
        bubble.ShowBalloon(ThoughtEmoteEnum.Sweat);
    }
}
