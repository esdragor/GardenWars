using System;
using GameStates;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

namespace Entities.Capacities
{
    public abstract class ActiveCapacity
    {
        protected bool isMaster => !PhotonNetwork.IsConnected || PhotonNetwork.IsMasterClient;
        public byte indexOfSOInCollection;
        
        public Entity caster;
        private ICastable castable => caster.GetComponent<ICastable>();
        private IMoveable moveable => caster.GetComponent<IMoveable>();
        private IAttackable attackable => caster.GetComponent<IAttackable>();
        public Champion.Champion champion => caster as Champion.Champion;
        
        public bool isBasicAttack => champion != null && champion.attackAbilityIndex == indexOfSOInCollection;
        protected Transform casterTr => caster.transform;
        protected Vector3 casterPos => casterTr.position;

        public double baseCooldown => champion == null ? AssociatedActiveCapacitySO().cooldown : isBasicAttack ? champion.attackSpeed : AssociatedActiveCapacitySO().cooldown;
        public bool isOnCooldown;
        public double cooldownTimer { get; private set; }

        private double castTimeTimer;
        private double castTime => AssociatedActiveCapacitySO().castTime;

        protected Entity targetedEntity;
        protected Vector3 targetedPosition;

        public int level;
        public bool canBeUpgraded => level < AssociatedActiveCapacitySO().maxLevel;

        protected GameStateMachine gsm => GameStateMachine.Instance;

        private ActiveCapacitySO _so => AssociatedActiveCapacitySO();

        public ActiveCapacitySO AssociatedActiveCapacitySO()
        {
            return CapacitySOCollectionManager.GetActiveCapacitySOByIndex(indexOfSOInCollection);
        }

        public bool CanCast(int targetsEntityIndex, Vector3 targetPosition)
        {
            if (isOnCooldown)
            {
                if(champion != null) champion.HideAreaIndicator();
                return false;
            }
            var maxRange = isBasicAttack ? champion.attackRange : _so.maxRange;
            var canCast = true;
            switch (_so.shootType)
            {
                case Enums.CapacityShootType.Skillshot:
                    break;
                case Enums.CapacityShootType.TargetPosition:
                    targetedPosition = targetPosition;
                    if (Vector3.Distance(casterPos, targetedPosition) > maxRange && maxRange != 0)
                    {
                        canCast = false;
                    }
                    break;
                case Enums.CapacityShootType.TargetEntity:
                    targetedEntity = EntityCollectionManager.GetEntityByIndex(targetsEntityIndex);
                    if (targetedEntity == null)
                    {
                        canCast = false;
                    }
                    if (Vector3.Distance(casterPos, targetedEntity.position) > maxRange)
                    {
                        canCast = false;
                    }

                    var targetable = targetedEntity.GetComponent<ITargetable>();
                    if (targetable != null)
                    {
                        if (!targetable.CanBeTargeted())
                        {
                            canCast = false;
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (canCast || champion == null) return AdditionalCastConditions(targetsEntityIndex, targetPosition);
            
            champion.HideAreaIndicator();
            champion.HideMaxRangeIndicator();
            champion.HideSkillShotIndicator();
            
            return false;
        }

        protected abstract bool AdditionalCastConditions(int targetsEntityIndexes, Vector3 targetPositions);
        
        public bool OnPress(int targetsEntityIndexes, Vector3 targetPositions)
        {
            if(!CanCast(targetsEntityIndexes,targetPositions)) return false;
            if(isMaster) Press(targetsEntityIndexes,targetPositions);
            PressFeedback(targetsEntityIndexes,targetPositions);
            
            if (!caster.isLocal) return true;
            
            switch (_so.shootType)
            {
                case Enums.CapacityShootType.Skillshot:
                    if(_so.showMaxRangeIfSkillShot) champion.ShowMaxRangeIndicator(_so.maxRange);
                    break;
                case Enums.CapacityShootType.TargetPosition:
                    champion.ShowMaxRangeIndicator(_so.maxRange);
                    break;
                case Enums.CapacityShootType.TargetEntity:
                    champion.ShowMaxRangeIndicator(_so.maxRange);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            PressLocal(targetsEntityIndexes,targetPositions);
            return true;
        }
        protected abstract void Press(int targetsEntityIndexes, Vector3 targetPositions);
        protected abstract void PressFeedback(int targetsEntityIndexes, Vector3 targetPositions);
        protected abstract void PressLocal(int targetsEntityIndexes, Vector3 targetPositions);

        public void OnHold(int targetsEntityIndexes, Vector3 targetPositions)
        {
            if(isOnCooldown) return;
            if(isMaster) Hold(targetsEntityIndexes,targetPositions);
            HoldFeedback(targetsEntityIndexes,targetPositions);
            if (caster.isLocal)
            {
                if(_so.shootType == Enums.CapacityShootType.Skillshot) champion.ShowSkillShotIndicator(targetPositions,_so.maxRange);
                HoldLocal(targetsEntityIndexes,targetPositions);
            }
        }

        protected abstract void Hold(int targetsEntityIndexes, Vector3 targetPositions);
        protected abstract void HoldFeedback(int targetsEntityIndexes, Vector3 targetPositions);
        protected abstract void HoldLocal(int targetsEntityIndexes, Vector3 targetPositions);
        public void OnRelease(int targetsEntityIndexes, Vector3 targetPositions)
        {
            if(champion != null) champion.MoveToPosition(champion.position);
            
            castTimeTimer = 0;

            if (castTime == 0)
            {
                ReleaseCapacity();
                return;
            }
            
            castable?.SetCanCastRPC(false);
            attackable?.SetCanAttackRPC(false);
            moveable?.SetCanMoveRPC(false);

            gsm.OnUpdate += DecreaseCastTimer;
            
            
            void ReleaseCapacity()
            {
                if(!CanCast(targetsEntityIndexes,targetPositions)) return;
                if(isMaster) Release(targetsEntityIndexes,targetPositions);
                ReleaseFeedback(targetsEntityIndexes,targetPositions);
                if (caster.isLocal)
                {
                    switch (_so.shootType)
                    {
                        case Enums.CapacityShootType.Skillshot:
                            champion.HideMaxRangeIndicator();
                            champion.HideSkillShotIndicator();
                            break;
                        case Enums.CapacityShootType.TargetPosition:
                            champion.HideMaxRangeIndicator();
                            break;
                        case Enums.CapacityShootType.TargetEntity:
                            champion.HideMaxRangeIndicator();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    
                    ReleaseLocal(targetsEntityIndexes,targetPositions);
                }
                if(baseCooldown > 0) EnterCooldown(baseCooldown);
            }
            
            void DecreaseCastTimer()
            {
                castTimeTimer += Time.deltaTime;
                
                if(castTimeTimer < castTime) return;
                
                gsm.OnUpdate -= DecreaseCastTimer;
                
                castable?.SetCanCastRPC(true);
                attackable?.SetCanAttackRPC(true);
                moveable?.SetCanMoveRPC(true);
                
                ReleaseCapacity();
            }
        }
        
        

        protected abstract void Release(int targetsEntityIndexes, Vector3 targetPositions);
        protected abstract void ReleaseFeedback(int targetEntityIndex, Vector3 targetPositions);
        protected abstract void ReleaseLocal(int targetEntityIndex, Vector3 targetPositions);

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
                OnCooldownEnded?.Invoke();
                gsm.OnTickFeedback -= DecreaseCooldown;
            }
        }
        public event Action OnCooldownEnded; 

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

