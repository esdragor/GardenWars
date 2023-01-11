using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/ArcherVentouse", fileName = "new ArcherVentouse")]
    public class ArcherVentouseSO : ActiveCapacitySO
    {
        public ProjectileOnCollideEffect projectile;
        public float projectileSpeed = 1f;
        public float projectileDamage;
        public float dragDistance;
        public float dragTime;

        public override Type AssociatedType()
        {
            return typeof(ArcherVentouse);
        }
    }

    public class ArcherVentouse : ActiveCapacity
    {
        private ArcherVentouseSO so => (ArcherVentouseSO) AssociatedActiveCapacitySO();
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
            
            champion.LookAt(targetPositions);
            
            champion.SetAnimatorTrigger("Ability1");

            var projectileSpawnPos = casterPos + shotDirection * 0.5f;
            
            var projectile = Object.Instantiate(so.projectile,projectileSpawnPos,Quaternion.LookRotation(shotDirection));
            
            var targetPos = projectileSpawnPos + (shotDirection * so.maxRange);
            
            var projectileTr = projectile.transform;

            projectile.OnEntityCollide += EntityCollide;
            projectile.OnEntityCollideFeedback += EntityCollideFeedback;

            gsm.OnUpdateFeedback += MoveProjectile;
            
            void MoveProjectile()
            {
                projectileTr.position = Vector3.MoveTowards(projectileTr.position, targetPos, so.projectileSpeed * Time.deltaTime);
                if (Vector3.Distance(projectileTr.position, targetPos) <= 0.01f)
                {
                    gsm.OnUpdateFeedback -= MoveProjectile;
                    
                    projectile.DestroyProjectile();
                }
            }

            void DealDamage(Entity entity)
            {
                var lifeable = entity.GetComponent<IActiveLifeable>();

                lifeable?.DecreaseCurrentHpRPC(so.projectileDamage, caster.entityIndex);
            }
            
            void Displace(Entity entity)
            {
                var displaceable = entity.GetComponent<IDisplaceable>();
                
                displaceable?.DisplaceRPC(champion.position + champion.forward*so.dragDistance,so.dragTime);
            }
            
            void EntityCollide(Entity entity)
            {
                if (!caster.GetEnemyTeams().Contains(entity.team)) return;
                
                DealDamage(entity);
                
                Displace(entity);
                
                gsm.OnUpdateFeedback -= MoveProjectile;
                
                projectile.DestroyProjectile();
            }
            
            void EntityCollideFeedback(Entity entity)
            {
                if (!caster.GetEnemyTeams().Contains(entity.team)) return;

                gsm.OnUpdateFeedback -= MoveProjectile;
                
                projectile.DestroyProjectile();
            }
        }

        protected override void ReleaseLocal(int targetEntityIndex, Vector3 targetPositions)
        {
        }
    }
}


