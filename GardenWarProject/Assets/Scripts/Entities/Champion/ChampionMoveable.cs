using System;
using Entities.Capacities;
using FMODUnity;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;

namespace Entities.Champion
{
    [RequireComponent(typeof(NavMeshAgent))]
    public partial class Champion : IMoveable
    {
        [Header("Movable")] public float baseMoveSpeed;
        public float bonusMoveSpeed;
        public float moveSpeed => baseMoveSpeed + bonusMoveSpeed;
        public bool canMove;

        [HideInInspector] public NavMeshAgent agent;
        public float currentVelocity => agent.velocity.magnitude;
        public bool isMoving = false;
        
                
        [Header("Sound")]
        [SerializeField] private FMODUnity.StudioEventEmitter fmodEmitter;
        public string[] SFXMoves = new string[2];
        public string CurrentSFXMove;

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
            return baseMoveSpeed;
        }

        public float GetCurrentMoveSpeed()
        {
            return moveSpeed;
        }

        public void RequestSetCanMove(bool value)
        {
            if (isMaster)
            {
                SetCanMoveRPC(value);
                return;
            }

            photonView.RPC("SetCanMoveRPC", RpcTarget.MasterClient, value);
        }

        [PunRPC]
        public void SetCanMoveRPC(bool value)
        {
            canMove = value;
            OnSetCanMove?.Invoke(value);
            if (isOffline)
            {
                SyncSetCanMoveRPC(value);
                return;
            }

            photonView.RPC("SyncSetCanMoveRPC", RpcTarget.All, value);
        }

        [PunRPC]
        public void SyncSetCanMoveRPC(bool value)
        {
            canMove = value;
            OnSetCanMoveFeedback?.Invoke(value);
        }

        public event GlobalDelegates.BoolDelegate OnSetCanMove;
        public event GlobalDelegates.BoolDelegate OnSetCanMoveFeedback;

        public void RequestSetReferenceMoveSpeed(float value)
        {
        }

        [PunRPC]
        public void SyncSetReferenceMoveSpeedRPC(float value)
        {
        }

        [PunRPC]
        public void SetReferenceMoveSpeedRPC(float value)
        {
        }

        public event GlobalDelegates.FloatDelegate OnSetReferenceMoveSpeed;
        public event GlobalDelegates.FloatDelegate OnSetReferenceMoveSpeedFeedback;

        public void RequestIncreaseReferenceMoveSpeed(float amount)
        {
        }

        [PunRPC]
        public void SyncIncreaseReferenceMoveSpeedRPC(float amount)
        {
        }

        [PunRPC]
        public void IncreaseReferenceMoveSpeedRPC(float amount)
        {
        }

        public event GlobalDelegates.FloatDelegate OnIncreaseReferenceMoveSpeed;
        public event GlobalDelegates.FloatDelegate OnIncreaseReferenceMoveSpeedFeedback;

        public void RequestDecreaseReferenceMoveSpeed(float amount)
        {
        }

        [PunRPC]
        public void SyncDecreaseReferenceMoveSpeedRPC(float amount)
        {
        }

        [PunRPC]
        public void DecreaseReferenceMoveSpeedRPC(float amount)
        {
        }

        public event GlobalDelegates.FloatDelegate OnDecreaseReferenceMoveSpeed;
        public event GlobalDelegates.FloatDelegate OnDecreaseReferenceMoveSpeedFeedback;

        public void RequestSetCurrentMoveSpeed(float value)
        {
        }

        [PunRPC]
        public void SyncSetCurrentMoveSpeedRPC(float value)
        {
        }

        [PunRPC]
        public void SetCurrentMoveSpeedRPC(float value)
        {
        }

        public event GlobalDelegates.FloatDelegate OnSetCurrentMoveSpeed;
        public event GlobalDelegates.FloatDelegate OnSetCurrentMoveSpeedFeedback;

        public void RequestIncreaseCurrentMoveSpeed(float amount)
        {
            if (isMaster)
            {
                IncreaseCurrentMoveSpeedRPC(amount);
                return;
            }

            photonView.RPC("IncreaseCurrentMoveSpeedRPC", RpcTarget.MasterClient, amount);
        }

        [PunRPC]
        public void IncreaseCurrentMoveSpeedRPC(float amount)
        {
            bonusMoveSpeed += amount;
            OnIncreaseCurrentMoveSpeed?.Invoke(amount);
            if (isOffline)
            {
                SyncIncreaseCurrentMoveSpeedRPC(bonusMoveSpeed);
                return;
            }

            photonView.RPC("SyncIncreaseCurrentMoveSpeedRPC", RpcTarget.All, bonusMoveSpeed);
        }

        [PunRPC]
        public void SyncIncreaseCurrentMoveSpeedRPC(float amount)
        {
            var gainedMs = amount - bonusMoveSpeed;
            bonusMoveSpeed = amount;
            agent.speed = moveSpeed;
            OnIncreaseCurrentMoveSpeedFeedback?.Invoke(gainedMs);
        }

        public event GlobalDelegates.FloatDelegate OnIncreaseCurrentMoveSpeed;
        public event GlobalDelegates.FloatDelegate OnIncreaseCurrentMoveSpeedFeedback;

        public void RequestDecreaseCurrentMoveSpeed(float amount)
        {
            if (isMaster)
            {
                DecreaseCurrentMoveSpeedRPC(amount);
                return;
            }

            photonView.RPC("DecreaseCurrentMoveSpeedRPC", RpcTarget.MasterClient, amount);
        }

        [PunRPC]
        public void DecreaseCurrentMoveSpeedRPC(float amount)
        {
            bonusMoveSpeed -= amount;
            OnDecreaseCurrentMoveSpeed?.Invoke(amount);
            if (isOffline)
            {
                SyncDecreaseCurrentMoveSpeedRPC(bonusMoveSpeed);
                return;
            }

            photonView.RPC("SyncDecreaseCurrentMoveSpeedRPC", RpcTarget.All, bonusMoveSpeed);
        }

        [PunRPC]
        public void SyncDecreaseCurrentMoveSpeedRPC(float amount)
        {
            var lostMs = amount - bonusMoveSpeed;
            bonusMoveSpeed = amount;
            agent.speed = moveSpeed;
            OnDecreaseCurrentMoveSpeedFeedback?.Invoke(lostMs);
        }

        public event GlobalDelegates.FloatDelegate OnDecreaseCurrentMoveSpeed;
        public event GlobalDelegates.FloatDelegate OnDecreaseCurrentMoveSpeedFeedback;

        public void MoveToPosition(Vector3 position)
        {
            if (!canMove) return;

            //CancelMoveToTarget();

            position = ActiveCapacity.GetClosestValidPoint(position);

            if (agent.isOnNavMesh) agent.SetDestination(position);
            OnMove?.Invoke(position);

            position.y = rotateParent.position.y;
            rotateParent.LookAt(position);
        }

        public event GlobalDelegates.Vector3Delegate OnMove;
        public event GlobalDelegates.Vector3Delegate OnMoveFeedback;

        private void ChangingStateMoving()
        {
            if (!photonView.IsMine) return;
            if (currentVelocity > 0.1f && !isMoving)
            {
                // EventReference newRef = new EventReference();
                // newRef.Path = "event:/" + CurrentSFXMove;
                // fmodEmitter.EventReference = newRef;
                //
                // fmodEmitter.Play();
                isMoving = true;
                //FMODUnity.RuntimeManager.PlayOneShot("event:/" + CurrentSFXMove, transform.position);
                foreach (var animator in animators)
                {
                    animator.SetBool("IsMoving", isMoving);
                }
                if(!isOffline) photonView.RPC("SyncIsMovingRPC", RpcTarget.All, entityIndex, isMoving);
            }
            else if (currentVelocity < 0.1f && isMoving)
            {
                fmodEmitter.Stop();
                
                isMoving = false;
                foreach (var animator in animators)
                {
                    animator.SetBool("IsMoving", isMoving);
                }
                if(!isOffline) photonView.RPC("SyncIsMovingRPC", RpcTarget.All, entityIndex, isMoving);
            }
        }

        private void ModifyMoving(bool move)
        {
            foreach (var animator in animators)
            {
                animator.SetBool("IsMoving", move);
            }
            if(isMaster) OnMoving?.Invoke(move);
            OnMovingFeedback?.Invoke(move);
        }

        public event Action<bool> OnMoving;
        public event Action<bool> OnMovingFeedback;

        [PunRPC]
        private void SyncIsMovingRPC(int actorNumber, bool isMoving)
        {
            (EntityCollectionManager.GetEntityByIndex(actorNumber) as Champion)?.ModifyMoving(isMoving);
        }

        private void TryMoveToTarget()
        {
            MoveToTargetAction?.Invoke();
        }

        private void MoveToTarget(Vector3 targetPos, float rangeToAction, Action action)
        {
            var distanceToTarget = Vector3.Distance(transform.position, targetPos);
            if (distanceToTarget <= rangeToAction)
            {
                if (agent.isOnNavMesh) agent.ResetPath();
                action.Invoke();
                return;
            }

            if (agent.isOnNavMesh) agent.SetDestination(targetPos);

            OnMove?.Invoke(targetPos);

            targetPos.y = rotateParent.position.y;
            rotateParent.LookAt(targetPos);
        }

        public void StartMoveToTarget(Entity targetEntity, float rangeToAction, Action action)
        {
            if (!targetEntity) return;
            MoveToTargetAction += TargetAction;

            void TargetAction()
            {
                MoveToTarget(targetEntity.position, rangeToAction, action);
            }
        }

        public void CancelMoveToTarget()
        {
            MoveToTargetAction = null;
        }

        public event Action MoveToTargetAction;
    }
}