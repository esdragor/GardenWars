using UnityEngine;

namespace Entities.Capacities
{
    public class DashCapacity : ActiveCapacity
    {
        private Champion.Champion champion => caster.GetComponent<Champion.Champion>();
        private DashCapacitySO so => (DashCapacitySO)AssociatedActiveCapacitySO();
        private bool isBlink => so.isBlink;
        private double dashDuration => so.dashTime;

        protected override bool AdditionalCastConditions(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            return true;
        }

        protected override void Press(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            
        }

        protected override void PressFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            
        }

        protected override void Hold(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            
        }

        protected override void HoldFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            
        }

        protected override void Release(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            
        }

        protected override void ReleaseFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            var destination = targetPositions[0];
            var casterPos = caster.transform.position;
            if (Vector3.Distance(casterPos, destination) > so.maxRange || so.fixedDistance)
            {
                destination = casterPos + (destination - casterPos).normalized * so.maxRange;

            }
            if (isBlink)
            {
                Blink(destination);
                return;
            }
            StartDash(casterPos,destination);
        }

        private void Blink(Vector3 destination)
        {
            if (champion == null) return;
            champion.agent.Warp(destination);
        }

        private void StartDash(Vector3 start,Vector3 destination)
        {
            champion.agent.enabled = false;

            double timeInDash = 0;
            
            gsm.OnUpdateFeedback += DashTowardsDestination;

            void DashTowardsDestination()
            {
                timeInDash += Time.deltaTime;
                
                caster.transform.position = Vector3.Lerp(start,destination,(float)(timeInDash/dashDuration));
                if(timeInDash < dashDuration) return;
                champion.agent.enabled = true;
                champion.agent.Warp(destination);
                gsm.OnUpdateFeedback -= DashTowardsDestination;
            }
            
        }
    }
}


