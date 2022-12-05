using System;
using GameStates;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

namespace Entities.Capacities
{
    public abstract class ActiveCapacity
    {
        private bool isMaster => !PhotonNetwork.IsConnected || PhotonNetwork.IsMasterClient;
        
        public byte indexOfSOInCollection;
        
        public Entity caster;
        protected Vector3 casterPos => caster.transform.position;

        public double baseCooldown => AssociatedActiveCapacitySO().cooldown;
        public bool isOnCooldown;
        private double cooldownTimer;
        
        protected GameStateMachine gsm => GameStateMachine.Instance;

        protected ActiveCapacitySO AssociatedActiveCapacitySO()
        {
            return CapacitySOCollectionManager.GetActiveCapacitySOByIndex(indexOfSOInCollection);
        }

        public bool CanCast(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            if (isOnCooldown)
            {
                Debug.Log("On Cooldown");
                return false;
            }
            var so = AssociatedActiveCapacitySO();
            switch (so.shootType)
            {
                case Enums.CapacityShootType.Skillshot:
                    break;
                case Enums.CapacityShootType.TargetPosition:
                    if (Vector3.Distance(casterPos, targetPositions[0]) > so.maxRange)
                    {
                        Debug.Log("Out of range");
                        return false;
                    }
                    break;
                case Enums.CapacityShootType.TargetEntity:
                    if (Vector3.Distance(casterPos,
                            EntityCollectionManager.GetEntityByIndex(targetsEntityIndexes[0]).transform.position) >
                        so.maxRange)
                    {
                        Debug.Log("Out of range");
                        return false;
                    }
                    break;
            }

            return AdditionalCastConditions(casterIndex, targetsEntityIndexes, targetPositions);
        }

        protected abstract bool AdditionalCastConditions(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions);
        
        public void OnPress(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            if(isMaster) Press(casterIndex,targetsEntityIndexes,targetPositions);
            PressFeedback(casterIndex,targetsEntityIndexes,targetPositions);
        }
        protected abstract void Press(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions);
        protected abstract void PressFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions);

        public void OnHold(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            if(isMaster) Hold(casterIndex,targetsEntityIndexes,targetPositions);
            HoldFeedback(casterIndex,targetsEntityIndexes,targetPositions);
        }

        protected abstract void Hold(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions);
        protected abstract void HoldFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions);
        public void OnRelease(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            if(!CanCast(casterIndex,targetsEntityIndexes,targetPositions)) return;
            if(isMaster) Release(casterIndex,targetsEntityIndexes,targetPositions);
            ReleaseFeedback(casterIndex,targetsEntityIndexes,targetPositions);
            if(baseCooldown > 0) EnterCooldown(baseCooldown);
        }

        protected abstract void Release(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions);
        protected abstract void ReleaseFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions);

        private void EnterCooldown(double timeOnCooldown)
        {
            isOnCooldown = true;
            
            cooldownTimer = timeOnCooldown;
            
            gsm.OnTickFeedback += DecreaseCooldown;

            void DecreaseCooldown()
            {
                cooldownTimer -= 1/gsm.tickRate;
                
                if(cooldownTimer > 0) return;
                isOnCooldown = false;
                gsm.OnTickFeedback -= DecreaseCooldown;
            }
        }

        public bool isInRange(int casterIndex, Vector3 position)
        {
            float distance = Vector3.Distance(EntityCollectionManager.GetEntityByIndex(casterIndex).transform.position, position);
            if ( distance > AssociatedActiveCapacitySO().maxRange) return false;
            
            return true;
        }

        public static Vector3 GetClosestValidPoint(Vector3 position)
        {
            if (NavMesh.SamplePosition(position, out var hit, 5, NavMesh.AllAreas))
            {
                return hit.position;
            }
            position.y = 0;
            return position;
        }
    }
}

