using System;
using UnityEngine;
using UnityEngine.AI;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/PassiveCapacitySO/MoveSpeed", fileName = "MoveSpeed")]
    public class MoveSpeedSO : PassiveCapacitySO
    {
        public float moveSpeed = 5f;
        
        public override Type AssociatedType()
        {
            return typeof(MoveSpeed);
        }
    }

    public class MoveSpeed : PassiveCapacity
    {
        private MoveSpeedSO so => (MoveSpeedSO)AssociatedPassiveCapacitySO();
        private IMoveable moveable;

        protected override void OnAddedEffects(Entity target)
        {
            moveable = entity.GetComponent<IMoveable>();
            moveable?.IncreaseCurrentMoveSpeedRPC(so.moveSpeed);
        }

        protected override void OnAddedFeedbackEffects(Entity target)
        {
        }

        protected override void OnAddedLocalEffects(Entity target)
        {
            
        }

        protected override void OnRemovedEffects(Entity target)
        {
            moveable?.DecreaseCurrentMoveSpeedRPC(so.moveSpeed);
        }

        protected override void OnRemovedFeedbackEffects(Entity target)
        {
        }

        protected override void OnRemovedLocalEffects(Entity target)
        {
            
        }
    }
}