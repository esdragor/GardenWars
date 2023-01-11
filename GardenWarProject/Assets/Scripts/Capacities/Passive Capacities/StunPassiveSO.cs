using System;
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
        private IMoveable moveable;
        private IAttackable attackable;
        private ICastable castable;
        
        public override PassiveCapacitySO AssociatedPassiveCapacitySO()
        {
            return CapacitySOCollectionManager.Instance.GetPassiveCapacitySOByIndex(indexOfSo);
        }

        protected override void OnAddedEffects(Entity target)
        {
            moveable = entity.GetComponent<IMoveable>();
            attackable = entity.GetComponent<IAttackable>();
            castable = entity.GetComponent<ICastable>();

            moveable?.SetCanMoveRPC(false);
            attackable?.SetCanAttackRPC(false);
            castable?.SetCanCastRPC(false);
        }

        protected override void OnAddedFeedbackEffects(Entity target)
        {
            
        }

        protected override void OnAddedLocalEffects(Entity target)
        {
            
        }

        protected override void OnRemovedEffects(Entity target)
        {
            moveable?.SetCanMoveRPC(true);
            attackable?.SetCanAttackRPC(true);
            castable?.SetCanCastRPC(true);
        }

        protected override void OnRemovedFeedbackEffects(Entity target)
        {
            
        }

        protected override void OnRemovedLocalEffects(Entity target)
        {
            
        }
    }
}


