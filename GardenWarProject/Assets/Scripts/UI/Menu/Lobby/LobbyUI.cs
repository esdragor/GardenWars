using System.Collections.Generic;
using GameStates;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIComponents
{
    using Lobby;
    
    public class LobbyUI : MonoBehaviour
    {
        [Header("TeamSlots")] [SerializeField] private TeamSlot teamSlotPrefab;
        [SerializeField] private Transform team1SlotsParent;
        [SerializeField] private Transform team2SlotsParent;

        private readonly Dictionary<Enums.Team, Stack<TeamSlot>> teamSlotsQueues =
            new Dictionary<Enums.Team, Stack<TeamSlot>>();

        private readonly Dictionary<int, TeamSlot> slotsDict = new Dictionary<int, TeamSlot>();

        [Header("Champion Selection")] [SerializeField]
        private ChampionButton championButtonPrefab;

        [SerializeField] private Transform championButtonParent;

        [Header("Role Selection")] [SerializeField]
        private RoleButton roleButtonPrefab;

        [SerializeField] private Transform roleButtonParent;

        [Header("Ready Button")]
        [SerializeField] private Button readyButton;
        [SerializeField] private TextMeshProUGUI readyButtonText;

        [Header("Debug")]
        [SerializeField] private Button forceStartButton;
        [SerializeField] private TextMeshProUGUI forceStartText;
        [SerializeField] private TextMeshProUGUI roomNameText;
        private bool forceStart = false;

        private bool isReady = false;
        private GameStateMachine gameStateMachine;

        private void Start()
        {
            gameStateMachine = GameStateMachine.Instance;

            InitializeTeamSlots();

            InitializeChampionButtons();

            InitializeRoleButtons();

            SetupReadyButton();
            
            SetupForceStartButton();

            gameStateMachine.RequestAddPlayer();

            gameStateMachine.RequestSendDataDictionary();

            gameStateMachine.OnDataDictUpdated += OnPlayerDataUpdated;

            roomNameText.text = $"Room : {PhotonNetwork.CloudRegion}#{NetworkManager.Instance.currentRoomName}";
        }
        
        private void OnDisable()
        {
            gameStateMachine.OnDataDictUpdated -= OnPlayerDataUpdated;
        }

        private void InitializeTeamSlots()
        {
            foreach (var tc in gameStateMachine.teamColors)
            {
                var team = tc.team;
                teamSlotsQueues.Add(team, new Stack<TeamSlot>());
            }

            for (int i = 0; i < GameStateMachine.Instance.expectedPlayerCount / 2; i++)
            {
                var slot = Instantiate(teamSlotPrefab, Vector3.zero, Quaternion.identity, team1SlotsParent);
                slot.InitSlot(gameStateMachine.teamColors[0].color, (byte) gameStateMachine.teamColors[0].team,this);
                teamSlotsQueues[gameStateMachine.teamColors[0].team].Push(slot);
                slot = Instantiate(teamSlotPrefab, Vector3.zero, Quaternion.identity, team2SlotsParent);
                slot.InitSlot(gameStateMachine.teamColors[1].color, (byte) gameStateMachine.teamColors[1].team,this);
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
        }

        private void OnPlayerDataUpdated(int actorNumber, GameStateMachine.PlayerData data)
        {
            var local = actorNumber == PhotonNetwork.LocalPlayer.ActorNumber;

            if (!slotsDict.ContainsKey(actorNumber)) slotsDict.Add(actorNumber, null);
            
            var slot = slotsDict[actorNumber];
            
            if (slot != null)
            {
                if (data != null)
                {
                    if (data.team == slot.GetTeam())
                    {
                        slot.UpdateSlot(data,local);
                        return;
                    }
                    //clear current slot
                    slot.UpdateSlot(null,local);
                    //Enqueue slot
                    teamSlotsQueues[slot.GetTeam()].Push(slot);
                    //assign new slot
                    slot = teamSlotsQueues[data.team].Pop();
                    slotsDict[actorNumber] = slot;
                    slot.UpdateSlot(data,local);
                    return;
                }
                //clear current slot
                slot.UpdateSlot(null,local);
                //Enqueue slot
                teamSlotsQueues[slot.GetTeam()].Push(slot);
                return;

            }
            
            if (data == null) return;
            //assign new slot
            if (data.team == Enums.Team.Neutral) return;
            slot = teamSlotsQueues[data.team].Pop();
            slotsDict[actorNumber] = slot;
            slot.UpdateSlot(data,local);
        }

        private void InitializeChampionButtons()
        {
            for (byte index = 0; index < gameStateMachine.allChampionsSo.Length; index++)
            {
                var championButton = Instantiate(championButtonPrefab, Vector3.zero, Quaternion.identity,
                    championButtonParent);
                championButton.InitButton(index,this);
            }
        }

        private void InitializeRoleButtons()
        {
            foreach (var role in gameStateMachine.roles)
            {
                var roleButton = Instantiate(roleButtonPrefab, Vector3.zero, Quaternion.identity,
                    roleButtonParent);
                roleButton.InitButton(role.role, role.sprite,this);
            }
        }

        private void SetupReadyButton()
        {
            isReady = false;
            readyButtonText.text = isReady ? "Unlock" : "Lock";
            readyButton.onClick.AddListener(ToggleReady);
        }

        private void ToggleReady()
        {
            isReady = !isReady;
            readyButtonText.text = isReady ? "Unlock" : "Lock";
            
            championButtonParent.gameObject.SetActive(!isReady);
            roleButtonParent.gameObject.SetActive(!isReady);
            
            gameStateMachine.RequestSetReady(isReady);
        }

        public void SetChampion(byte championSoIndex)
        {
            if(isReady) return;
            gameStateMachine.RequestSetChampionSOIndex(championSoIndex);
        }

        public void SetRole(Enums.ChampionRole role)
        {
            if(isReady) return;
            gameStateMachine.RequestSetRole((byte)role);
        }

        public void SetTeam(Enums.Team team)
        {
            if(isReady) return;
            gameStateMachine.RequestSetTeam((byte)team);
        }

        private void SetupForceStartButton()
        {
            forceStartButton.onClick.AddListener(ToggleForceStart);
        }

        private void ToggleForceStart()
        {
            forceStart = !forceStart;
            forceStartText.text = forceStart ? "force start is ON" : "force start is OFF";
            gameStateMachine.isInDebugMode = forceStart;
        }
    }
}