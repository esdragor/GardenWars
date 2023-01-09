using System.Collections.Generic;
using System.Linq;
using Entities.Capacities;
using UnityEngine;
using Photon.Pun;

namespace Entities.Champion
{
    public partial class Champion : ICastable
    {
        public byte[] abilitiesIndexes = new byte[3];
        private readonly Dictionary<byte, CastingAbility> capacityDict = new Dictionary<byte, CastingAbility>();

        public int targetedEntities;
        public Vector3 targetedPositions;

        private class CastingAbility
        {
            public bool isCasting = false;
            public ActiveCapacity capacity;
        }

        public bool canCast;
        
        public bool CanCast()
        {
            return canCast;
        }
        
        public void RequestSetCanCast(bool value)
        {
            if (isMaster)
            {
                SetCanCastRPC(value);
                return;
            }
            photonView.RPC("SetCanCastRPC", RpcTarget.MasterClient, value);
        }
        
        [PunRPC]
        public void SetCanCastRPC(bool value)
        {
            canCast = value;
            OnSetCanCast?.Invoke(value);
            if (isOffline)
            {
                SyncSetCanCastRPC(value);
                return;
            }
            photonView.RPC("SyncSetCanCastRPC", RpcTarget.All, value);
        }

        [PunRPC]
        public void SyncSetCanCastRPC(bool value)
        {
            canCast = value;
            OnSetCanCastFeedback?.Invoke(value);
        }
        
        public event GlobalDelegates.BoolDelegate OnSetCanCast;
        public event GlobalDelegates.BoolDelegate OnSetCanCastFeedback;

        public void RequestCast(byte capacityIndex, int targetedEntities, Vector3 targetedPositions) { }
        
        public void CastRPC(byte capacityIndex, int targetedEntities, Vector3 targetedPositions) { }
        
        public void SyncCastRPC(byte capacityIndex, int targetedEntities, Vector3 targetedPositions) { }
        
        public event GlobalDelegates.ByteIntArrayVector3ArrayDelegate OnCast;
        public event GlobalDelegates.ByteIntArrayVector3ArrayDelegate OnCastFeedback;
        
        
        public void RequestOnCastCapacity(byte capacityIndex)
        {
            if(isMaster)
            {
                OnCastCapacityRPC(capacityIndex,targetedEntities,targetedPositions);
                return;
            }
            photonView.RPC("OnCastCapacityRPC",RpcTarget.MasterClient,capacityIndex,targetedEntities,targetedPositions);
        }

        [PunRPC]
        private void OnCastCapacityRPC(byte capacityIndex, int newTargetedEntities, Vector3 newTargetedPositions)
        {
            if(!canCast) return;
            if (isOffline)
            {
                SyncOnCastCapacityRPC(capacityIndex, newTargetedEntities, newTargetedPositions);
                return;
            }
            photonView.RPC("SyncOnCastCapacityRPC",RpcTarget.All,capacityIndex,newTargetedEntities,newTargetedPositions);
        }
        
        [PunRPC]
        private void SyncOnCastCapacityRPC(byte capacityIndex, int newTargetedEntities, Vector3 newTargetedPositions)
        {
            targetedEntities = newTargetedEntities;
            targetedPositions = newTargetedPositions;
            if (capacityDict.ContainsKey(capacityIndex))
            {
                capacityDict[capacityIndex].isCasting = true;
            }
            else
            {
                var newCapacity = new CastingAbility
                {
                    isCasting = true,
                    capacity = CapacitySOCollectionManager.CreateActiveCapacity(capacityIndex,this)
                };
                capacityDict.Add(capacityIndex,newCapacity);
            }
            capacityDict[capacityIndex].capacity.OnPress(targetedEntities,targetedPositions);
        }

        private void CastHeldCapacities()
        {
            if(!canCast) return;
            foreach (var ability in capacityDict.Values.Where(ability => ability.isCasting))
            {
                ability.capacity.OnHold(targetedEntities,targetedPositions);
            }
        }

        public void RequestOnReleaseCapacity(byte capacityIndex)
        {
            if(isMaster)
            {
                OnReleaseCapacityRPC(capacityIndex,targetedEntities,targetedPositions);
                return;
            }
            photonView.RPC("OnReleaseCapacityRPC",RpcTarget.MasterClient,capacityIndex,targetedEntities,targetedPositions);
        }

        [PunRPC]
        private void OnReleaseCapacityRPC(byte capacityIndex, int newTargetedEntities, Vector3 newTargetedPositions)
        {
            if(!canCast) return;
            if (isOffline)
            {
                SyncOnReleaseCapacityRPC(capacityIndex, newTargetedEntities, newTargetedPositions);
                return;
            }
            photonView.RPC("SyncOnReleaseCapacityRPC",RpcTarget.All,capacityIndex,newTargetedEntities,newTargetedPositions);
        }
        
        [PunRPC]
        private void SyncOnReleaseCapacityRPC(byte capacityIndex, int newTargetedEntities, Vector3 newTargetedPositions)
        {
            targetedEntities = newTargetedEntities;
            targetedPositions = newTargetedPositions;
            if (capacityDict.ContainsKey(capacityIndex))
            {
                capacityDict[capacityIndex].isCasting = false;
            }
            else
            {
                var newCapacity = new CastingAbility
                {
                    isCasting = false,
                    capacity = CapacitySOCollectionManager.CreateActiveCapacity(capacityIndex,this)
                };
                capacityDict.Add(capacityIndex,newCapacity);
            }
            
            capacityDict[capacityIndex].capacity.OnRelease(targetedEntities,targetedPositions);
            if(isMaster) OnCast?.Invoke(capacityIndex,targetedEntities,targetedPositions);
            
            OnCastFeedback?.Invoke(capacityIndex,targetedEntities,targetedPositions);
        }
    }
}