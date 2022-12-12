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
        public Champion.Champion champion => caster as Champion.Champion;
        
        public bool isBasicAttack => champion != null && champion.attackAbilityIndex == indexOfSOInCollection;
        protected Vector3 casterPos => caster.transform.position;

        public double baseCooldown => champion == null ? AssociatedActiveCapacitySO().cooldown : isBasicAttack ? champion.attackSpeed : AssociatedActiveCapacitySO().cooldown;
        public bool isOnCooldown;
        private double cooldownTimer;

        protected Entity targetedEntity;
        protected Vector3 targetedPosition;
        
        protected GameStateMachine gsm => GameStateMachine.Instance;

        protected ActiveCapacitySO AssociatedActiveCapacitySO()
        {
            return CapacitySOCollectionManager.GetActiveCapacitySOByIndex(indexOfSOInCollection);
        }

        public bool CanCast(int targetsEntityIndex, Vector3 targetPosition)
        {
            if (isOnCooldown)
            {
                return false;
            }
            var so = AssociatedActiveCapacitySO();
            var maxRange = isBasicAttack ? champion.attackRange : so.maxRange;
            switch (so.shootType)
            {
                case Enums.CapacityShootType.Skillshot:
                    break;
                case Enums.CapacityShootType.TargetPosition:
                    targetedPosition = targetPosition;
                    if (Vector3.Distance(casterPos, targetedPosition) > maxRange)
                    {
                        return false;
                    }
                    break;
                case Enums.CapacityShootType.TargetEntity:
                    targetedEntity = EntityCollectionManager.GetEntityByIndex(targetsEntityIndex);
                    if (targetedEntity == null)
                    {
                        return false;
                    }
                    if (Vector3.Distance(casterPos, targetedEntity.position) > maxRange)
                    {
                        return false;
                    }
                    break;
            }

            return AdditionalCastConditions(targetsEntityIndex, targetPosition);
        }

        protected abstract bool AdditionalCastConditions(int targetsEntityIndexes, Vector3 targetPositions);
        
        public bool OnPress(int targetsEntityIndexes, Vector3 targetPositions)
        {
            if(!CanCast(targetsEntityIndexes,targetPositions)) return false;
            if(isMaster) Press(targetsEntityIndexes,targetPositions);
            PressFeedback(targetsEntityIndexes,targetPositions);
            return true;
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
        protected abstract void ReleaseFeedback(int targetEntityIndex, Vector3 targetPositions);

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

