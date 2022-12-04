using GameStates;
using UnityEngine;

namespace Entities.Capacities
{
    public class ActiveTormentedShadow : ActiveCapacity
    {
        private Vector3 position;
        public ActiveTormentedShadowSO so;
        public double tickDamageTimer;
        public float durationTimer;
        
        public override bool TryCast(int casterIndex, int[] targets, Vector3[] pos)
        {
            return true;
        }

        protected override void Press(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            so = (ActiveTormentedShadowSO)AssociatedActiveCapacitySO();
            if (Vector3.Distance(targetPositions[0], caster.transform.position) > so.maxRange) return;

            ApplyDamage();

            GameStateMachine.Instance.OnTick += PoolOfShadow;
            
            position = targetPositions[0];
            
            Debug.Log("Pressed");
        }

        protected override void PressFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            Debug.Log("Pressed Feedback");
        }

        protected override void Hold(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            Debug.Log("Hold");
        }

        protected override void HoldFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            Debug.Log("Hold Feedback");
        }

        protected override void Release(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            Debug.Log("Released");
        }

        protected override void ReleaseFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            Debug.Log("Released Feedback");
        }

        private void PoolOfShadow()
        {
            tickDamageTimer += GameStateMachine.Instance.tickRate;
            
            if (tickDamageTimer >= so.tickDamage)
            {
                ApplyDamage();
                tickDamageTimer = 0;
            }

            if (durationTimer >= so.activeDuration)
            {
                GameStateMachine.Instance.OnTick -= PoolOfShadow;
            }
        }

        private void ApplyDamage()
        {
            Collider[] detected = Physics.OverlapSphere(position, so.zoneRadius);

            foreach (var hit in detected)
            {
                Entity entityTouch = hit.GetComponent<Entity>();

                if (entityTouch)
                {
                    Debug.Log(entityTouch);
                    ITeamable entityTeam = entityTouch.GetComponent<ITeamable>();
                    ITeamable casterTeam = caster.GetComponent<ITeamable>();

                    if (entityTeam.GetTeam() != casterTeam.GetTeam())
                    {
                        IActiveLifeable entityActiveLifeable = entityTouch.GetComponent<IActiveLifeable>();

                        if(entityActiveLifeable != null) entityActiveLifeable.DecreaseCurrentHpRPC(so.damageAmount);
                    }
                }
            }
        }

        public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            Debug.Log("Cast tormented shadow");
            
        }
    }
}

