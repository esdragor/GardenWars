using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/PassiveCapacitySO/StunPassive", fileName = "StunPassive")]
    public class StunPassiveSO : PassiveCapacitySO
    {
        public override Type AssociatedType()
        {
            return typeof(StunPassive);
        }
    }

    public class StunPassive : PassiveCapacity
    {
        public override PassiveCapacitySO AssociatedPassiveCapacitySO()
        {
            return CapacitySOCollectionManager.Instance.GetPassiveCapacitySOByIndex(indexOfSo);
        }

        protected override void OnAddedEffects(Entity target)
        {
        }

        protected override void OnAddedFeedbackEffects(Entity target)
        {
            
        }

        protected override void OnRemovedEffects(Entity target)
        {
        }

        protected override void OnRemovedFeedbackEffects(Entity target)
        {
        }
    }
}


