using System;
using UnityEngine;

namespace Entities
{
    public interface IDisplaceable
    {
        /// <returns>true if the entity can be displaced, false if not</returns>
        public bool CanBeDisplaced();
        /// <summary>
        /// Sends an RPC to the master to set if the entity can be displaced.
        /// </summary>
        public void RequestSetCanBeDisplaced(bool value);
        /// <summary>
        /// Sends an RPC to all clients to set if the entity can be displaced.
        /// </summary>
        public void SyncSetCanBeDisplacedRPC(bool value);
        /// <summary>
        /// Sets if the entity can be displaced.
        /// </summary>
        public void SetCanBeDisplacedRPC(bool value);

        public event GlobalDelegates.BoolDelegate OnSetCanBeDisplaced;
        public event GlobalDelegates.BoolDelegate OnSetCanBeDisplacedFeedback;
        
        /// <summary>
        /// Sends an RPC to the master to displace the entity.
        /// </summary>
        public void RequestDisplace(Vector3 destination, float time);
        /// <summary>
        /// Sends an RPC to all clients to displace the entity.
        /// </summary>
        public void SyncDisplaceRPC(Vector3 destination, float time);
        /// <summary>
        /// Displaces the entity.
        /// </summary>
        public void DisplaceRPC(Vector3 destination, float time);

        public event Action<Vector3,float> OnDisplace;
        public event Action<Vector3,float> OnDisplaceFeedback;
    }
}