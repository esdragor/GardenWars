using System;
using UnityEngine;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/PassiveCapacitySO/AttackSpeedBuff", fileName = "AttackSpeedBuff")]
    public class AttackSpeedBuffSO : PassiveCapacitySO
    {
        public double timeGained = 0.5f;

        public override Type AssociatedType()
        {
            return typeof(AttackSpeedBuff);
        }
    }
    
    public class AttackSpeedBuff : PassiveCapacity
    {
        private AttackSpeedBuffSO so => (AttackSpeedBuffSO) AssociatedPassiveCapacitySO();
        protected override void OnAddedEffects(Entity target)
        {
            champion.ChangeAttackSpeedRPC((float)so.timeGained);
        }

        protected override void OnAddedFeedbackEffects(Entity target)
        {
            
        }

        protected override void OnAddedLocalEffects(Entity target)
        {
            
        }

        protected override void OnRemovedEffects(Entity target)
        {
            champion.ChangeAttackSpeedRPC(-(float)so.timeGained);
        }

        protected override void OnRemovedFeedbackEffects(Entity target)
        {
            
        }

        protected override void OnRemovedLocalEffects(Entity target)
        {
            
        }
    }
}