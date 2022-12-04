using GameStates;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

namespace Entities.Capacities
{
    public abstract class ActiveCapacity
    {
        private bool isMaster => !PhotonNetwork.IsConnected || PhotonNetwork.IsMasterClient;
        public byte indexOfSOInCollection;
        public Entity caster;
        private double cooldownTimer;
        public bool onCooldown;
        private double feedbackTimer;
        
        public GameObject instantiateFeedbackObj;

        protected int target;

        protected ActiveCapacitySO AssociatedActiveCapacitySO()
        {
            return CapacitySOCollectionManager.GetActiveCapacitySOByIndex(indexOfSOInCollection);
        }

        #region Cast

        /// <summary>
        /// Initialize the cooldown of the capacity when used.
        /// </summary>
        protected virtual void InitiateCooldown()
        {
            cooldownTimer = AssociatedActiveCapacitySO().cooldown;
            onCooldown = true;
         
            
            GameStateMachine.Instance.OnTick += CooldownTimer;
        }

        /// <summary>
        /// Method which update the timer.
        /// </summary>
        protected virtual void CooldownTimer()
        {
            cooldownTimer -= 1.0 / GameStateMachine.Instance.tickRate;
            
            if (cooldownTimer <= 0)
            {
                Debug.Log("Cooldown is over");
                onCooldown = false;
                Debug.Log(onCooldown);
                GameStateMachine.Instance.OnTick -= CooldownTimer;
            }
        }
        
        /// <summary>
        /// Called when trying cast a capacity.
        /// </summary>
        /// <param name="casterIndex"></param>
        /// <param name="targetsEntityIndexes"></param>
        /// <param name="targetPositions"></param>
        /// <returns></returns>
        public virtual bool TryCast(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            // if (Vector3.Distance(EntityCollectionManager.GetEntityByIndex(casterIndex).transform.position, EntityCollectionManager.GetEntityByIndex(targetsEntityIndexes[0]).transform.position)> 
            //     AssociatedActiveCapacitySO().maxRange) return false;
            
            if (!onCooldown)
            {
                InitiateCooldown();
                return true;
            }
            else return false;
        }

        public virtual bool CanCast(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            //if out of range return false
            // if on cooldown return false
            return true;
        }

        public void OnPress(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            if(isMaster) Press(casterIndex,targetsEntityIndexes,targetPositions);
            PressFeedback(casterIndex,targetsEntityIndexes,targetPositions);
        }
        protected abstract void Press(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions);
        protected abstract void PressFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions);

        public void OnHold(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            if(isMaster) Hold(casterIndex,targetsEntityIndexes,targetPositions);
            HoldFeedback(casterIndex,targetsEntityIndexes,targetPositions);
        }

        protected abstract void Hold(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions);
        protected abstract void HoldFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions);
        public void OnRelease(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            if(isMaster) Release(casterIndex,targetsEntityIndexes,targetPositions);
            ReleaseFeedback(casterIndex,targetsEntityIndexes,targetPositions);
        }

        protected abstract void Release(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions);
        protected abstract void ReleaseFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions);

        public virtual bool isInRange(int casterIndex, Vector3 position)
        {
            float distance = Vector3.Distance(EntityCollectionManager.GetEntityByIndex(casterIndex).transform.position, position);
            //Debug.Log($"distance:{distance}  >  range:{ AssociatedActiveCapacitySO().maxRange}");
            if ( distance > AssociatedActiveCapacitySO().maxRange) return false;
            
            return true;
        }

        #endregion

        #region Feedback

        public abstract void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions);

        protected virtual void InitializeFeedbackCountdown()
        {
            feedbackTimer = AssociatedActiveCapacitySO().feedbackDuration;
            GameStateMachine.Instance.OnTick += FeedbackCountdown;
        }

        protected virtual void FeedbackCountdown()
        {
            feedbackTimer -= GameStateMachine.Instance.tickRate;

            if (feedbackTimer <= 0)
            {
                DisableFeedback();
            }
        }
        
        protected virtual void DisableFeedback()
        {
            PoolLocalManager.Instance.EnqueuePool(AssociatedActiveCapacitySO().feedbackPrefab, instantiateFeedbackObj);
            GameStateMachine.Instance.OnTick -= FeedbackCountdown;
        }
        
        #endregion
        
        public static Vector3 GetClosestValidPoint(Vector3 position)
        {
            if (NavMesh.SamplePosition(position, out var hit, 5, NavMesh.AllAreas))
            {
                return hit.position;
            }
            position.y = 0;
            return position;
        }
    }
}

