using System;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/SharkSkillShot", fileName = "new Shark SkillShot")]
    public class SharkShotSO : ActiveCapacitySO
    {
        public ProjectileOnCollideEffect projectile;
        public ShieldPassiveSO shieldPassiveSo;
        public float projectileSpeed = 1f;
        public float projectileDamage;
        public ParticleSystem FXhit;

        public override Type AssociatedType()
        {
            return typeof(SharkShot);
        }
    }
    
    public class SharkShot : ActiveCapacity
    {
        private SharkShotSO so => (SharkShotSO) AssociatedActiveCapacitySO();
        private GameObject FXHitGo;
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
            
            var projectile = LocalPoolManager.PoolInstantiate(so.projectile,projectileSpawnPos, Quaternion.LookRotation(shotDirection));
            projectile.gameObject.GetComponent<SharkShotManager>().EnableFXShot(champion.team);
            
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
                    if (!FXHitGo)
                    {
                        FXHitGo = LocalPoolManager.PoolInstantiate(so.FXhit, targetPos, quaternion.identity).gameObject;
                    }
                    else
                    {
                        FXHitGo.transform.position = targetPos;
                    }
                    FXHitGo.SetActive(false);
                    FXHitGo.SetActive(true);
                    projectile.DestroyProjectile(true);
                }
            }

            void DealDamage(Entity entity)
            {
                var lifeable = entity.GetComponent<IActiveLifeable>();
                
                caster.AddPassiveCapacityRPC(so.shieldPassiveSo.indexInCollection);

                lifeable?.DecreaseCurrentHpRPC(so.projectileDamage, caster.entityIndex);
            }
            
            void EntityCollide(Entity entity)
            {
                if (!caster.GetEnemyTeams().Contains(entity.team)) return;
                
                DealDamage(entity);
                
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


