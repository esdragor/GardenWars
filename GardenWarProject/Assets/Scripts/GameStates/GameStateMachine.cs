using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers.Inputs;
using Entities.Champion;
using Entities.Inventory;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

namespace GameStates
{
    using States;

    [RequireComponent(typeof(PhotonView))]
    public class GameStateMachine : MonoBehaviourPun
    {
        public static GameStateMachine Instance;
        public bool IsMaster => PhotonNetwork.IsMasterClient;

        [SerializeField] private string gameSceneName;

        private GameState currentState;
        private GameState[] gamesStates;

        [Tooltip("Ticks per second")] public double tickRate = 1;

        public Enums.Team winner = Enums.Team.Neutral;
        public List<int> allPlayersIDs = new List<int>();

        private readonly Dictionary<int, PlayerData> playerDataDict =
            new Dictionary<int, PlayerData>();

        public uint expectedPlayerCount = 4;

        public ChampionSO[] allChampionsSo;
        public Enums.Team[] allTeams;

        public TeamColor[] teamColors;

        public bool isInDebugMode = false;

        [Serializable]
        public struct TeamColor
        {
            public Enums.Team team;
            public Color color;
        }

        [Serializable]
        public class PlayerData
        {
            public string name;
            public Enums.Team team;
            public byte championSOIndex;
            public Enums.ChampionRole role;
            public bool isReady;
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

            if (tickRate <= 0)
            {
                Debug.LogWarning("TickRate can't be negative. Set to 1");
                tickRate = 1;
            }

            gamesStates = new GameState[4];
            gamesStates[0] = new LobbyState(this);
            gamesStates[1] = new LoadingState(this);
            gamesStates[2] = new InGameState(this);
            gamesStates[3] = new PostGameState(this);

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (PhotonNetwork.IsMasterClient)
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

        public event GlobalDelegates.NoParameterDelegate OnTick;
        public event GlobalDelegates.NoParameterDelegate OnTickFeedback;

        #region PlayerJoin/Leave

        public void RequestAddPlayer()
        {
            photonView.RPC("AddPlayerRPC", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
        }

        [PunRPC]
        private void AddPlayerRPC(int actorNumber)
        {
            photonView.RPC("SyncAddPlayerRPC", RpcTarget.All, actorNumber);
        }

        [PunRPC]
        private void SyncAddPlayerRPC(int actorNumber)
        {
            if (playerDataDict.ContainsKey(actorNumber))
            {
                Debug.LogWarning($"This player already exists (on {PhotonNetwork.LocalPlayer.ActorNumber})!");
            }
            else
            {
                var playerData = new PlayerData
                {
                    isReady = false,
                    team = Enums.Team.Neutral,
                    role = Enums.ChampionRole.Fighter,
                    championSOIndex = 255,
                    championPhotonViewId = -1,
                    champion = null
                };
                playerDataDict.Add(actorNumber, playerData);
                allPlayersIDs.Add(actorNumber);
            }
        }

        public void RequestRemovePlayer()
        {
            photonView.RPC("RemovePlayerRPC", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
        }

        [PunRPC]
        private void RemovePlayerRPC(int actorNumber)
        {
            photonView.RPC("SyncRemovePlayerRPC", RpcTarget.All, actorNumber);
        }

        [PunRPC]
        private void SyncRemovePlayerRPC(int actorNumber)
        {
            if (!playerDataDict.ContainsKey(actorNumber)) return;
            playerDataDict.Remove(actorNumber);
            allPlayersIDs.Remove(actorNumber);
            OnDataDictUpdated?.Invoke(actorNumber,null);
        }

        #endregion

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
            return playerDataDict[PhotonNetwork.LocalPlayer.ActorNumber].champion;
        }

        #endregion

        #region SetPlayerData

        public void RequestSetReady(bool ready)
        {
            photonView.RPC("SetReadyRPC", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber, ready);
        }

        [PunRPC]
        private void SetReadyRPC(int actorNumber, bool ready)
        {
            if (!playerDataDict.ContainsKey(actorNumber))
            {
                Debug.LogError("This key is not valid.");
                return;
            }

            playerDataDict[actorNumber].isReady = ready;
            if (!IsMaster) return;
            if (!playerDataDict[actorNumber].isReady) return;
            OnPlayerSetReady();
        }

        public void RequestSetTeam(byte team)
        {
            photonView.RPC("SetTeamRPC", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber, team);
        }

        [PunRPC]
        private void SetTeamRPC(int actorNumber, byte team)
        {
            photonView.RPC("SyncSetTeamRPC", RpcTarget.All, actorNumber, team);
        }

        [PunRPC]
        private void SyncSetTeamRPC(int actorNumber, byte team)
        {
            if (!playerDataDict.ContainsKey(actorNumber))
            {
                Debug.LogWarning($"This player is not added (on {PhotonNetwork.LocalPlayer.ActorNumber}).");
                return;
            }

            playerDataDict[actorNumber].team = (Enums.Team)team;
        }

        public void RequestSetRole(byte role)
        {
            photonView.RPC("SetRoleRPC", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber, role);
        }

        [PunRPC]
        private void SetRoleRPC(int actorNumber, byte role)
        {
            photonView.RPC("SyncSetRoleRPC", RpcTarget.All, actorNumber, role);
        }

        [PunRPC]
        private void SyncSetRoleRPC(int actorNumber, byte role)
        {
            if (!playerDataDict.ContainsKey(actorNumber)) return;

            playerDataDict[actorNumber].role = (Enums.ChampionRole)role;
        }

        public void RequestSetChampionSOIndex(byte index)
        {
            photonView.RPC("SetChampionSOIndexRPC", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber,
                index);
        }

        [PunRPC]
        private void SetChampionSOIndexRPC(int actorNumber, byte index)
        {
            photonView.RPC("SyncSetChampionSOIndexRPC", RpcTarget.All, actorNumber, index);
        }

        [PunRPC]
        private void SyncSetChampionSOIndexRPC(int actorNumber, byte index)
        {
            if (!playerDataDict.ContainsKey(actorNumber)) return;

            playerDataDict[actorNumber].championSOIndex = index;
        }

        #endregion

        public void RequestSendDataDictionary()
        {
            photonView.RPC("SendDataDictionaryRPC", RpcTarget.MasterClient);
        }

        [PunRPC]
        private void SendDataDictionaryRPC()
        {
            foreach (var kvp in playerDataDict)
            {
                photonView.RPC("SyncDataDictionaryRPC", RpcTarget.Others, kvp.Key, kvp.Value.isReady,
                    (byte)kvp.Value.team,
                    (byte)kvp.Value.role, kvp.Value.championSOIndex);
            }
        }

        public void RequestDataSync(int actorNumber)
        {
            photonView.RPC("DataSyncRPC",RpcTarget.MasterClient,actorNumber);
        }
        
        
        [PunRPC]
        private void DataSyncRPC(int actorNumber)
        {
            if(!playerDataDict.ContainsKey(actorNumber)) return;
            var data = playerDataDict[actorNumber];
            photonView.RPC("SyncDataDictionaryRPC",RpcTarget.All,actorNumber,data.isReady,(byte)data.team,(byte)data.role,data.championSOIndex);
        }

        [PunRPC]
        private void SyncDataDictionaryRPC(int actorNumber, bool isReady, byte team, byte role, byte championSOindex)
        {
            var data = new PlayerData
            {
                isReady = isReady,
                team = (Enums.Team)team,
                role = (Enums.ChampionRole)role,
                championSOIndex = championSOindex
            };
            if (!playerDataDict.ContainsKey(actorNumber)) playerDataDict.Add(actorNumber, data);
            else playerDataDict[actorNumber] = data;
            OnDataDictUpdated?.Invoke(actorNumber,data);
        }

        public event GlobalDelegates.IntPlayerDataDelegate OnDataDictUpdated;

        #region OnPlayerSetReady

        private void OnPlayerSetReady()
        {
            if (!CanChangeState()) return;

            foreach (var data in playerDataDict.Select(kvp => kvp.Value))
            {
                data.isReady = false;
            }

            currentState.OnAllPlayerReady();
        }

        private bool CanChangeState()
        {
            if (playerDataDict.Count < expectedPlayerCount)
            {
                Debug.Log($"Not enough players expected at least {expectedPlayerCount}, found {playerDataDict.Count}");
                return false; //enough players ?
            }

            var team1 = new List<PlayerData>();
            var team2 = new List<PlayerData>();
            foreach (var data in playerDataDict.Select(kvp => kvp.Value))
            {
                if (!data.isReady)
                {
                    Debug.Log($"A player is not ready");
                    return false; // everyone isReady ?
                }
                if (data.team == Enums.Team.Team1) team1.Add(data);
                if (data.team == Enums.Team.Team2) team2.Add(data);
                if (data.championSOIndex >= allChampionsSo.Length)
                {
                    Debug.Log($"{data.championSOIndex} is not a valid championSO index");
                    return false; // valid championSOIndex ? 
                }
            }

            if (isInDebugMode)
            {
                Debug.Log("In debug mode, skipping some steps");
                return true;
            }

            if (team1.Count != 2)
            {
                Debug.Log($"Team1 doesn't have 2 players ({team1.Count})");
                return false;
            }

            if (team1.Count != team2.Count)
            {
                Debug.Log($"Team1 and Team2 don't have the same amount of players ({team1.Count} and {team2.Count})");
                return false;
            }

            if (team1[0].role == team1[1].role)
            {
                Debug.Log($"Team1 members have the same role {team1[0].role} and {team1[1].role}");
                return false;
            }

            if (team2[0].role == team2[1].role)
            {
                Debug.Log($"Team1 members have the same role {team2[0].role} and {team2[1].role}");
                return false;
            }

            Debug.Log("Good to go");
            return true;
        }

        public void ResetPlayerReady()
        {
            foreach (var data in playerDataDict.Select(kvp => kvp.Value))
            {
                data.isReady = false;
            }
        }

        #endregion


        public void StartLoadingMap()
        {
            LobbyUIManager.Instance.SendStartGame();
            SwitchState(1);
        }

        public void MoveToGameScene()
        {
            PhotonNetwork.IsMessageQueueRunning = false;
            PhotonNetwork.LoadLevel(gameSceneName);
        }

        /// <summary>
        /// Executed by MapLoaderManager on a GO on the scene 'gameSceneName', so only once the scene is loaded
        /// </summary>
        public void LoadMap()
        {
            // TODO - init pools

            LinkChampionSOCapacityIndexes();

            ItemCollectionManager.Instance.LinkCapacityIndexes();

            InstantiateChampion();

            RequestSetReady(true);
        }

        /// <summary>
        /// Executed during the exit of loading state, so after every champion is instantiated and every indexes are linked
        /// </summary>
        public void LateLoad()
        {
            LinkLoadChampionData();

            SetupUI();
        }

        private void LinkChampionSOCapacityIndexes()
        {
            foreach (var championSo in allChampionsSo)
            {
                championSo.SetIndexes();
            }
        }

        private void InstantiateChampion()
        {
            var champion = (Champion)PoolNetworkManager.Instance.PoolInstantiate(0, Vector3.up, Quaternion.identity);

            photonView.RPC("SyncChampionPhotonId", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber,
                champion.photonView.ViewID);

            champion.name = $"Player ID:{PhotonNetwork.LocalPlayer.ActorNumber} [MINE]";
            LinkController(champion);
        }

        [PunRPC]
        private void SyncChampionPhotonId(int photonId, int photonViewId)
        {
            var champion = PhotonNetwork.GetPhotonView(photonViewId);
            playerDataDict[photonId].championPhotonViewId = champion.ViewID;
            playerDataDict[photonId].champion = champion.GetComponent<Champion>();

            champion.name = $"Player ID : {photonId}";
        }

        private void LinkLoadChampionData()
        {
            foreach (var playerData in playerDataDict.Values)
            {
                ApplyChampionSoData(playerData);
            }
        }

        private void LinkController(Champion champion)
        {
            var controller = champion.GetComponent<PlayerInputController>();

            // We set local parameters
            controller.LinkControlsToPlayer();
            controller.LinkCameraToPlayer();
        }

        private void ApplyChampionSoData(PlayerData playerData)
        {
            if (playerData.championSOIndex >= allChampionsSo.Length)
            {
                Debug.LogWarning("Make sure the mesh is valid. Selects default mesh.");
                playerData.championSOIndex = 1;
            }

            var championSo = allChampionsSo[playerData.championSOIndex];

            // We state name
            playerData.champion.name += $" / {championSo.name}";

            // We sync data and champion mesh
            playerData.champion.ApplyChampionSO(playerData.championSOIndex, playerData.team);
        }

        private void SetupUI()
        {
            if (UIManager.Instance == null) return;

            UIManager.Instance.InstantiateChampionHUD();

            foreach (var actorNumber in playerDataDict)
            {
                UIManager.Instance.AssignInventory(actorNumber.Key);
            }
        }

        public void SendWinner(Enums.Team team)
        {
            photonView.RPC("SyncWinnerRPC", RpcTarget.All, (byte)team);
        }

        [PunRPC]
        private void SyncWinnerRPC(byte team)
        {
            winner = (Enums.Team)team;
        }

        public static Vector3 GetClosestValidPoint(Vector3 position)
        {
            if (NavMesh.SamplePosition(position, out var hit, 5, NavMesh.AllAreas))
            {
                return hit.position;
            }

            position.y = 0;
            return position;
        }
    }
}