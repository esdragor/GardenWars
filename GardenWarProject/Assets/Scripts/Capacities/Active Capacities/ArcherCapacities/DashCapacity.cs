using GameStates;
using Photon.Pun;
using UnityEngine;

namespace Entities.Capacities
{
    public class DashCapacity : ActiveCapacity
    {
        private Champion.Champion champion => caster.GetComponent<Champion.Champion>();
        private bool isBlink => ((DashCapacitySO) AssociatedActiveCapacitySO()).isBlink;
        
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
            if (isBlink)
            {
                Blink(targetPositions[0]);
                return;
            }
            champion.agent.enabled = false;
        }

        private void Blink(Vector3 destination)
        {
            if (champion == null) return;
            champion.agent.Warp(destination);
        }

        private void DashTowardsDestination()
        {
            
        }

        public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            
        }
    }
}


