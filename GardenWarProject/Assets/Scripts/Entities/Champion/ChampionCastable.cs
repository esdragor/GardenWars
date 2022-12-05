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

        public int[] targetedEntities;
        public Vector3[] targetedPositions;

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
            photonView.RPC("CastRPC",RpcTarget.MasterClient,value);
        }
        
        [PunRPC]
        public void SetCanCastRPC(bool value)
        {
            canCast = value;
            OnSetCanCast?.Invoke(value);
            photonView.RPC("SyncCastRPC",RpcTarget.All,canCast);
        }

        [PunRPC]
        public void SyncSetCanCastRPC(bool value)
        {
            canCast = value;
            OnSetCanCastFeedback?.Invoke(value);
        }
        
        public event GlobalDelegates.BoolDelegate OnSetCanCast;
        public event GlobalDelegates.BoolDelegate OnSetCanCastFeedback;

        public void RequestCast(byte capacityIndex, int[] targetedEntities, Vector3[] targetedPositions)
        {
            photonView.RPC("CastRPC",RpcTarget.MasterClient,capacityIndex,targetedEntities,targetedPositions);
        }
        
        [PunRPC]
        public void CastRPC(byte capacityIndex, int[] targetedEntities, Vector3[] targetedPositions)
        {
            var activeCapacity = CapacitySOCollectionManager.CreateActiveCapacity(capacityIndex,this);
            
            if (!activeCapacity.CanCast(targetedEntities, targetedPositions)) return;
            
            OnCast?.Invoke(capacityIndex,targetedEntities,targetedPositions);
            photonView.RPC("SyncCastRPC",RpcTarget.All,capacityIndex,targetedEntities,targetedPositions);

        }

        [PunRPC]
        public void SyncCastRPC(byte capacityIndex, int[] targetedEntities, Vector3[] targetedPositions)
        {
            var activeCapacity = CapacitySOCollectionManager.CreateActiveCapacity(capacityIndex,this);
            activeCapacity.OnRelease(targetedEntities,targetedPositions);
            OnCastFeedback?.Invoke(capacityIndex,targetedEntities,targetedPositions,activeCapacity);
        }
        
        public event GlobalDelegates.ByteIntArrayVector3ArrayDelegate OnCast;
        public event GlobalDelegates.ByteIntArrayVector3ArrayCapacityDelegate OnCastFeedback;
        
        

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
        private void OnCastCapacityRPC(byte capacityIndex, int[] newTargetedEntities, Vector3[] newTargetedPositions)
        {
            if (isOffline)
            {
                SyncOnCastCapacityRPC(capacityIndex, newTargetedEntities, newTargetedPositions);
                return;
            }
            photonView.RPC("SyncOnCastCapacityRPC",RpcTarget.All,capacityIndex,newTargetedEntities,newTargetedPositions);
        }
        
        [PunRPC]
        private void SyncOnCastCapacityRPC(byte capacityIndex, int[] newTargetedEntities, Vector3[] newTargetedPositions)
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
        private void OnReleaseCapacityRPC(byte capacityIndex, int[] newTargetedEntities, Vector3[] newTargetedPositions)
        {
            if (isOffline)
            {
                SyncOnReleaseCapacityRPC(capacityIndex, newTargetedEntities, newTargetedPositions);
                return;
            }
            photonView.RPC("SyncOnReleaseCapacityRPC",RpcTarget.All,capacityIndex,newTargetedEntities,newTargetedPositions);
        }
        
        [PunRPC]
        private void SyncOnReleaseCapacityRPC(byte capacityIndex, int[] newTargetedEntities, Vector3[] newTargetedPositions)
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
        }
    }
}