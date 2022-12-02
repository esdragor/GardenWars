using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;

namespace Entities.Champion
{
    [RequireComponent(typeof(NavMeshAgent))]
    public partial class Champion : IMoveable
    {
        public float referenceMoveSpeed;
        public float currentMoveSpeed;
        public float currentRotateSpeed;
        public bool canMove;
        private Vector3 moveDirection;

        // === League Of Legends
        private int mouseTargetIndex;
        private bool isFollowing;
        private Entity entityFollow;
        private float attackRange;

        private Vector3 movePosition;
        //NavMesh

        private NavMeshAgent agent;

        private Vector3 rotateDirection;

        public bool CanMove()
        {
            return canMove;
        }

        void SetupNavMesh()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.SetDestination(transform.position);
            if (!photonView.IsMine) agent.enabled = false;
            //NavMeshBuilder.ClearAllNavMeshes();
            //NavMeshBuilder.BuildNavMesh();
        }

        public float GetReferenceMoveSpeed()
        {
            return referenceMoveSpeed;
        }

        public float GetCurrentMoveSpeed()
        {
            return currentMoveSpeed;
        }

        public void RequestSetCanMove(bool value) { }

        [PunRPC]
        public void SyncSetCanMoveRPC(bool value) { }

        [PunRPC]
        public void SetCanMoveRPC(bool value) { }

        public event GlobalDelegates.BoolDelegate OnSetCanMove;
        public event GlobalDelegates.BoolDelegate OnSetCanMoveFeedback;

        public void RequestSetReferenceMoveSpeed(float value) { }

        [PunRPC]
        public void SyncSetReferenceMoveSpeedRPC(float value) { }

        [PunRPC]
        public void SetReferenceMoveSpeedRPC(float value) { }

        public event GlobalDelegates.FloatDelegate OnSetReferenceMoveSpeed;
        public event GlobalDelegates.FloatDelegate OnSetReferenceMoveSpeedFeedback;

        public void RequestIncreaseReferenceMoveSpeed(float amount) { }

        [PunRPC]
        public void SyncIncreaseReferenceMoveSpeedRPC(float amount) { }

        [PunRPC]
        public void IncreaseReferenceMoveSpeedRPC(float amount) { }

        public event GlobalDelegates.FloatDelegate OnIncreaseReferenceMoveSpeed;
        public event GlobalDelegates.FloatDelegate OnIncreaseReferenceMoveSpeedFeedback;

        public void RequestDecreaseReferenceMoveSpeed(float amount) { }

        [PunRPC]
        public void SyncDecreaseReferenceMoveSpeedRPC(float amount) { }

        [PunRPC]
        public void DecreaseReferenceMoveSpeedRPC(float amount) { }

        public event GlobalDelegates.FloatDelegate OnDecreaseReferenceMoveSpeed;
        public event GlobalDelegates.FloatDelegate OnDecreaseReferenceMoveSpeedFeedback;

        public void RequestSetCurrentMoveSpeed(float value) { }

        [PunRPC]
        public void SyncSetCurrentMoveSpeedRPC(float value) { }

        [PunRPC]
        public void SetCurrentMoveSpeedRPC(float value) { }

        public event GlobalDelegates.FloatDelegate OnSetCurrentMoveSpeed;
        public event GlobalDelegates.FloatDelegate OnSetCurrentMoveSpeedFeedback;

        public void RequestIncreaseCurrentMoveSpeed(float amount) { }

        [PunRPC]
        public void SyncIncreaseCurrentMoveSpeedRPC(float amount) { }

        [PunRPC]
        public void IncreaseCurrentMoveSpeedRPC(float amount) { }

        public event GlobalDelegates.FloatDelegate OnIncreaseCurrentMoveSpeed;
        public event GlobalDelegates.FloatDelegate OnIncreaseCurrentMoveSpeedFeedback;

        public void RequestDecreaseCurrentMoveSpeed(float amount) { }

        [PunRPC]
        public void SyncDecreaseCurrentMoveSpeedRPC(float amount) { }

        [PunRPC]
        public void DecreaseCurrentMoveSpeedRPC(float amount) { }

        public event GlobalDelegates.FloatDelegate OnDecreaseCurrentMoveSpeed;
        public event GlobalDelegates.FloatDelegate OnDecreaseCurrentMoveSpeedFeedback;

        #region League Of Legends

        public void MoveToPosition(Vector3 position)
        {
            movePosition = position;
            movePosition.y = transform.position.y;
            agent.SetDestination(position);
        }

        private void SendFollowEntity(int entityIndex, float capacityDistance)
        {
            photonView.RPC("StartFollowEntityRPC", RpcTarget.All, entityIndex, capacityDistance);
        }

        [PunRPC]
        public void StartFollowEntityRPC(int entityIndex, float capacityDistance)
        {
            Debug.Log("Start Follow Entity");
            if (!photonView.IsMine) return;
            isFollowing = true;
            entityFollow = EntityCollectionManager.GetEntityByIndex(entityIndex);
            attackRange = capacityDistance;
        }

        private void FollowEntity()
        {
            if (!isFollowing) return;
            agent.SetDestination(entityFollow.transform.position);
            if (lastCapacity.isInRange(entityIndex, entityFollow.transform.position))
            {
                agent.SetDestination(transform.position);
                isFollowing = false;
                RequestAttack(lastCapacityIndex, lastTargetedEntities, lastTargetedPositions);
            }
        }

        private void CheckMoveDistance()
        {
            if (agent == null) return;
          
            if (Vector3.Distance(transform.position, movePosition) < 0.5 )
            {
                agent.SetDestination(transform.position);
            }
            else if(agent.velocity != Vector3.zero)
            {
                rotateParent.forward = agent.velocity.normalized;
            }
        }

        #endregion

        public event GlobalDelegates.Vector3Delegate OnMove;
        public event GlobalDelegates.Vector3Delegate OnMoveFeedback;
    }
}