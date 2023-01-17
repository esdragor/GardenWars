using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Controllers.Inputs;
using Entities.Capacities;
using Entities.Champion;
using Entities.FogOfWar;
using Entities.Inventory;
using Photon.Pun;
using UnityEngine;

namespace GameStates
{
    public partial class GameStateMachine
    {
        [Header("Loading")]
        [SerializeField] private GameObject loadingCanvas;

        [SerializeField] private int maxBytePackSize = 75000;
        
        private int expectedEmotes;
        private int receivedEmotes;
        private Dictionary<int, bool> emoteLoadingDict = new Dictionary<int, bool>();


        public void ShowLoadingCanvas(bool value)
        {
            loadingCanvas.SetActive(value);
        }

        public void LoadEmotes()
        {
            if (isMaster)
            {
                PrepareToReceiveEmoteDataRPC();
                return;
            }
            photonView.RPC("PrepareToReceiveEmoteDataRPC", RpcTarget.All);
        }
        
        [PunRPC]
        private void PrepareToReceiveEmoteDataRPC()
        {
            ShowLoadingCanvas(true);
            
            expectedEmotes = playerDataDict.Count * 6;
            receivedEmotes = 0;

            emoteLoadingDict.Clear();
            foreach (var actorNumber in playerDataDict.Keys)
            {
                emoteLoadingDict.Add(actorNumber,false);
            }

            SendEmoteData();
        }
        
        private void SendEmoteData()
        {
            for (int i = 0; i < 6; i++)
            {
                var bytes = GameSettingsManager.bytes[i];
                SendEmote(bytes,PhotonNetwork.LocalPlayer.ActorNumber,i);
            }
        }

        private async void SendEmote(byte[] bytes,int actorNumber,int index)
        {
            
            int max = maxBytePackSize;
            if (bytes.Length <= max)
            {
                photonView.RPC("SyncSendEmoteRPC", RpcTarget.All, bytes, (byte)0,actorNumber,index);
                photonView.RPC("SyncSendEmoteRPC", RpcTarget.All, Array.Empty<byte>(), (byte)2,actorNumber,index);
                return;
            }

            
            Debug.Log($"over {max}");
            
            int length;
            for (int i = 0; i < bytes.Length; i += length)
            {
                if (!Application.isPlaying) return;
                length = max;
                if (i + max > bytes.Length)
                {
                    length = bytes.Length - i;
                }

                byte[] buff = new byte[length];
                Array.Copy(bytes, i, buff, 0, length);
                if (i == 0)
                {
                    photonView.RPC("SyncSendEmoteRPC", RpcTarget.All, buff,(byte)0,actorNumber,index);
                }
                else if (i + length >= bytes.Length)
                {
                    photonView.RPC("SyncSendEmoteRPC", RpcTarget.All, buff,(byte)2,actorNumber,index);
                }
                else
                {
                    photonView.RPC("SyncSendEmoteRPC", RpcTarget.All, buff,(byte)1,actorNumber,index);
                }
                
                await Task.Delay(500);
            }
        }
        
        [PunRPC]
        public void SyncSendEmoteRPC(byte[] bytes, byte position,int actorNumber,int index)
        {
            if (position == 0) playerDataDict[actorNumber].emoteByteBuffer[index].Clear();
            
            playerDataDict[actorNumber].emoteByteBuffer[index].AddRange(bytes);
            
            if (position != 2) return;
            
            Texture2D tex = new Texture2D(2, 2);
            
            tex.LoadImage(playerDataDict[actorNumber].emoteByteBuffer[index].ToArray());
            
            //tex.LoadRawTextureData(playerDataDict[actorNumber].emoteByteBuffer[index].ToArray());
            
            tex.Apply();
            
            playerDataDict[actorNumber].emotesTextures[index] = tex;
            
            receivedEmotes++;
            
            OnReceivedEmote();
            
            
        }

        private void OnReceivedEmote()
        {
            //Debug.Log($"Receive emote, now at {receivedEmotes}/{expectedEmotes}");
            
            if(receivedEmotes < expectedEmotes) return;
            
            OnAllEmoteReceived();
        }

        private void OnAllEmoteReceived()
        {
            if(emoteLoadingDict[PhotonNetwork.LocalPlayer.ActorNumber]) return;
            
            emoteLoadingDict[PhotonNetwork.LocalPlayer.ActorNumber] = true;
            
            photonView.RPC("SendReceivedEmoteRPC", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
        }

        [PunRPC]
        private void SendReceivedEmoteRPC(int actorNumber)
        {
            emoteLoadingDict[actorNumber] = true;

            bool everybodyReady = true;
            foreach (var _ in emoteLoadingDict.Values.Where(doneLoading => doneLoading == false))
            {
                everybodyReady = false;
            }
            
            if(!everybodyReady) return;
            
            StartLoadingMap();
        }
        
        private void StartLoadingMap()
        {
            MoveToGameScene();
        }

        private void MoveToGameScene()
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
            
            UIManager.Instance.SetupEmoteWheel();
            
            UIManager.Instance.HideSettings();

            foreach (var championSo in allChampionsSo)
            {
                championSo.SetIndexes();
            }

            ItemCollectionManager.Instance.LinkCapacityIndexes();
            
            LocalPoolManager.Init();
            
            NetworkPoolManager.Init();
            
            InstantiateChampion();

            InitEntitySpawner();

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
            
            FogOfWarManager.RunFog();
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
            
            Debug.Log("Initiating Spawner");
            
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

    }
}
