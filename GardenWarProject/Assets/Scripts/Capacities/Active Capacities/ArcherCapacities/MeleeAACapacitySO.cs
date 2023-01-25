using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/Melee Auto Attack", fileName = "new Melee Auto Attack")]
    public class MeleeAACapacitySO : ActiveCapacitySO
    {
        public double timeUntilAttack;
        public ParticleSystem FXAttack;

        public string SFX;

        public override Type AssociatedType()
        {
            return typeof(MeleeAACapacity);
        }
    }

    public class MeleeAACapacity : ActiveCapacity
    {
        private MeleeAACapacitySO so => (MeleeAACapacitySO)AssociatedActiveCapacitySO();
        private IDeadable deadableEntity;
        public GameObject FXAttack;

        protected override bool AdditionalCastConditions(int targetsEntityIndexes, Vector3 targetPositions)
        {
            if (targetedEntity.GetComponent<IActiveLifeable>() == null) return false;
            deadableEntity = targetedEntity.GetComponent<IDeadable>();

            if (deadableEntity != null)
            {
                if (!deadableEntity.IsAlive())
                {
                    return false;
                }
            }

            if (!caster.GetEnemyTeams().Contains(targetedEntity.team))
            {
                return false;
            }

            return true;
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
            champion.LookAt(targetedEntity.position);
            champion.SetAnimatorTrigger("Basic Attack");

            var timer = so.timeUntilAttack;
            if (so.FXAttack)
            {
                if (!FXAttack)
                {
                    FXAttack = LocalPoolManager.PoolInstantiate(so.FXAttack, champion.championMesh.transform)
                        .gameObject;
                }

                FXAttack.SetActive(false);
                FXAttack.SetActive(true);
                FXAttack.transform.position = champion.championMesh.transform.position + champion.forward;
                if (so.SFX != null)
                    FMODUnity.RuntimeManager.PlayOneShot("event:/" + so.SFX, champion.position);
            }

            gsm.OnUpdate += IncreaseTimer;

            void IncreaseTimer()
            {
                timer -= Time.deltaTime;
                if (timer > 0) return;

                targetedEntity.GetComponent<IActiveLifeable>()
                    ?.DecreaseCurrentHpRPC(champion.attackDamage, caster.entityIndex);

                gsm.OnUpdate -= IncreaseTimer;
            }
        }

        protected override void ReleaseFeedback(int targetEntityIndex, Vector3 targetPositions)
        {
            //TODO - Play Animation
        }

        protected override void ReleaseLocal(int targetEntityIndex, Vector3 targetPositions)
        {
        }
    }
}