using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameStates;
using Photon.Pun;
using UIComponents.Lobby;
using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    [Header("TeamSlots")]
    [SerializeField] private TeamSlot teamSlotPrefab;
    [SerializeField] private Transform team1SlotsParent;
    [SerializeField] private Transform team2SlotsParent;

    private readonly Dictionary<Enums.Team, Stack<TeamSlot>> teamSlotsQueues =
        new Dictionary<Enums.Team, Stack<TeamSlot>>();
    private readonly Dictionary<int, TeamSlot> slotsDict = new Dictionary<int, TeamSlot>();


    private GameStateMachine gameStateMachine;
    
    private void Start()
    {
        gameStateMachine = GameStateMachine.Instance;
        foreach (var tc in gameStateMachine.teamColors)
        {
            var team = tc.team;
            teamSlotsQueues.Add(team,new Stack<TeamSlot>());
        }
        for (int i = 0; i < GameStateMachine.Instance.expectedPlayerCount/2; i++)
        {
            var slot = Instantiate(teamSlotPrefab,Vector3.zero, Quaternion.identity,team1SlotsParent);
            slot.SetupSlot(gameStateMachine.teamColors[0].color,(byte)gameStateMachine.teamColors[0].team);
            teamSlotsQueues[gameStateMachine.teamColors[0].team].Push(slot);
            slot = Instantiate(teamSlotPrefab,Vector3.zero, Quaternion.identity,team2SlotsParent);
            slot.SetupSlot(gameStateMachine.teamColors[1].color,(byte)gameStateMachine.teamColors[1].team);
            teamSlotsQueues[gameStateMachine.teamColors[1].team].Push(slot);
        }
        
        for (var i = 0; i < team1SlotsParent.childCount - 1; i++)
        {
            team1SlotsParent.GetChild(0).SetSiblingIndex(team1SlotsParent.childCount - 1 - i);
        }
        for (var i = 0; i < team2SlotsParent.childCount - 1; i++)
        {
            team2SlotsParent.GetChild(0).SetSiblingIndex(team2SlotsParent.childCount - 1 - i);
        }

        gameStateMachine.RequestAddPlayer();
        
        gameStateMachine.RequestSendDataDictionary();

        gameStateMachine.OnDataDictUpdated += OnPlayerDataUpdated;
    }

    private void OnPlayerDataUpdated(int actorNumber,GameStateMachine.PlayerData data)
    {
        Debug.Log("Data Updated");
        if(!slotsDict.ContainsKey(actorNumber)) slotsDict.Add(actorNumber,null);
        
        Debug.Log($"Is Data null ? : {data == null}");
        var slot = slotsDict[actorNumber];
        Debug.Log($"Is slot null ? : {slot == null}");


        if (slot != null)
        {
            Debug.Log($"Player {actorNumber} already has a slot");
            if (data != null)
            {
                Debug.Log($"Data is not null");
                if (data.team == slot.GetTeam())
                {
                    Debug.Log($"Player didn't change teams");
                    slot.UpdateSlot(data);
                    return;
                }
                else
                {
                    Debug.Log($"Player changed teams");
                    //clear current slot
                    slot.UpdateSlot(null);
                    //Enqueue slot
                    teamSlotsQueues[slot.GetTeam()].Push(slot);
                    //assign new slot
                    slot = teamSlotsQueues[data.team].Pop();
                    slotsDict[actorNumber] = slot;
                    slot.UpdateSlot(data);
                    return;
                }
            }
            else
            {
                Debug.Log($"Data is null");
                //clear current slot
                slot.UpdateSlot(null);
                //Enqueue slot
                teamSlotsQueues[slot.GetTeam()].Push(slot);
                return;
            }
            
        }
        else
        {
            Debug.Log($"Player {actorNumber} doesn't have a slot");
            if (data != null)
            {
                Debug.Log($"Data is not null (team is {data.team})");
                //assign new slot
                if (data.team != Enums.Team.Neutral)
                {
                    slot = teamSlotsQueues[data.team].Pop();
                    slotsDict[actorNumber] = slot;
                    slot.UpdateSlot(data);
                    return;
                }
            }
            else
            {
                Debug.Log($"Data is null");
                return;
            }
            
        }
    }
}
