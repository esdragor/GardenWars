using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/ConsumeCandy", fileName = "ConsumeCandy")]
    public class ConsumeCandySO : ActiveCapacitySO
    {
        public CandySpeedBuffSO buff;
        public int candyPerStack = 5;
        
        public override Type AssociatedType()
        {
            return typeof(ConsumeCandy);
        }
    }

    public class ConsumeCandy : ActiveCapacity
    {
        private ConsumeCandySO so => (ConsumeCandySO) AssociatedActiveCapacitySO();

        protected override bool AdditionalCastConditions(int targetsEntityIndexes, Vector3 targetPositions)
        {
            var stacks = champion.GetPassiveCapacityCount<CandySpeedBuff>();
            
            return champion.currentCandy >= (so.candyPerStack + so.candyPerStack * stacks) && !champion.isFighter;
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
            var stacks = champion.GetPassiveCapacityCount<CandySpeedBuff>();
            
            champion.DecreaseCurrentCandyRPC(so.candyPerStack + so.candyPerStack * stacks);
            
            champion.AddPassiveCapacityRPC(so.buff.indexInCollection);
        }

        protected override void ReleaseFeedback(int targetEntityIndex, Vector3 targetPositions)
        {
            
        }

        protected override void ReleaseLocal(int targetEntityIndex, Vector3 targetPositions)
        {
            
        }
    }
}

