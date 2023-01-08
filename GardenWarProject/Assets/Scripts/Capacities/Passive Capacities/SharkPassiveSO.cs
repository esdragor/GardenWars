using System;
using UnityEngine;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/PassiveCapacitySO/SharkPassive", fileName = "SharkPassive")]
    public class SharkPassiveSO : PassiveCapacitySO
    {
        public double timeUntilBorrow = 3;
        public float borrowDamage = 10f;
        public double knockUpDuration = 1;
        public float knockUpDamage = 20f;
        
        public override Type AssociatedType()
        {
            return typeof(SharkPassive);
        }
    }

    public class SharkPassive : PassiveCapacity
    {
        private SharkPassiveSO so => (SharkPassiveSO) AssociatedPassiveCapacitySO();
        
        private double timer;

        private bool isMoving;
        
        public override PassiveCapacitySO AssociatedPassiveCapacitySO()
        {
            return CapacitySOCollectionManager.Instance.GetPassiveCapacitySOByIndex(indexOfSo);
        }

        protected override void OnAddedEffects(Entity target)
        {
            Debug.Log("Added Shark Passive :)");
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


