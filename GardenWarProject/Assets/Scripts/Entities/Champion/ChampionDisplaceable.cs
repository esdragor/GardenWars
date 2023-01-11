using System;
using Photon.Pun;
using UnityEngine;

namespace Entities.Champion
{
    public partial class Champion : IDisplaceable
    {
        public bool canBeDisplaced;

        public bool CanBeDisplaced()
        {
            return canBeDisplaced;
        }

        public void RequestSetCanBeDisplaced(bool value)
        {
            if (isMaster)
            {
                SetCanBeDisplacedRPC(value);
                return;
            }
            photonView.RPC("SetCanBeDisplacedRPC", RpcTarget.MasterClient, value);
        }
        
        [PunRPC]
        public void SetCanBeDisplacedRPC(bool value)
        {
            canBeDisplaced = value;
            OnSetCanBeDisplaced?.Invoke(value);
            if (isOffline)
            {
                SyncSetCanBeDisplacedRPC(value);
                return;
            }
            photonView.RPC("SyncSetCanBeDisplacedRPC", RpcTarget.All, value);
        }

        [PunRPC]
        public void SyncSetCanBeDisplacedRPC(bool value)
        {
            canBeDisplaced = value;
            OnSetCanBeDisplacedFeedback?.Invoke(value);
        }

        public event GlobalDelegates.BoolDelegate OnSetCanBeDisplaced;
        public event GlobalDelegates.BoolDelegate OnSetCanBeDisplacedFeedback;

        public void RequestDisplace(Vector3 destination, float time)
        {
            if (isMaster)
            {
                DisplaceRPC(destination,time);
                return;
            }
            photonView.RPC("DisplaceRPC", RpcTarget.MasterClient, destination,time);
        }
        
        [PunRPC]
        public void DisplaceRPC(Vector3 destination, float time)
        {
            
        }

        [PunRPC]
        public void SyncDisplaceRPC(Vector3 destination, float time)
        {
            
        }

        public event Action<Vector3,float> OnDisplace;
        public event Action<Vector3,float> OnDisplaceFeedback;
    }
}