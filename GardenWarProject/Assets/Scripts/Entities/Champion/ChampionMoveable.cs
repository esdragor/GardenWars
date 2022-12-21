using System;
using Entities.Capacities;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Entities.Champion
{
    [RequireComponent(typeof(NavMeshAgent))]
    public partial class Champion : IMoveable
    {
        public float referenceMoveSpeed;
        public float currentMoveSpeed;
        public bool canMove;
        
        [HideInInspector] public NavMeshAgent agent;
        
        public bool CanMove()
        {
            return canMove;
        }

        public void SetupNavMesh()
        {
            agent = GetComponent<NavMeshAgent>();
            if (!photonView.IsMine) agent.enabled = false;
            agent.Warp(transform.position);
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

        public void MoveToPosition(Vector3 position)
        {
            if(!canMove) return;
            CancelMoveToTarget();
            position = ActiveCapacity.GetClosestValidPoint(position);
            agent.SetDestination(position);
        }
        public event GlobalDelegates.Vector3Delegate OnMove;
        public event GlobalDelegates.Vector3Delegate OnMoveFeedback;

        private void TryMoveToTarget()
        {
            MoveToTargetAction?.Invoke();
        }
        
        public void StartMoveToTarget(Vector3 targetPos,float rangeToAction,Action action)
        {
            MoveToTargetAction += () => MoveToTarget(targetPos, rangeToAction, action);
        }

        private void MoveToTarget(Vector3 targetPos,float rangeToAction,Action action)
        {
            var distanceToTarget = Vector3.Distance(transform.position, targetPos);
            if (distanceToTarget <= rangeToAction)
            {
                agent.ResetPath();
                action.Invoke();
                return;
            }
            agent.SetDestination(targetPos);
        }

        public void StartMoveToTarget(Entity targetEntity, float rangeToAction, Action action)
        {
            if (!targetEntity) return;
            MoveToTargetAction += () => MoveToTarget(targetEntity.position, rangeToAction, action);
        }
        
        private void MoveToTarget(Entity targetEntity, float rangeToAction, Action action)
        {
            MoveToTarget(targetEntity.position,rangeToAction,action);
        }

        public void CancelMoveToTarget()
        {
            MoveToTargetAction = null;
        }

        public event Action MoveToTargetAction;
    }
}