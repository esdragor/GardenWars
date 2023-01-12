using System;
using System.Collections;
using System.Collections.Generic;
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

        public float maxBorrowedTime = 1f; 
        
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
                damageArea = Object.Instantiate(so.impactArea,champion.rotateParent);
                damageArea.transform.localPosition = Vector3.zero;
                damageAreaGo = damageArea.gameObject;
                damageAreaGo.SetActive(false);

                damageArea.OnEntityCollide += DamageAndStun;
            }
            
            champion.SetAnimatorTrigger("Ability3");
            champion.rotateParent.localScale = Vector3.one * so.sizeMultiplier;

            timer = 0;

            var destination = GetClosestValidPoint(targetPositions);
            champion.LookAt(destination);
            
            if(isMaster) champion.DisplaceRPC(destination,so.animationDuration);

            gsm.OnUpdateFeedback += IncreaseTimer;

            void IncreaseTimer()
            {
                timer += Time.deltaTime;

                if (timer / so.animationDuration <= so.impactZoneAppearance) return;
                
                if(!damageAreaGo.activeSelf) damageAreaGo.SetActive(true);

                if (timer <= so.animationDuration) return;

                timer = 0;
                champion.rotateParent.localScale = Vector3.one;
                damageAreaGo.SetActive(false);

                gsm.OnUpdateFeedback -= IncreaseTimer;
            }

            void DamageAndStun(Entity entity)
            {
                if (!caster.GetEnemyTeams().Contains(entity.team)) return;
                
                entity.AddPassiveCapacityRPC(so.stun.indexInCollection);
                
                var lifeable = entity.GetComponent<IActiveLifeable>();
                lifeable?.DecreaseCurrentHpRPC(so.damage,caster.entityIndex);
            }
        }

        protected override void ReleaseLocal(int targetEntityIndex, Vector3 targetPositions)
        {
            
        }
    }
    
}


