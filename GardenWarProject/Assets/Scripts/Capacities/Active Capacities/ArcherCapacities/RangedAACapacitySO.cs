using System;
using GameStates;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/Ranged Auto Attack", fileName = "new Ranged Auto Attack")]
    public class RangedAACapacitySO : ActiveCapacitySO
    {
        public ProjectileOnCollideEffect projectileBlue;
        public ProjectileOnCollideEffect projectileRed;
        public ProjectileOnCollideEffect defaultProjectile;
        public float speedFire = 0.1f;
        
        public string SFXName;
        
        public override Type AssociatedType()
        {
            return typeof(RangedAACapacity);
        }
    }
    
    public class RangedAACapacity : ActiveCapacity
    {
        private RangedAACapacitySO so => (RangedAACapacitySO) AssociatedActiveCapacitySO();
        private IDeadable deadableEntity;

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

        }

        protected override void ReleaseFeedback(int targetEntityIndex, Vector3 targetPositions)
        {
            var target = EntityCollectionManager.GetEntityByIndex(targetEntityIndex);
            
            var rotation = casterTr.localRotation;

            var spawnPos = casterPos + casterTr.forward;

            if (caster is Champion.Champion castChamp)
            {
                castChamp.LookAt(target.position);
                
                castChamp.SetAnimatorTrigger("Basic Attack");
                
                spawnPos = casterPos + castChamp.forward * 0.5f;
                rotation = Quaternion.LookRotation(castChamp.forward);
            }

            ProjectileOnCollideEffect Arrow = (so.projectileRed && so.projectileBlue) ?
             (caster.GetEnemyTeams().Contains(GameStateMachine.Instance.GetPlayerTeam()) ? so.projectileRed : so.projectileBlue) : so.defaultProjectile;
            
            var projectile = LocalPoolManager.PoolInstantiate(Arrow,spawnPos,rotation);

            var projectileTr = projectile.transform;
            
            float damage = 0;
            
            if (so.SFXName != "")
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/" + so.SFXName, casterPos);
            }
            
            var attackable = caster.GetComponent<IAttackable>();
            if (attackable != null)
            {
                damage = attackable.GetAttackDamage();
            }
            
            if (damage == 0) return;

            gsm.OnUpdateFeedback += MoveProjectile;
            
            void MoveProjectile()
            {
                if (!deadableEntity.IsAlive())
                {
                   RemoveProjectile();
                    
                   return;
                }

                var targetPos = targetedEntity.transform.position;
                
                projectileTr.position = Vector3.MoveTowards(projectileTr.position,targetPos, so.speedFire);
                
                projectileTr.LookAt(targetPos);

                if (Vector3.Distance(projectileTr.position, targetPos) <= 0.05f)
                {
                    RemoveProjectile();
                    
                    DealDamage(target);
                }
            }

            void DealDamage(Entity entity)
            {
                if (!entity) return;
                var lifeable = entity.GetComponent<IActiveLifeable>();
                lifeable.DecreaseCurrentHpRPC(damage, caster.entityIndex);
            }

            void RemoveProjectile()
            {
                gsm.OnUpdateFeedback -= MoveProjectile;
                    
                projectile.DestroyProjectile(true);
            }
        }

        protected override void ReleaseLocal(int targetEntityIndex, Vector3 targetPositions)
        {
        }
    }

}
