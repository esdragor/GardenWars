using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/ArcherUlt", fileName = "new ArcherUlt")]
    public class ArcherUltSO : ActiveCapacitySO
    {
        public ProjectileOnCollideEffect projectile;

        public float explosionRadius;
        public float explosionFxMultiplier = 0.5f;

        public float projectileSpeed = 1f;
        public float projectileDamage;
        public ParticleSystem FXLaunch;
        public ParticleSystem FXBurst;

        public string SFXArcherUlt;

        public override Type AssociatedType()
        {
            return typeof(ArcherUlt);
        }
    }

    public class ArcherUlt : ActiveCapacity
    {
        private ArcherUltSO so => (ArcherUltSO)AssociatedActiveCapacitySO();
        private GameObject FXLaunchGo;
        private GameObject FXLaunchBurst;
        private Transform burstTr;
        
        public override void Init()
        {
            isUnusable = true;
            OnUsable?.Invoke(!isUnusable);
        }

        protected override void OnUpgrade()
        {
            isUnusable = false;
            OnUsable?.Invoke(!isUnusable);
        }

        protected override bool AdditionalCastConditions(int targetsEntityIndexes, Vector3 targetPositions)
        {
            return level > 1;
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
            champion.SetCanMoveRPC(false);
        }

        protected override void ReleaseFeedback(int targetEntityIndex, Vector3 targetPositions)
        {
            targetPositions.y = casterPos.y;
            var shotDirection = (targetPositions - casterPos).normalized;
            if (!FXLaunchGo)
            {
                FXLaunchGo = LocalPoolManager.PoolInstantiate(so.FXLaunch, champion.rotateParent.transform).gameObject;
                FXLaunchGo.transform.localPosition = new Vector3(0f, 1, shotDirection.z);
                FXLaunchGo.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
            }
            
            FMODUnity.RuntimeManager.PlayOneShot("event:/" + so.SFXArcherUlt, casterPos);

            if (!FXLaunchBurst)
            {
                FXLaunchBurst = LocalPoolManager.PoolInstantiate(so.FXBurst).gameObject;
                burstTr = FXLaunchBurst.transform;
            }

            FXLaunchBurst.SetActive(false);

            FXLaunchGo.SetActive(false);
            FXLaunchGo.SetActive(true);


            champion.LookAt(targetPositions);

            champion.SetAnimatorTrigger("Ability3");

            var projectileSpawnPos = casterPos + shotDirection * 0.5f;

            var projectile =
                LocalPoolManager.PoolInstantiate(so.projectile, projectileSpawnPos,
                    Quaternion.LookRotation(shotDirection));
            projectile.gameObject.SetActive(false);

            var targetPos = projectileSpawnPos + (shotDirection * so.maxRange);

            var projectileTr = projectile.transform;

            async void LaunchMoveRocket()
            {
                await Task.Delay(700);
                if (isMaster) champion.SetCanMoveRPC(true);
                projectile.transform.position = shotDirection + champion.transform.position;
                projectile.gameObject.SetActive(true);
                gsm.OnUpdateFeedback += MoveProjectile;
            }

            LaunchMoveRocket();

            projectile.OnEntityCollide += EntityCollide;
            projectile.OnEntityCollideFeedback += EntityCollideFeedback;

            void MoveProjectile()
            {
                projectileTr.position =
                    Vector3.MoveTowards(projectileTr.position, targetPos, so.projectileSpeed * Time.deltaTime);
                if (Vector3.Distance(projectileTr.position, targetPos) <= 0.01f)
                {
                    gsm.OnUpdateFeedback -= MoveProjectile;

                    burstTr.position = targetPos;
                    burstTr.localScale = Vector3.one;

                    FXLaunchBurst.SetActive(true);

                    projectile.DestroyProjectile(true);
                }
            }

            void DealDamage(Entity entity)
            {
                if (!entity) return;
                var lifeable = entity.GetComponent<IActiveLifeable>();
                lifeable?.DecreaseCurrentHpRPC(so.projectileDamage, caster.entityIndex);

                gsm.OnUpdateFeedback -= MoveProjectile;

                projectile.DestroyProjectile(true);

                burstTr.position = entity.position;
                burstTr.localScale =
                    (level < 3) ? Vector3.one : Vector3.one * so.explosionRadius * so.explosionFxMultiplier;
                ;

                FXLaunchBurst.SetActive(true);

                if (level < 3) return;

                var hitColliders = Physics.OverlapSphere(entity.position, so.explosionRadius);
                foreach (var hitCollider in hitColliders)
                {
                    var exploEnt = hitCollider.GetComponent<Entity>();
                    ExplosionDamage(exploEnt);
                }

                void ExplosionDamage(Entity exploEntity)
                {
                    if (exploEntity == null) return;
                    if (exploEntity == entity) return;
                    if (!champion.GetEnemyTeams().Contains(exploEntity.team)) return;

                    var exploLifeable = exploEntity.GetComponent<IActiveLifeable>();
                    exploLifeable?.DecreaseCurrentHpRPC(so.projectileDamage, caster.entityIndex);
                }
            }


            void EntityCollide(Entity entity)
            {
                if (!caster.GetEnemyTeams().Contains(entity.team)) return;

                DealDamage(entity);

                gsm.OnUpdateFeedback -= MoveProjectile;
            }

            void EntityCollideFeedback(Entity entity)
            {
                if (!caster.GetEnemyTeams().Contains(entity.team)) return;

                if (!FXLaunchBurst)
                {
                    FXLaunchBurst = LocalPoolManager.PoolInstantiate(so.FXBurst, champion.championMesh.transform)
                        .gameObject;
                }

                FXLaunchBurst.SetActive(false);
                FXLaunchBurst.SetActive(true);
                gsm.OnUpdateFeedback -= MoveProjectile;

                projectile.DestroyProjectile(true);
            }
        }

        protected override void ReleaseLocal(int targetEntityIndex, Vector3 targetPositions)
        {
        }
    }
}