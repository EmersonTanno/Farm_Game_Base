using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public static NPCController Instance;
    public List<NPC> npcs;

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        TileMapController.OnTileMapReady += SetStartNPCs;
    }

    void OnDisable()
    {
        TileMapController.OnTileMapReady -= SetStartNPCs;
    }


    private void SetStartNPCs()
    {
        TileMapController map = TileMapController.Instance;
        foreach(NPC npc in npcs)
        {
            if(npc.npcData.location == SceneInfo.Instance.location)
            {
                npc.SetNPC(true);
                map.SetNPC((int)npc.transform.position.x, (int)npc.transform.position.y, npc.npcData.id);
            }
            else
            {
                npc.SetNPC(false);
            }
        } 
    }



}
