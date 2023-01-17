using System;
using System.Collections.Generic;
using System.Linq;
using Controllers.Inputs;
using Entities.Capacities;
using Entities.Champion;
using Entities.Inventory;
using Photon.Pun;
using UnityEngine;

namespace GameStates
{
    using States;

    [RequireComponent(typeof(PhotonView))]
    public partial class GameStateMachine : MonoBehaviourPun
    {
        public static GameStateMachine Instance;
        public static bool isOffline => !PhotonNetwork.IsConnected;
        public static bool isMaster => isOffline || PhotonNetwork.IsMasterClient;
        private GameState currentState;
        private GameState[] gamesStates;

        [Header("Debug")] [SerializeField] private string gameSceneName;
        public bool isInDebugMode = false;

        [Header("InGameSettings")] [SerializeField]
        private double ticksPerSecond = 1;

        public double tickRate => ticksPerSecond > 0 ? ticksPerSecond : 1;
        public double increasePerTick => 1 / tickRate;
        public uint expectedPlayerCount = 4;

        [Header("Collections")] public ChampionSO[] allChampionsSo;
        public Role[] roles;
        public List<int> allPlayersIDs = new List<int>();

        private readonly Dictionary<int, PlayerData> playerDataDict =
            new Dictionary<int, PlayerData>();

        [Header("InGameData")]
        public Enums.Team winner = Enums.Team.Neutral;
        private List<int> scores = new List<int>();
        [SerializeField] private int scoreToWin = 10;
        [HideInInspector] public double startTime;

        [Serializable]
        public struct Role
        {
            public Enums.ChampionRole role;
            public Sprite sprite;
        }

        [Serializable]
        public class PlayerData
        {
            public bool isReady;

            //pre lobby
            public string name;
            public byte[][] emotesArrays = new byte[6][];

            //lobby
            public Enums.Team team;
            public byte championSOIndex;
            public Enums.ChampionRole role;

            //in game
            public int championPhotonViewId;
            public Champion champion;
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                DestroyImmediate(this);
                return;
            }

            Instance = this;
            PhotonNetwork.AutomaticallySyncScene = true;

            gamesStates = new GameState[4];
            gamesStates[0] = new LobbyState(this);
            gamesStates[1] = new LoadingState(this);
            gamesStates[2] = new InGameState(this);
            gamesStates[3] = new PostGameState(this);

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (isMaster)
            {
                InitState();
            }
            else
            {
                RequestStartCurrentState();
            }
        }

        private void Update()
        {
            currentState?.UpdateState();
        }

        private void InitState()
        {
            currentState = gamesStates[0];
            if (isOffline) currentState = gamesStates[2];
            currentState.StartState();
        }

        public void SwitchState(byte stateIndex)
        {
            photonView.RPC("SyncSwitchStateRPC", RpcTarget.All, stateIndex);
        }

        [PunRPC]
        private void SyncSwitchStateRPC(byte stateIndex)
        {
            currentState.ExitState();
            currentState = gamesStates[stateIndex];
            currentState.StartState();
        }

        private void RequestStartCurrentState()
        {
            photonView.RPC("StartCurrentStateRPC", RpcTarget.MasterClient);
        }

        [PunRPC]
        private void StartCurrentStateRPC()
        {
            byte index = 255;
            for (int i = 0; i < gamesStates.Length - 1; i++)
            {
                if (gamesStates[i] == currentState) index = (byte)i;
            }

            if (index == 255)
            {
                Debug.LogError("Index is not valid.");
                return;
            }

            photonView.RPC("SyncStartCurrentStateRPC", RpcTarget.All, index);
        }

        [PunRPC]
        private void SyncStartCurrentStateRPC(byte index)
        {
            if (currentState != null) return; // We don't want to sync a client already synced

            currentState = gamesStates[index];
            currentState.StartState();
        }

        public void Tick()
        {
            OnTick?.Invoke();
        }

        public void TickFeedback()
        {
            OnTickFeedback?.Invoke();
        }

        public void UpdateEvent()
        {
            OnUpdate?.Invoke();
        }

        public void UpdateEventFeedback()
        {
            OnUpdateFeedback?.Invoke();
        }

        public event GlobalDelegates.NoParameterDelegate OnUpdate;
        public event GlobalDelegates.NoParameterDelegate OnUpdateFeedback;

        public event GlobalDelegates.NoParameterDelegate OnTick;
        public event GlobalDelegates.NoParameterDelegate OnTickFeedback;
        
        #region GetPlayerData

        public Enums.Team GetPlayerTeam(int actorNumber)
        {
            return playerDataDict.ContainsKey(actorNumber) ? playerDataDict[actorNumber].team : Enums.Team.Neutral;
        }

        public Enums.Team GetPlayerTeam()
        {
            return GetPlayerTeam(PhotonNetwork.LocalPlayer.ActorNumber);
        }

        public Enums.ChampionRole GetPlayerRole(int actorNumber)
        {
            return playerDataDict.ContainsKey(actorNumber)
                ? playerDataDict[actorNumber].role
                : Enums.ChampionRole.Fighter;
        }

        public Enums.ChampionRole GetPlayerRole()
        {
            return GetPlayerRole(PhotonNetwork.LocalPlayer.ActorNumber);
        }

        public byte GetPlayerChampionSOIndex(int actorNumber)
        {
            return playerDataDict.ContainsKey(actorNumber) ? playerDataDict[actorNumber].championSOIndex : (byte)0;
        }

        public byte GetPlayerChampionSOIndex()
        {
            return GetPlayerChampionSOIndex(PhotonNetwork.LocalPlayer.ActorNumber);
        }

        public int GetPlayerChampionPhotonViewId(int actorNumber)
        {
            return playerDataDict[actorNumber].championPhotonViewId;
        }

        public int GetPlayerChampionPhotonViewId()
        {
            return playerDataDict[PhotonNetwork.LocalPlayer.ActorNumber].championPhotonViewId;
        }

        public Champion GetPlayerChampion(int actorNumber)
        {
            return playerDataDict[actorNumber].champion;
        }

        public Champion GetPlayerChampion()
        {
            return isOffline ? playerDataDict[-1].champion : playerDataDict[PhotonNetwork.LocalPlayer.ActorNumber].champion;
        }

        #endregion
        

        public void SendWinner(Enums.Team team)
        {
            photonView.RPC("SyncWinnerRPC", RpcTarget.All, (byte)team);
        }

        [PunRPC]
        private void SyncWinnerRPC(byte team)
        {
            winner = (Enums.Team)team;
        }

        public void ResetScore()
        {
            scores.Clear();
            foreach (var team in Enum.GetValues(typeof(Enums.Team)))
            {
                scores.Add(0);
            }
        }

        public int GetTeamScore(Enums.Team team)
        {
            return scores[(int)team];
        }
        
        public void IncreaseScore(Enums.Team team)
        {
            if (!isMaster || isOffline) return;
            scores[(int)team]++;
            OnTeamIncreaseScore?.Invoke((byte)team);
            for (var i = 0; i < scores.Count; i++)
            {
                if (scores[i] <= scoreToWin) continue;
                winner = (Enums.Team)i;
                break;
            }

            photonView.RPC("SyncIncreaseScoreRPC", RpcTarget.All, (byte)team);
        }

        [PunRPC]
        private void SyncIncreaseScoreRPC(byte team)
        {
            if (!isMaster) scores[team]++;
            OnTeamIncreaseScoreFeedBack?.Invoke(team);
            Debug.Log($"increase score of team {team} ({scores[team]})");
        }
        
        public event Action<byte> OnTeamIncreaseScore;
        public event Action<byte> OnTeamIncreaseScoreFeedBack;
    }
}