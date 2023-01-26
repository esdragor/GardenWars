using System;
using System.Collections.Generic;
using Entities.Champion;
using Photon.Pun;
using UnityEngine;

namespace GameStates
{
    public partial class GameStateMachine
    {
        public void RequestAddPlayer()
        {
            if (isMaster)
            {
                AddPlayerRPC(PhotonNetwork.LocalPlayer.ActorNumber, GameSettingsManager.playerName);
                return;
            }

            photonView.RPC("AddPlayerRPC", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber,
                GameSettingsManager.playerName);
        }

        [PunRPC]
        private void AddPlayerRPC(int actorNumber, string playerName)
        {
            if (isOffline)
            {
                SyncAddPlayerRPC(actorNumber, playerName);
                return;
            }

            photonView.RPC("SyncAddPlayerRPC", RpcTarget.All, actorNumber, playerName);
        }

        [PunRPC]
        private void SyncAddPlayerRPC(int actorNumber, string playerName)
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
                    name = playerName,
                    
                    emoteByteBuffer = new List<byte>[6],
                    emotesTextures = new Texture2D[6]
                };
                
                for (var index = 0; index < 6; index++)
                {
                    playerData.emoteByteBuffer[index] = new List<byte>();
                }
                
                playerDataDict.Add(actorNumber, playerData);
                allPlayersIDs.Add(actorNumber);
                
                OnPlayerAdded?.Invoke();
            }
        }
        
        public static event Action OnPlayerAdded;

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
                name = $"Offline Player",
                
                emoteByteBuffer = new List<byte>[6],
                emotesTextures = new Texture2D[6]
            };

            for (var index = 0; index < 6; index++)
            {
                playerData.emoteByteBuffer[index] = new List<byte>();
            }

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
            OnPlayerAdded?.Invoke();
        }
    }
}