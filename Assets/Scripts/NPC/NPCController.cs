using System;
using System.Collections;
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
        Time_Controll.OnMinuteChange += NPCActivateRoutine;
        CutsceneController.OnCutsceneEnd += CharaterInCutscene;
    }

    void OnDisable()
    {
        Time_Controll.OnMinuteChange -= NPCActivateRoutine;
        CutsceneController.OnCutsceneEnd -= CharaterInCutscene;
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

    #region Routine
    private void NPCActivateRoutine()
    {
        foreach (NPC npc in npcs)
        {
            if (npc.npcData.state == NPCStateEnum.Traveling)
                continue;

            NPCRoutine selectedRoutine = null;

            foreach (NPCRoutine routine in npc.npcData.routine)
            {
                if (!IsRoutineTime(routine)) continue;
                if (!IsRoutineValid(npc, routine)) continue;

                if (selectedRoutine == null ||
                    routine.priority > selectedRoutine.priority)
                {
                    selectedRoutine = routine;
                }
            }

            if (selectedRoutine != null)
            {
                npc.StartRoutine(selectedRoutine);
            }
        }
    }

    private bool IsRoutineTime(NPCRoutine routine)
    {
        return routine.startHour == Time_Controll.Instance.hours &&
            routine.startMinute == Time_Controll.Instance.minutes;
    }

    private bool IsRoutineValid(NPC npc, NPCRoutine routine)
    {
        // Dia da semana
        if (routine.validDays != null && routine.validDays.Count > 0)
        {
            WeekDayEnum today = Calendar_Controller.Instance.GetWeekDay();
            if (!routine.validDays.Contains(today))
                return false;
        }

        // Clima
        if (routine.validWeather != null && routine.validWeather.Count > 0)
        {
            WeatherEnum currentWeather = WeatherController.Instance.GetWeather();
            if (!routine.validWeather.Contains(currentWeather))
                return false;
        }

        // Afinidade
        if (npc.npcData.hearts < routine.minHearts)
            return false;

        return true;
    }

    private int ConvertToMinutes(int hour, int minute)
    {
        return hour * 60 + minute;
    }

    private NPCRoutine GetCurrentValidRoutine(NPC npc)
    {
        int currentTime = ConvertToMinutes(
            Time_Controll.Instance.hours,
            Time_Controll.Instance.minutes);

        NPCRoutine bestRoutine = null;
        int bestTime = -1;

        foreach (NPCRoutine routine in npc.npcData.routine)
        {
            if (!IsRoutineValid(npc, routine))
                continue;

            int routineTime = ConvertToMinutes(
                routine.startHour,
                routine.startMinute);

            if (routineTime <= currentTime)
            {
                if (routineTime > bestTime)
                {
                    bestTime = routineTime;
                    bestRoutine = routine;
                }
                else if (routineTime == bestTime &&
                        bestRoutine != null &&
                        routine.priority > bestRoutine.priority)
                {
                    bestRoutine = routine;
                }
            }
        }

        return bestRoutine;
    }

    private void CharaterInCutscene(List<CutsceneNPCData> npcsInCutscene)
    {
        foreach (CutsceneNPCData npcData in npcsInCutscene)
        {
            NPC npc = GetNPC(npcData.npcId);

            NPCRoutine routine = GetCurrentValidRoutine(npc);

            if (routine != null)
            {
                npc.StartRoutine(routine);
            }
        }
    }
    #endregion

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

    public void InteractWithNPC(int id, Vector2 side)
    {
        NPC npc = GetNPC(id);

        if(npc == null) return;
        
        npc.Interact(side);
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


    #region Save / Load
    public void Save(ref NPCSaveData data)
    {
        data.npcs.Clear();
        foreach(NPC npc in npcs)
        {
            NPCSaveDataData saveData = new NPCSaveDataData();
            saveData.id = npc.npcData.id;
            saveData.hearts = npc.npcData.hearts;
            data.npcs.Add(saveData);
        }
    }

    public void Load(NPCSaveData data)
    {
        foreach(NPCSaveDataData npcData in data.npcs)
        {
           NPC npc = npcs.Find(n => n.npcData.id == npcData.id);
           npc.npcData.hearts = npcData.hearts;
        }
    }
    #endregion

    #region Cutscene
    public void SetNPCInCutscene(int npcId, Vector2Int pos, SceneLocationEnum scene)
    {
        NPC npc = GetNPC(npcId);
        npc.npcData.gridPosition = pos;
        npc.npcData.location = scene;

        NPCMovement movement = npc.GetComponent<NPCMovement>();
        movement.ResetMovePointer();

        SetDataInNPCMap(pos.x, pos.y, npc.npcData.id);

        npc.transform.position = new Vector3(pos.x, pos.y, 0) + GetNPCOffset();
    }

    public IEnumerator SetNpcMovementInCutscene(int npcId, Vector2Int targetPosition, SceneLocationEnum targetLocation, NPCSide nPCSide, float speed)
    {
        NPC npc = GetNPC(npcId);

        NPCMovement movement = npc.GetComponent<NPCMovement>();

        yield return movement.SetupMoveToCutscene(
            targetPosition,
            targetLocation,
            nPCSide,
            speed
        );
    }

    public IEnumerator ShowNPCReactionInCutscene(int id, ThoughtEmoteEnum reaction)
    {
        NPC npc = GetNPC(id);

        if(npc == null) yield return null;

        npc.ShowReaction(reaction);
        yield return new WaitForSeconds(2f);
    }
    #endregion
}
