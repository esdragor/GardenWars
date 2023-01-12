using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/ArcherUlt", fileName = "new ArcherUlt")]
    public class ArcherUltSO : ActiveCapacitySO
    {
        public ProjectileOnCollideEffect projectile;
        public float projectileSpeed = 1f;
        public float projectileDamage;
        public ParticleSystem FXLaunch;
        public ParticleSystem FXBurst;


        public override Type AssociatedType()
        {
            return typeof(ArcherUlt);
        }
    }

    public class ArcherUlt : ActiveCapacity
    {
        private ArcherUltSO so => (ArcherUltSO) AssociatedActiveCapacitySO();
        private GameObject FXLaunchGo;
        private GameObject FXLaunchBurst;

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
            champion.SetCanMoveRPC(false);
        }

        protected override void ReleaseFeedback(int targetEntityIndex, Vector3 targetPositions)
        {
            targetPositions.y = casterPos.y;
            if (!FXLaunchGo)
            {
                FXLaunchGo = Object.Instantiate(so.FXLaunch, champion.rotateParent.transform).gameObject;
                FXLaunchGo.transform.localPosition = Vector3.up;
            }
            FXLaunchGo.SetActive(false);
            FXLaunchGo.SetActive(true);

            var shotDirection = (targetPositions - casterPos).normalized;
            
            champion.LookAt(targetPositions);

            champion.SetAnimatorTrigger("Ability3");

            var projectileSpawnPos = casterPos + shotDirection * 0.5f;

            var projectile =
                Object.Instantiate(so.projectile, projectileSpawnPos, Quaternion.LookRotation(shotDirection));
            projectile.gameObject.SetActive(false);

            var targetPos = projectileSpawnPos + (shotDirection * so.maxRange);

            var projectileTr = projectile.transform;

            async void LaunchMoveRocket()
            {
                await Task.Delay(700);
                if(isMaster) champion.SetCanMoveRPC(true);
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
                    if (!FXLaunchBurst)
                    {
                        FXLaunchBurst = Object.Instantiate(so.FXBurst, targetPos, quaternion.identity).gameObject;
                    }
                    else
                    {
                        FXLaunchBurst.transform.position = targetPos;
                    }

                    FXLaunchBurst.SetActive(false);
                    FXLaunchBurst.SetActive(true);
                    projectile.DestroyProjectile();
                }
            }

            void DealDamage(Entity entity)
            {
                if (!entity) return;
                var lifeable = entity.GetComponent<IActiveLifeable>();

                lifeable.DecreaseCurrentHpRPC(so.projectileDamage, caster.entityIndex);
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

                if (!FXLaunchBurst)
                {
                    FXLaunchBurst = Object.Instantiate(so.FXBurst, champion.championMesh.transform).gameObject;
                }

                FXLaunchBurst.SetActive(false);
                FXLaunchBurst.SetActive(true);
                gsm.OnUpdateFeedback -= MoveProjectile;

                projectile.DestroyProjectile();
            }
        }

        protected override void ReleaseLocal(int targetEntityIndex, Vector3 targetPositions)
        {
        }
    }
}