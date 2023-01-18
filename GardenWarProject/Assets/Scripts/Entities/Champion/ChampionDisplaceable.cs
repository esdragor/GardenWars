using System;
using Entities.Capacities;
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
            if(!canBeDisplaced) return;
            
            if (isOffline)
            {
                SyncDisplaceRPC(destination,time);
                return;
            }
            photonView.RPC("SyncDisplaceRPC", RpcTarget.All, destination,time);
        }

        [PunRPC]
        public void SyncDisplaceRPC(Vector3 destination, float time)
        {
            agent.enabled = false;
            canMove = false;
            canBeDisplaced = false;

            var startPos = transform.position;
            var endPos = ActiveCapacity.GetClosestValidPoint(destination);
            var timeToDestination = time;
            float timer = 0;
            if (time == 0)
            {
                Arrived();
                return;
            }
            
            gsm.OnUpdateFeedback += MoveToDestination;
            
            void MoveToDestination()
            {
                timer += Time.deltaTime;

                transform.position = Vector3.Lerp(startPos, endPos, timer / timeToDestination);
                
                if (timer < timeToDestination) return;
                
                gsm.OnUpdateFeedback -= MoveToDestination;
                
                Arrived();
            }

            void Arrived()
            {
                canMove = true;
                canBeDisplaced = true;
                
                if(isMaster) OnDisplace?.Invoke(destination,time);
                OnDisplaceFeedback?.Invoke(destination,time);

                if (!isPlayerChampion) return;
                
                agent.enabled = true;

                agent.Warp(endPos);
            }
        }

        public event Action<Vector3,float> OnDisplace;
        public event Action<Vector3,float> OnDisplaceFeedback;
    }
}