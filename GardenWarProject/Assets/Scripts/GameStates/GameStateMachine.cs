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
    public class GameStateMachine : MonoBehaviourPun
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
        public TeamColor[] teamColors;
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

        #region PlayerJoin/Leave

        public void RequestAddPlayer()
        {
            if (isMaster)
            {
                AddPlayerRPC(PhotonNetwork.LocalPlayer.ActorNumber);
                return;
            }

            photonView.RPC("AddPlayerRPC", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
        }

        [PunRPC]
        private void AddPlayerRPC(int actorNumber)
        {
            if (isOffline)
            {
                SyncAddPlayerRPC(actorNumber);
                return;
            }

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
                    champion = null,
                    name = $"Player {actorNumber}"
                };
                playerDataDict.Add(actorNumber, playerData);
                allPlayersIDs.Add(actorNumber);
            }
        }

        public static void AddOfflinePlayer(Champion champion, Enums.Team team, Enums.ChampionRole role)
        {
            if (!isOffline) return;
            var playerData = new PlayerData
            {
                isReady = false,
                team = team,
                role = role,
                championSOIndex = 255,
                championPhotonViewId = -1,
                champion = champion,
                name = $"Offline Player"
            };
            Instance.playerDataDict.Add(-1, playerData);
            Instance.allPlayersIDs.Add(-1);
        }

        public void RequestRemovePlayer()
        {
            photonView.RPC("RemovePlayerRPC", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
        }

        [PunRPC]
        private void RemovePlayerRPC(int actorNumber)
        {
            Debug.Log($"Trying to remove actor {actorNumber}");
            photonView.RPC("SyncRemovePlayerRPC", RpcTarget.All, actorNumber);
        }

        [PunRPC]
        private void SyncRemovePlayerRPC(int actorNumber)
        {
            if (!playerDataDict.ContainsKey(actorNumber)) return;
            playerDataDict.Remove(actorNumber);
            allPlayersIDs.Remove(actorNumber);
            OnDataDictUpdated?.Invoke(actorNumber, null);
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
            return isOffline ? playerDataDict[-1].champion : playerDataDict[PhotonNetwork.LocalPlayer.ActorNumber].champion;
        }

        #endregion

        #region SetPlayerData

        public void RequestSetReady(bool ready)
        {
            if (isMaster)
            {
                SetReadyRPC(PhotonNetwork.LocalPlayer.ActorNumber, ready);
                return;
            }

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
            OnDataDictUpdated?.Invoke(actorNumber, playerDataDict[actorNumber]);
            if (!isMaster) return;
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
            OnDataDictUpdated?.Invoke(actorNumber, playerDataDict[actorNumber]);
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
            OnDataDictUpdated?.Invoke(actorNumber, playerDataDict[actorNumber]);
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
            OnDataDictUpdated?.Invoke(actorNumber, playerDataDict[actorNumber]);
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
                var values = kvp.Value;
                photonView.RPC("SyncDataDictionaryRPC", RpcTarget.Others, kvp.Key, values.name, values.isReady,
                    (byte)values.team,
                    (byte)values.role, values.championSOIndex);
            }
        }

        public void RequestDataSync(int actorNumber)
        {
            photonView.RPC("DataSyncRPC", RpcTarget.MasterClient, actorNumber);
        }


        [PunRPC]
        private void DataSyncRPC(int actorNumber)
        {
            if (!playerDataDict.ContainsKey(actorNumber)) return;
            var data = playerDataDict[actorNumber];
            photonView.RPC("SyncDataDictionaryRPC", RpcTarget.All, actorNumber, data.name, data.isReady,
                (byte)data.team, (byte)data.role, data.championSOIndex);
        }

        [PunRPC]
        private void SyncDataDictionaryRPC(int actorNumber, string playerName, bool isReady, byte team, byte role,
            byte championSOindex)
        {
            var data = new PlayerData
            {
                name = playerName,
                isReady = isReady,
                team = (Enums.Team)team,
                role = (Enums.ChampionRole)role,
                championSOIndex = championSOindex
            };
            if (!playerDataDict.ContainsKey(actorNumber)) playerDataDict.Add(actorNumber, data);
            else playerDataDict[actorNumber] = data;
            OnDataDictUpdated?.Invoke(actorNumber, data);
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
            if (playerDataDict[PhotonNetwork.MasterClient.ActorNumber].team == Enums.Team.Neutral)
            {
                Debug.Log($"Master is in team {playerDataDict[PhotonNetwork.MasterClient.ActorNumber].team}");
                return false;
            }

            var neutralPlayersIds = playerDataDict.Where(kvp => kvp.Value.team == Enums.Team.Neutral).Select(kvp => kvp.Key);
            foreach (var key in neutralPlayersIds)
            {
                playerDataDict.Remove(key);
            }
            
            if (isInDebugMode)
            {
                foreach (var data in playerDataDict.Select(kvp => kvp.Value))
                {
                    if (!data.isReady)
                    {
                        Debug.Log($"A player is not ready");
                        if (currentState != gamesStates[0]) return false; // everyone isReady ?
                    }

                    if (data.championSOIndex >= allChampionsSo.Length)
                    {
                        Debug.Log($"{data.championSOIndex} is not a valid championSO index");
                        return false; // valid championSOIndex ? 
                    }
                }

                Debug.Log("In debug mode, skipping some steps");
                return true;
            }

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
            CapacitySOCollectionManager.Instance.SetIndexes();

            foreach (var championSo in allChampionsSo)
            {
                championSo.SetIndexes();
            }

            ItemCollectionManager.Instance.LinkCapacityIndexes();
            
            LocalPoolManager.Init();
            
            NetworkPoolManager.Init();

            InstantiateChampion();

            InitEntitySpawner();

            Debug.Log("Done Loading");
            RequestSetReady(true);
        }

        /// <summary>
        /// Executed during the exit of loading state, so after every champion is instantiated and every indexes are linked
        /// </summary>
        public void LateLoad()
        {
            SyncEntitySpawner();
            
            foreach (var playerData in playerDataDict.Values)
            {
                ApplyChampionSoData(playerData);
            }
            
            SetupUI();

            foreach (var champion in playerDataDict.Select(kvp => kvp.Value).Select(value => value.champion))
            {
                SetupChampion(champion);
                UIManager.Instance.InitPlayerIcon(champion);
            }
        }

        public static void SetupChampion(Champion champion)
        {
            champion.SetupSpawn();
            champion.SetupNavMesh();
            champion.SetupUI();
            if (isMaster) champion.SyncInstantiate(champion.team);
        }

        private void InstantiateChampion()
        {
            var champion = NetworkPoolManager.PoolInstantiate("NewPlayer", Vector3.up, Quaternion.identity).GetComponent<Champion>();
            champion.OnInstantiated();
            champion.OnInstantiatedFeedback();

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
            playerData.champion.ApplyChampionSO(playerData.championSOIndex, playerData.team, playerData.role);
        }

        private void SetupUI()
        {
            if (UIManager.Instance == null) return;

            UIManager.Instance.InstantiateChampionHUD();
            
            UIManager.Instance.SetupTopBar();

            foreach (var actorNumber in playerDataDict)
            {
                UIManager.Instance.AssignInventory(actorNumber.Key);
            }
        }

        private void InitEntitySpawner()
        {
            if (!isMaster || isOffline) return;
            SpawnAIs.Instance.Init();
        }

        private void SyncEntitySpawner()
        {
            if (!isMaster || isOffline) return;
            SpawnAIs.Instance.Sync();
        }

        public void StartEntitySpawner()
        {
            if (!isMaster || isOffline) return;
            SpawnAIs.Instance.StartSpawns();
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