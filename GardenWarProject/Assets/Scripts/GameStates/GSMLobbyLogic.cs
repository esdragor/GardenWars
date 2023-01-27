using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

namespace GameStates
{
    public partial class GameStateMachine
    {
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

            playerDataDict[actorNumber].team = (Enums.Team) team;
            OnDataDictUpdated?.Invoke(actorNumber, playerDataDict[actorNumber]);
            OnPlayerAdded?.Invoke();
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

            playerDataDict[actorNumber].role = (Enums.ChampionRole) role;

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
                    (byte) values.team,
                    (byte) values.role, values.championSOIndex);
            }
        }

        [PunRPC]
        private void SyncDataDictionaryRPC(int actorNumber, string playerName, bool isReady, byte team, byte role,
            byte championSOindex)
        {
            var data = new PlayerData
            {
                name = playerName,
                isReady = isReady,
                team = (Enums.Team) team,
                role = (Enums.ChampionRole) role,
                championSOIndex = championSOindex,

                emoteByteBuffer = new List<byte>[6],
                emotesTextures = new Texture2D[6]
            };

            for (var index = 0; index < 6; index++)
            {
                data.emoteByteBuffer[index] = new List<byte>();
            }

            if (!playerDataDict.ContainsKey(actorNumber)) playerDataDict.Add(actorNumber, data);
            else playerDataDict[actorNumber] = data;
            if (!allPlayersIDs.Contains(actorNumber)) allPlayersIDs.Add(actorNumber);
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

            var neutralPlayersIds =
                playerDataDict.Where(kvp => kvp.Value.team == Enums.Team.Neutral).Select(kvp => kvp.Key);
            foreach (var key in neutralPlayersIds)
            {
                playerDataDict.Remove(key);
                allPlayersIDs.Remove(key);
            }

            if (isInDebugMode)
            {
                foreach (var data in playerDataDict.Values)
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
            foreach (var data in playerDataDict.Values)
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
            foreach (var data in playerDataDict.Values)
            {
                data.isReady = false;
            }
        }

        #endregion
    }
}