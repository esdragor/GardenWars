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
        public Champion.Champion champion => ((Champion.Champion) caster);
        
        public bool isBasicAttack => champion.attackAbilityIndex == indexOfSOInCollection;
        protected Vector3 casterPos => caster.transform.position;

        public double baseCooldown => isBasicAttack ? champion.attackSpeed : AssociatedActiveCapacitySO().cooldown;
        public bool isOnCooldown;
        private double cooldownTimer;
        
        protected GameStateMachine gsm => GameStateMachine.Instance;

        protected ActiveCapacitySO AssociatedActiveCapacitySO()
        {
            return CapacitySOCollectionManager.GetActiveCapacitySOByIndex(indexOfSOInCollection);
        }

        public bool CanCast(int targetsEntityIndex, Vector3 targetPosition)
        {
            Debug.Log($"CanCast : {targetsEntityIndex}, {targetPosition}");
            if (isOnCooldown)
            {
                Debug.Log("On Cooldown");
                return false;
            }
            var so = AssociatedActiveCapacitySO();
            var maxRange = isBasicAttack ? champion.attackRange : so.maxRange;
            switch (so.shootType)
            {
                case Enums.CapacityShootType.Skillshot:
                    break;
                case Enums.CapacityShootType.TargetPosition:
                    if (Vector3.Distance(casterPos, targetPosition) > maxRange)
                    {
                        Debug.Log($"Out of range");
                        
                        return false;
                    }
                    break;
                case Enums.CapacityShootType.TargetEntity:
                    var targetEntity = EntityCollectionManager.GetEntityByIndex(targetsEntityIndex);
                    if (Vector3.Distance(casterPos, targetEntity.position) > maxRange)
                    {
                        Debug.Log($"Out of range");
                        return false;
                    }
                    break;
            }

            return AdditionalCastConditions(targetsEntityIndex, targetPosition);
        }

        protected abstract bool AdditionalCastConditions(int targetsEntityIndexes, Vector3 targetPositions);
        
        public void OnPress(int targetsEntityIndexes, Vector3 targetPositions)
        {
            if(!CanCast(targetsEntityIndexes,targetPositions)) return;
            if(isMaster) Press(targetsEntityIndexes,targetPositions);
            PressFeedback(targetsEntityIndexes,targetPositions);
        }
        protected abstract void Press(int targetsEntityIndexes, Vector3 targetPositions);
        protected abstract void PressFeedback(int targetsEntityIndexes, Vector3 targetPositions);

        public void OnHold(int targetsEntityIndexes, Vector3 targetPositions)
        {
            if(isMaster) Hold(targetsEntityIndexes,targetPositions);
            HoldFeedback(targetsEntityIndexes,targetPositions);
        }

        protected abstract void Hold(int targetsEntityIndexes, Vector3 targetPositions);
        protected abstract void HoldFeedback(int targetsEntityIndexes, Vector3 targetPositions);
        public void OnRelease(int targetsEntityIndexes, Vector3 targetPositions)
        {
            if(!CanCast(targetsEntityIndexes,targetPositions)) return;
            if(isMaster) Release(targetsEntityIndexes,targetPositions);
            ReleaseFeedback(targetsEntityIndexes,targetPositions);
            if(baseCooldown > 0) EnterCooldown(baseCooldown);
        }

        protected abstract void Release(int targetsEntityIndexes, Vector3 targetPositions);
        protected abstract void ReleaseFeedback(int targetsEntityIndexes, Vector3 targetPositions);

        private void EnterCooldown(double timeOnCooldown)
        {
            isOnCooldown = true;
            
            cooldownTimer = timeOnCooldown;
            
            gsm.OnTickFeedback += DecreaseCooldown;

            void DecreaseCooldown()
            {
                cooldownTimer -= gsm.increasePerTick;
                
                if(cooldownTimer > 0) return;
                isOnCooldown = false;
                gsm.OnTickFeedback -= DecreaseCooldown;
            }
        }

        public bool isInRange(Vector3 position)
        {
            float distance = Vector3.Distance(caster.transform.position, position);
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

