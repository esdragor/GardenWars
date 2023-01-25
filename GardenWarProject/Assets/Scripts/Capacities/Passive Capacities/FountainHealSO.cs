using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/PassiveCapacitySO/FountainHeal", fileName = "FountainHeal")]
    public class FountainHealSO : PassiveCapacitySO
    {
        public ParticleSystem fx;
        public float healthPerSecond = 5f;

        public override Type AssociatedType()
        {
            return typeof(FountainHeal);
        }
    }
    
    public class FountainHeal : PassiveCapacity
    {
        private FountainHealSO so => (FountainHealSO) AssociatedPassiveCapacitySO();

        private GameObject fxGo;
        private float timer;
        
        protected override void OnAddedEffects(Entity target)
        {
            
        }

        protected override void OnAddedFeedbackEffects(Entity target)
        {
            fxGo = LocalPoolManager.PoolInstantiate(so.fx, champion.position, Quaternion.Euler(-90, 0, 0)).gameObject;
            fxGo.SetActive(champion.isVisible);
            
            timer = 0;

            gsm.OnTickFeedback += IncreaseHp;
        }

        protected override void OnAddedLocalEffects(Entity target)
        {
            
        }

        protected override void OnRemovedEffects(Entity target)
        {
            
        }

        protected override void OnRemovedFeedbackEffects(Entity target)
        {
            gsm.OnTickFeedback -= IncreaseHp;
            
            fxGo.SetActive(false);
        }

        protected override void OnRemovedLocalEffects(Entity target)
        {
            
        }
        
        private void IncreaseHp()
        {
            timer += (float)gsm.increasePerTick;

            fxGo.transform.position = champion.position;
            
            fxGo.SetActive(champion.isVisible);

            if(timer < 1) return;
            timer = 0;
            
            champion.IncreaseCurrentHpRPC(so.healthPerSecond,champion.entityIndex);
        }
    }
}


