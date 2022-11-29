using System.Collections;
using System.Collections.Generic;
using GameStates;
using UIComponents.Lobby;
using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    [Header("TeamSlots")]
    [SerializeField] private TeamSlot teamSlotPrefab;
    [SerializeField] private Transform team1SlotsParent;
    [SerializeField] private Transform team2SlotsParent;

    private readonly Dictionary<Enums.Team, Queue<TeamSlot>> teamSlotsQueues =
        new Dictionary<Enums.Team, Queue<TeamSlot>>();
    private readonly Dictionary<int, TeamSlot> slotsDict = new Dictionary<int, TeamSlot>();


    private GameStateMachine gameStateMachine;
    
    private void Start()
    {
        gameStateMachine = GameStateMachine.Instance;
        foreach (var tc in gameStateMachine.teamColors)
        {
            var team = tc.team;
            teamSlotsQueues.Add(team,new Queue<TeamSlot>());
        }
        for (int i = 0; i < GameStateMachine.Instance.expectedPlayerCount/2; i++)
        {
            var slot = Instantiate(teamSlotPrefab,Vector3.zero, Quaternion.identity,team1SlotsParent);
            slot.SetupSlot(gameStateMachine.teamColors[0].color,0,(byte)gameStateMachine.teamColors[0].team);
            teamSlotsQueues[gameStateMachine.teamColors[0].team].Enqueue(slot);
            slot =Instantiate(teamSlotPrefab,Vector3.zero, Quaternion.identity,team2SlotsParent);
            slot.SetupSlot(gameStateMachine.teamColors[1].color,1,(byte)gameStateMachine.teamColors[1].team);
            teamSlotsQueues[gameStateMachine.teamColors[1].team].Enqueue(slot);
        }

        gameStateMachine.OnDataDictUpdated += OnPlayerDataUpdated;
    }

    private void OnPlayerDataUpdated(int actorNumber,GameStateMachine.PlayerData data)
    {
        if(!slotsDict.ContainsKey(actorNumber)) slotsDict.Add(actorNumber,null);
        
        var slot = slotsDict[actorNumber];

        if (data == null && slot != null)
        {
            //unassign slot
            slot.UpdateSlot(null);
            teamSlotsQueues[slot.GetTeam()].Enqueue(slot);
            return;   
        }

        if (data.team == Enums.Team.Neutral)
        {
            //unassign slot
            slot.UpdateSlot(null);
            teamSlotsQueues[slot.GetTeam()].Enqueue(slot);
            return;
        }
        
        if (slot == null)
        {
            //assign available slot in team
            return;
        }
        

        if (slot.GetTeam() != data.team)
        {
            //unassign slot
            slot.UpdateSlot(null);
            teamSlotsQueues[slot.GetTeam()].Enqueue(slot);
            
            //assign available slot in team
            //refresh slots for team
        }
        




    }
}
