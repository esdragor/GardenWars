using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/PassiveCapacitySO/FeedPinata", fileName = "FeedPinata")]
    public class FeedPinataSO : PassiveCapacitySO
    {
        public override Type AssociatedType()
        {
            return typeof(FeedPinata);
        }
    }
    
    public class FeedPinata : PassiveCapacity
    {
        private FeedPinataSO so => (FeedPinataSO)AssociatedPassiveCapacitySO();
        
        private bool isCanceled;
        
        public override PassiveCapacitySO AssociatedPassiveCapacitySO()
        {
            return CapacitySOCollectionManager.Instance.GetPassiveCapacitySOByIndex(indexOfSo);
        }

        protected override void OnAddedEffects(Entity target)
        {
            /*
            champion.OnDecreaseCurrentHp += CancelRecall;
            champion.OnMove += CancelRecall;
            champion.OnAttack += CancelRecall;
            champion.OnCast += CancelRecall;
            */
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

