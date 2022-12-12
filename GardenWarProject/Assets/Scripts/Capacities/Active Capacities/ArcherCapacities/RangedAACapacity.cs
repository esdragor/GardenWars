using UnityEngine;

namespace Entities.Capacities
{
    public class RangedAACapacity : ActiveCapacity
    {
        private RangedAACapacitySO so => (RangedAACapacitySO) AssociatedActiveCapacitySO();

        protected override bool AdditionalCastConditions(int targetsEntityIndexes, Vector3 targetPositions)
        {
            if (targetedEntity.GetComponent<IActiveLifeable>() == null) return false;
            var deadable = targetedEntity.GetComponent<IDeadable>();

            if (deadable != null)
            {
                if (!deadable.IsAlive())
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
            var projectile = Object.Instantiate(so.projectile,casterPos+caster.transform.forward,caster.transform.localRotation);
            projectile.Init(caster, EntityCollectionManager.GetEntityByIndex(targetEntityIndex));
            var projectileTr = projectile.transform;
            float damage = 0;
            var attackable = caster.GetComponent<IAttackable>();
            if (attackable != null)
            {
                damage = attackable.GetAttackDamage();
            }
            
            if (damage == 0) return;
                
            projectile.OnEntityCollide += DealDamage;
            projectile.OnEntityCollideFeedback += (entity) => gsm.OnUpdateFeedback -= MoveProjectile;

            gsm.OnUpdateFeedback += MoveProjectile;
            
            void MoveProjectile()
            {
                projectileTr.position = Vector3.MoveTowards(projectileTr.position, targetedEntity.transform.position + Vector3.up, 0.1f);
                projectileTr.LookAt(targetedEntity.transform);
            }

            void DealDamage(Entity entity)
            {
                if (!entity) return;
                var lifeable = entity.GetComponent<IActiveLifeable>();
                lifeable.DecreaseCurrentHpRPC(damage);
            }
        }
    }
}


