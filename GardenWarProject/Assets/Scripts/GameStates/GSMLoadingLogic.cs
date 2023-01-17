using System.Collections;
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
    public partial class GameStateMachine
    {
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

    }
}
