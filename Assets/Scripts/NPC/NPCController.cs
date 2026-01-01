using System;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public static NPCController Instance;
    public List<NPC> npcs;

    private Vector3 npcOffSet = new Vector3(0.5f, 0.7f, 0);

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        Time_Controll.OnMinuteChange += ChangeNPCLocations;
    }

    void OnDisable()
    {
        Time_Controll.OnMinuteChange -= ChangeNPCLocations;
    }

    public void SetNPCsInScene()
    {
        TileMapController map = TileMapController.Instance;
        foreach(NPC npc in npcs)
        {
            if(npc.npcData.location == SceneInfo.Instance.location)
            {
                npc.SetNPC(true);
                map.SetNPC(npc.npcData.gridPosition.x, npc.npcData.gridPosition.y, npc.npcData.id);

                npc.transform.position = new Vector3(npc.npcData.gridPosition.x, npc.npcData.gridPosition.y, 0) + npcOffSet;
            }
            else
            {
                npc.SetNPC(false);
            }
        } 
    }

    private void ChangeNPCLocations()
    {
        TileMapController map = TileMapController.Instance;
        Grid<WarpTile> warps = map.GetGrid().GetWarpGrid();
        foreach(NPC npc in npcs)
        {
            foreach(NPCRoutine routine in npc.npcData.routine)
            {
                if(routine.startHour == Time_Controll.Instance.hours && routine.startMinute == Time_Controll.Instance.minutes)
                {
                    NPCMovement nPCMovement = npc.GetComponent<NPCMovement>();;
                    Debug.Log("Compromisso");
                    nPCMovement.MoveTo(routine.position, new Vector2Int((int)npc.transform.position.x, (int)npc.transform.position.y), routine.location);
                }
            }
        } 
    }

    public void SetDataInNPCMap(int x, int y, int data)
    {
        TileMapController map = TileMapController.Instance;
        map.SetNPC(x, y, data);
    }


}
