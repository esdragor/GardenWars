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
        private MoveSpeedSO passiveCapacitySo;
        public override PassiveCapacitySO AssociatedPassiveCapacitySO()
        {
            return CapacitySOCollectionManager.Instance.GetPassiveCapacitySOByIndex(indexOfSo);
        }

        protected override void OnAddedEffects(Entity target)
        {
            passiveCapacitySo = (MoveSpeedSO)AssociatedPassiveCapacitySO();
            target.GetComponent<NavMeshAgent>().speed += passiveCapacitySo.moveSpeed;
        }

        protected override void OnAddedFeedbackEffects(Entity target)
        {
        }

        protected override void OnRemovedEffects(Entity target)
        {
            target.GetComponent<NavMeshAgent>().speed -= passiveCapacitySo.moveSpeed;
        }

        protected override void OnRemovedFeedbackEffects(Entity target)
        {
        }
    }
}


