using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/PassiveCapacitySO/FountainHeal", fileName = "FountainHeal")]
    public class FountainHealSO : PassiveCapacitySO
    {
        public float healthPerSecond = 5f;

        public override Type AssociatedType()
        {
            return typeof(FountainHeal);
        }
    }
    
    public class FountainHeal : PassiveCapacity
    {
        private FountainHealSO so => (FountainHealSO) AssociatedPassiveCapacitySO();
        
        private float timer;
        
        protected override void OnAddedEffects(Entity target)
        {
            timer = 0;

            gsm.OnTick += IncreaseHp;
        }

        protected override void OnAddedFeedbackEffects(Entity target)
        {
            
        }

        protected override void OnAddedLocalEffects(Entity target)
        {
            
        }

        protected override void OnRemovedEffects(Entity target)
        {
            gsm.OnTick -= IncreaseHp;
        }

        protected override void OnRemovedFeedbackEffects(Entity target)
        {
            
        }

        protected override void OnRemovedLocalEffects(Entity target)
        {
            
        }
        
        private void IncreaseHp()
        {
            timer += (float)gsm.increasePerTick;
                
            if(timer < 1) return;
            timer = 0;
            
            champion.IncreaseCurrentHpRPC(so.healthPerSecond,champion.entityIndex);
        }
    }
}


