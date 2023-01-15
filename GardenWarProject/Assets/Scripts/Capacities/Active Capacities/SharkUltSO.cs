using System;
using System.Collections;
using System.Collections.Generic;
using Controllers.Inputs;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/SharkUlt", fileName = "new Shark Ult")]
    public class SharkUltSO : ActiveCapacitySO
    {
        public ProjectileOnCollideEffect impactArea;
        public StunPassiveSO stun;

        public float animationDuration = 1f;
        public float impactZoneAppearance = 0.9f;
        public float sizeMultiplier = 1.5f;

        public float borrowDuration = 1f;
        
        public float damage = 50f;

        public override Type AssociatedType()
        {
            return typeof(SharkUlt);
        }
    }

    public class SharkUlt : ActiveCapacity
    {
        private SharkUltSO so => (SharkUltSO) AssociatedActiveCapacitySO();
        private float timer;
        private GameObject damageAreaGo;
        private ProjectileOnCollideEffect damageArea;
        
        private SharkPassive passive;
        private SharkPassive sharkPassive => passive ??= champion.GetPassiveCapacity<SharkPassive>();
        
        private bool ultBorrowed;

        protected override bool AdditionalCastConditions(int targetsEntityIndexes, Vector3 targetPositions)
        {
            return true;
        }

        protected override void Press(int targetsEntityIndexes, Vector3 targetPositions)
        {
            
        }

        protected override void PressFeedback(int targetsEntityIndexes, Vector3 targetPositions)
        {
            
        }

        protected override void PressLocal(int targetsEntityIndexes, Vector3 targetPositions)
        {
            
        }

        protected override void Hold(int targetsEntityIndexes, Vector3 targetPositions)
        {
            
        }

        protected override void HoldFeedback(int targetsEntityIndexes, Vector3 targetPositions)
        {
            
        }

        protected override void HoldLocal(int targetsEntityIndexes, Vector3 targetPositions)
        {
            
        }

        protected override void Release(int targetsEntityIndexes, Vector3 targetPositions)
        {
            
        }

        protected override void ReleaseFeedback(int targetEntityIndex, Vector3 targetPositions)
        {
            if (damageArea == null)
            {
                damageArea = LocalPoolManager.PoolInstantiate(so.impactArea,champion.rotateParent);
                damageArea.transform.localPosition = Vector3.zero;
                damageAreaGo = damageArea.gameObject;
                damageAreaGo.SetActive(false);

                damageArea.OnEntityCollide += DamageAndStun;
            }
            
            if (ultBorrowed)
            {
                ExitUltBorrow();
                return;
            }
            
            if (isMaster)
            {
                champion.SetCanAttackRPC(false);
                champion.SetCanCastRPC(false);
                champion.SetCanMoveRPC(false);
            }
            
            Leap(targetPositions);

            timer = 0;

            ultBorrowed = false;

            gsm.OnUpdateFeedback += IncreaseTimerPart1;

            void IncreaseTimerPart1()
            {
                timer += Time.deltaTime;
                
                if (timer / so.animationDuration <= so.impactZoneAppearance) return;
                
                if(!damageAreaGo.activeSelf) damageAreaGo.SetActive(true);

                if (timer <= so.animationDuration) return;

                timer = 0;

                UltBorrow();
                    
                gsm.OnUpdateFeedback -= IncreaseTimerPart1;
                gsm.OnUpdateFeedback += IncreaseTimerPart2;
            }

            void UltBorrow()
            {
                if (isMaster)
                {
                    champion.SetCanCastRPC(true);
                    champion.SetCanMoveRPC(true);
                }
                
                sharkPassive.Borrow(true);
                
                ultBorrowed = true;
            }

            void IncreaseTimerPart2()
            {
                timer += Time.deltaTime;
                
                if(timer <= so.borrowDuration) return;
                
                ExitUltBorrow();
            }

            void ExitUltBorrow()
            {
                if (isMaster)
                {
                    champion.SetCanAttackRPC(false);
                    champion.SetCanCastRPC(false);
                    champion.SetCanMoveRPC(false);
                }
                
                Leap(PlayerInputController.CursorWorldPos);

                timer = 0;
                
                ultBorrowed = false;
                
                gsm.OnUpdateFeedback -= IncreaseTimerPart2;
                gsm.OnUpdateFeedback += IncreaseTimerPart3;
            }

            void IncreaseTimerPart3()
            {
                timer += Time.deltaTime;
                    
                if (timer / so.animationDuration <= so.impactZoneAppearance) return;
                
                if(!damageAreaGo.activeSelf) damageAreaGo.SetActive(true);

                if (timer <= so.animationDuration) return;

                timer = 0;
                
                champion.rotateParent.localScale = Vector3.one;
                    
                damageAreaGo.SetActive(false);

                if (isMaster)
                {
                    champion.SetCanAttackRPC(true);
                    champion.SetCanCastRPC(true);
                    champion.SetCanMoveRPC(true);
                }

                gsm.OnUpdateFeedback -= IncreaseTimerPart3;
            }

            void DamageAndStun(Entity entity)
            {
                if (!caster.GetEnemyTeams().Contains(entity.team)) return;
                
                entity.AddPassiveCapacityRPC(so.stun.indexInCollection);
                
                var lifeable = entity.GetComponent<IActiveLifeable>();
                lifeable?.DecreaseCurrentHpRPC(so.damage,caster.entityIndex);
            }
        }

        private void Leap(Vector3 destination)
        {
            sharkPassive.ForceUnBorrow();
            
            destination = GetClosestValidPoint(destination);

            if (Vector3.Distance(champion.position, destination) > so.maxRange)
            {
                var dir = destination - champion.position;
                dir = dir.normalized * so.maxRange;
                destination = champion.position + dir;
                destination = GetClosestValidPoint(destination);
            }
            
            Debug.Log($"Leaping to {destination} (cursor is at {PlayerInputController.CursorWorldPos})");
            
            champion.SetAnimatorTrigger("Ability3");
            champion.rotateParent.localScale = Vector3.one * so.sizeMultiplier;
            
            champion.LookAt(destination);

            if (isMaster)
            {
                champion.DisplaceRPC(destination,so.animationDuration);
            }
        }

        protected override void ReleaseLocal(int targetEntityIndex, Vector3 targetPositions)
        {
            
        }
    }
    
}


