using UnityEngine;

namespace Entities.Capacities
{
    public class RangedAACapacity : ActiveCapacity
    {
        private Entity target;
        
        protected override bool AdditionalCastConditions(int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            target = EntityCollectionManager.GetEntityByIndex(targetsEntityIndexes[0]);
            if (target == null)
            {
                Debug.Log("No Target");
                return false;
            }

            if (!caster.GetEnemyTeams().Contains(target.team))
            {
                Debug.Log("Target is Ally");
                return false;
            }
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
            //spawn un projectile
            // lui dit d'aller vers le target
            // a l'impact fait X degat
            //(ptet faire un object séparé jsp)
        }

        protected override void ReleaseFeedback(int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            
        }
    }
}


