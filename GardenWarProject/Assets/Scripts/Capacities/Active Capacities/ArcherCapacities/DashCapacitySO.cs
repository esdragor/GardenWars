using System;
using UnityEngine;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/DashCapacity", fileName = "new DashCapacity")]
    public class DashCapacitySO : ActiveCapacitySO
    {
        [Header("Dash Settings")]
        public bool isBlink;
        [Tooltip("In Seconds")]public double dashTime;
        public bool fixedDistance;
        public bool canPassWalls;
        
        public override Type AssociatedType()
        {
            return typeof(DashCapacity);
        }
    }
    
        public class DashCapacity : ActiveCapacity
    {
        private Champion.Champion champion => caster.GetComponent<Champion.Champion>();
        private DashCapacitySO so => (DashCapacitySO)AssociatedActiveCapacitySO();
        private bool isBlink => so.isBlink;
        private double dashDuration => so.dashTime;

        private LayerMask collisionLayers;

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

        protected override void Hold(int targetsEntityIndexes, Vector3 targetPositions)
        {
            
        }

        protected override void HoldFeedback(int targetsEntityIndexes, Vector3 targetPositions)
        {
            
        }

        protected override void Release(int targetsEntityIndexes, Vector3 targetPositions)
        {
            
        }

        protected override void ReleaseFeedback(int targetEntityIndex, Vector3 targetPositions)
        {
            if (champion != null)  champion.canMove = false;
            
            collisionLayers = 1 << 9 | 1 << 22 | 1 << 30;
            
            var destination = targetPositions;
            destination.y = casterPos.y;
            
            var direction = (destination - casterPos).normalized;
            var distance = Vector3.Distance(casterPos, destination);
            
            if (distance > so.maxRange || so.fixedDistance) distance = so.maxRange;
            
            destination = casterPos + direction * distance;
            
            
            if (Physics.Raycast(casterPos, direction, out var hit, distance, collisionLayers) && !so.canPassWalls && !isBlink)
            {
                destination = hit.point;
            }
            
            destination = GetClosestValidPoint(destination);
            
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
            champion.canMove = true;
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
                Blink(destination);
                gsm.OnUpdateFeedback -= DashTowardsDestination;
            }
            
        }
    }
}