using System;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public static NPCController Instance;
    public List<NPC> npcs;

    private Vector3 npcOffSet = new Vector3(0.5f, 0.7f, 0);

    public static event Action OnNPCSet;

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        Time_Controll.OnMinuteChange += NPCMovement;
    }

    void OnDisable()
    {
        Time_Controll.OnMinuteChange -= NPCMovement;
    }

    public void SetNPCsInScene()
    {
        TileMapController map = TileMapController.Instance;
        foreach(NPC npc in npcs)
        {
            if(npc.npcData.location == SceneInfo.Instance.location)
            {
                if(npc.npcData.state == NPCStateEnum.Traveling)
                {
                    continue;
                }

                npc.SetNPC(true);
                map.SetNPC(npc.npcData.gridPosition.x, npc.npcData.gridPosition.y, npc.npcData.id);

                npc.transform.position = new Vector3(npc.npcData.gridPosition.x, npc.npcData.gridPosition.y, 0) + npcOffSet;
            }
            else
            {
                npc.SetNPC(false);
            }
        } 
        OnNPCSet?.Invoke();
    }

    private void NPCMovement()
    {
        foreach(NPC npc in npcs)
        {
            foreach(NPCRoutine routine in npc.npcData.routine)
            {
                if(routine.startHour == Time_Controll.Instance.hours && routine.startMinute == Time_Controll.Instance.minutes)
                {
                    if(npc.npcData.state == NPCStateEnum.Traveling) continue;
                    
                    NPCMovement nPCMovement = npc.GetComponent<NPCMovement>();
                    nPCMovement.SetupMoveTo(routine.targetPosition, routine.targetLocation, routine.finalSide);
                    
                }
            }
        } 
    }

    public void SetDataInNPCMap(int x, int y, int data)
    {
        TileMapController map = TileMapController.Instance;
        map.SetNPC(x, y, data);
    }

    public Vector3 GetNPCOffset()
    {
        return npcOffSet;
    }

    public NPC GetNPC(int id)
    {
        return npcs.Find(p => p.npcData.id == id);
    }

    public void InteractWithNPC(int id)
    {
        NPC npc = GetNPC(id);

        if(npc == null) return;
        
        npc.Interact();
    }

    public void ShowNPCReaction(int id, ThoughtEmoteEnum reaction)
    {
        NPC npc = GetNPC(id);

        if(npc == null) return;

        npc.ShowReaction(reaction);
    }

    public void AddNPCHearts(int id, int hearts)
    {
        NPC npc = GetNPC(id);

        if(npc == null) return;

        npc.AddHeart(hearts);
    }
}
