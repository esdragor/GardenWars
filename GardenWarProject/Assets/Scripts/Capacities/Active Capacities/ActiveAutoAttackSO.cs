using System;
using Entities;
using Entities.Capacities;
using GameStates;
using UnityEngine;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/Auto Attack", fileName = "new Auto Attack")]
    public class ActiveAutoAttackSO : ActiveCapacitySO
    {
        public float range;
        public float damage;
        public float attackSpeed;
        
        public override Type AssociatedType()
        {
            return typeof(ActiveAutoAttack);
        }
    }
    
    public class ActiveAutoAttack : ActiveCapacity
    {
        private ActiveAutoAttackSO activeAutoAttackSO; 
        private double attackTimer;
        private int target;
    
        protected override bool AdditionalCastConditions(int targetsEntityIndexes, Vector3 targetPositions)
        {
            return true;
        }
    
        protected override void Press(int targetsEntityIndexes, Vector3 targetPositions)
        {
        }
    
        protected override void PressFeedback(int targetsEntityIndexes, Vector3 targetPositions)
        {
        }

        protected override void PressLocal(int targetsEntityIndexes, Vector3 targetPositions)
        {
            
        }

        protected override void Hold(int targetsEntityIndexes, Vector3 targetPositions)
        {
        }
    
        protected override void HoldFeedback(int targetsEntityIndexes, Vector3 targetPositions)
        {
        }

        protected override void HoldLocal(int targetsEntityIndexes, Vector3 targetPositions)
        {
            
        }

        protected override void Release(int targetsEntityIndexes, Vector3 targetPositions)
        {
        }
    
        protected override void ReleaseFeedback(int targetEntityIndex, Vector3 targetPositions)
        {
        }

        protected override void ReleaseLocal(int targetEntityIndex, Vector3 targetPositions)
        {
            
        }

        private void ApplyEffect()
        {
            IActiveLifeable activeLifeable = EntityCollectionManager.GetEntityByIndex(target).GetComponent<IActiveLifeable>();
            activeLifeable.DecreaseCurrentHpRPC(activeAutoAttackSO.damage, caster.entityIndex);
        }
    
        public void DelayAutoAttack()
        {
            attackTimer += GameStateMachine.Instance.tickRate;
            if (attackTimer >= activeAutoAttackSO.attackSpeed)
            {
                attackTimer = 0;
                ApplyEffect();
            }
        }
        
    }
    

}

