using System;
using Entities.Champion;
using Entities.Inventory;
using Photon.Pun;
using UnityEngine;

namespace GameStates
{
    public class MapLoaderManager : MonoBehaviourPun
    {
        public static MapLoaderManager Instance;

        public ChampionSpawner[] firstTeamBasePoint;
        public ChampionSpawner[] secondTeamBasePoint;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                DestroyImmediate(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            var gsm = GameStateMachine.Instance;
            if (!GameStateMachine.isOffline)
            {
                gsm.LoadMap();
                if (PhotonNetwork.IsMasterClient) PhotonNetwork.IsMessageQueueRunning = true;
                return;
            }
            
            ItemCollectionManager.Instance.LinkCapacityIndexes();
        }

        [Serializable]
        public class ChampionSpawner
        {
            public Transform position;
            public Champion champion;
        }
    }
}