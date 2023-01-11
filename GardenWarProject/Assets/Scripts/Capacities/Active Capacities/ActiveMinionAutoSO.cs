using System;
using GameStates;
using UnityEngine;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/Auto Attack Minion", fileName = "AI Auto Attack for Minion")]
    public class ActiveMinionAutoSO : ActiveCapacitySO
    {
        public int AtkValue;
        
        public override Type AssociatedType()
        {
            return typeof(ActivePinataAuto);
        }
    }
    
    public class ActiveMinionAuto : ActiveCapacity
    {
        private Entity _target;
        private double timer;
    
        protected override bool AdditionalCastConditions(int targetsEntityIndexes, Vector3 targetPositions)
        {
            return true;
        }
    
        protected override void Press(int targetsEntityIndexes, Vector3 targetPositions)
        {
            GameStateMachine.Instance.OnTick += DelayWaitingTick;
        }
    
        protected override void PressFeedback(int targetsEntityIndexes, Vector3 targetPositions)
        {
            
        }

        protected override void PressClient(int targetsEntityIndexes, Vector3 targetPositions)
        {
            
        }

        protected override void Hold(int targetsEntityIndexes, Vector3 targetPositions)
        {
            
        }
    
        protected override void HoldFeedback(int targetsEntityIndexes, Vector3 targetPositions)
        {
            
        }

        protected override void HoldClient(int targetsEntityIndexes, Vector3 targetPositions)
        {
            
        }

        protected override void Release(int targetsEntityIndexes, Vector3 targetPositions)
        {
            
        }
    
        protected override void ReleaseFeedback(int targetEntityIndex, Vector3 targetPositions)
        {
            
        }

        protected override void ReleaseClient(int targetEntityIndex, Vector3 targetPositions)
        {
            
        }

        private void DelayWaitingTick()
        {
            timer += 1 / GameStateMachine.Instance.tickRate;
    
            //   if (timer >= _minion.delayBeforeAttack) 
            {
                ApplyEffect();
                GameStateMachine.Instance.OnTick -= DelayWaitingTick;
            }
        }
    
        private void ApplyEffect()
        {
            //   if (Vector3.Distance(_target.transform.position, _minion.transform.position) < _minion.attackRange)
            {
                IActiveLifeable entityActiveLifeable = _target.GetComponent<IActiveLifeable>();
                //         entityActiveLifeable.DecreaseCurrentHpRPC(_minion.attackDamage);
            }
        }
    }
}


