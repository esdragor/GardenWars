using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/SkillShot", fileName = "new SkillShot")]
    public class SkillShotSO : ActiveCapacitySO
    {
        public ProjectileOnCollideEffect projectile;
        public float projectileSpeed = 1f;
        public float projectileDamage;

        public override Type AssociatedType()
        {
            return typeof(SkillShot);
        }
    }
    
    public class SkillShot : ActiveCapacity
    {
        private SkillShotSO so => (SkillShotSO) AssociatedActiveCapacitySO();
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
            targetPositions.y = casterPos.y;
            
            var shotDirection = (targetPositions - casterPos).normalized;

            var projectileSpawnPos = casterPos + shotDirection * 0.5f;
            
            var projectile = Object.Instantiate(so.projectile,projectileSpawnPos,casterTr.rotation);
            
            var targetPos = projectileSpawnPos + (shotDirection * so.maxRange);
            
            var projectileTr = projectile.transform;

            projectile.OnEntityCollide += EntityCollide;
            projectile.OnEntityCollideFeedback += (entity) => gsm.OnUpdateFeedback -= MoveProjectile;

            gsm.OnUpdateFeedback += MoveProjectile;
            
            void MoveProjectile()
            {
                projectileTr.position = Vector3.MoveTowards(projectileTr.position, targetPos, so.projectileSpeed * Time.deltaTime);
                if (Vector3.Distance(projectileTr.position, targetPos) <= 0.01f)
                {
                    Debug.Log("MaxRange, Destroying projectile");
                    
                    gsm.OnUpdateFeedback -= MoveProjectile;
                    
                    projectile.DestroyProjectile();
                    
                }
            }

            void DealDamage(Entity entity)
            {
                var lifeable = entity.GetComponent<IActiveLifeable>();
                lifeable?.DecreaseCurrentHpRPC(so.projectileDamage, caster.entityIndex);
            }
            
            void EntityCollide(Entity entity)
            {
                if(!caster.GetEnemyTeams().Contains(entity.team)) return;
                
                DealDamage(entity);
                
                gsm.OnUpdateFeedback -= MoveProjectile;
                
                projectile.DestroyProjectile();
            }
        }

        protected override void ReleaseLocal(int targetEntityIndex, Vector3 targetPositions)
        {
        }
    }
}


