using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/PassiveCapacitySO/SharkUltBorrow", fileName = "SharkUltBorrow")]
    public class SharkUltBorrowSO : PassiveCapacitySO
    {
        public float maxBorrowedTime = 1f; 
        
        public override Type AssociatedType()
        {
            return typeof(SharkUltBorrow);
        }
    }

    public class SharkUltBorrow : PassiveCapacity
    {
        private SharkPassive sharkPassive;
        
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

        protected override void OnAddedLocalEffects(Entity target)
        {
            
        }

        protected override void OnRemovedEffects(Entity target)
        {
            
        }

        protected override void OnRemovedFeedbackEffects(Entity target)
        {
            
        }

        protected override void OnRemovedLocalEffects(Entity target)
        {
            
        }
    }
}


