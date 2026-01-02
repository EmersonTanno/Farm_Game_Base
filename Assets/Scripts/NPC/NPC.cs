using UnityEngine;

public class NPC : MonoBehaviour
{
    public NPCData npcData;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetNPC(bool active)
    {
        spriteRenderer.enabled = active;
    }
}
