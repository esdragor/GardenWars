using UnityEngine;

namespace Entities.Capacities
{
    public class ActiveHealPotion : ActiveCapacity
    {
        private ActiveHealPotionSO so;
        private IActiveLifeable lifeable;

        public override bool TryCast(int casterIndex, int[] targets, Vector3[] position)
        {
            return true;
        }

        protected override void Press(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            so = (ActiveHealPotionSO)AssociatedActiveCapacitySO();
            
            lifeable = caster.GetComponent<IActiveLifeable>();
            
            lifeable.IncreaseCurrentHpRPC(so.healAmount);
        }

        protected override void PressFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            so = (ActiveHealPotionSO)AssociatedActiveCapacitySO();
            instantiateFeedbackObj = PoolLocalManager.Instance.PoolInstantiate(so.feedbackPrefab, caster.transform.position, Quaternion.identity,
                caster.transform);
            
            InitializeFeedbackCountdown();
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
            
        }

        public override void PlayFeedback(int entityIndex, int[] targets, Vector3[] position)
        {
            so = (ActiveHealPotionSO)AssociatedActiveCapacitySO();
            instantiateFeedbackObj = PoolLocalManager.Instance.PoolInstantiate(so.feedbackPrefab, caster.transform.position, Quaternion.identity,
                caster.transform);
            
            InitializeFeedbackCountdown();
        }

        protected override void InitializeFeedbackCountdown()
        {
            base.InitializeFeedbackCountdown();
        }

        protected override void FeedbackCountdown()
        {
            base.FeedbackCountdown();
        }

        protected override void DisableFeedback()
        {
            base.DisableFeedback();
        }
    }
}

