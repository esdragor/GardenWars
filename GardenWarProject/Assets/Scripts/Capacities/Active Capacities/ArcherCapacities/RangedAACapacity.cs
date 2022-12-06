using UnityEngine;

namespace Entities.Capacities
{
    public class RangedAACapacity : ActiveCapacity
    {
        protected override bool AdditionalCastConditions(int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            return true;
        }

        protected override void Press(int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            
        }

        protected override void PressFeedback(int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            
        }

        protected override void Hold(int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            
        }

        protected override void HoldFeedback(int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            
        }

        protected override void Release(int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            
        }

        protected override void ReleaseFeedback(int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            
        }
    }
}


