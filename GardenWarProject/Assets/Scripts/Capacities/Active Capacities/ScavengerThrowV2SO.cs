using System;
using System.Collections;
using System.Collections.Generic;
using Controllers.Inputs;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/ScavengerThrowV2", fileName = "ScavengerThrowV2")]
    public class ScavengerThrowV2SO : ActiveCapacitySO
    {
        [Header("Throw Config")]
        public ProjectileOnCollideEffect upgradeProjectile;
        
        public float projectileSpeed = 1.0f;
        public float height = 5.0f;
        public int nbBounce = 5;
        public float bounceRadius = 0.5f;
        public bool hasRandomBounceDirection = false;
        public float chargeRate = 1f;
        public float minDistance = 5.0f;

        public ParticleSystem ThrowDestFx;


        public override Type AssociatedType()
        {
            return typeof(ScavengerThrowV2);
        }
    }

    public class ScavengerThrowV2 : ActiveCapacity
    {
        private ScavengerThrowV2SO so => (ScavengerThrowV2SO)AssociatedActiveCapacitySO();

        private Vector3 targetPosition;

        private float time_Pressed = 0f;
        private double throwDistance;
        
        private ParticleSystem ThrowDestFx;
        private ParticleSystem ThrowDestFxGO;


        protected override bool AdditionalCastConditions(int targetsEntityIndexes, Vector3 targetPositions)
        {
            return !champion.isFighter && champion.upgrades > 0;
        }

        protected override void Press(int targetsEntityIndexes, Vector3 targetPositions)
        {
        }

        protected override void PressFeedback(int targetsEntityIndexes, Vector3 targetPositions)
        {
            time_Pressed = 0;
            throwDistance = so.minDistance;
        }

        protected override void PressLocal(int targetsEntityIndexes, Vector3 targetPositions)
        {
            
        }

        protected override void Hold(int targetsEntityIndexes, Vector3 targetPositions)
        {
            
        }

        protected override void HoldFeedback(int targetsEntityIndexes, Vector3 targetPositions)
        {
            time_Pressed += Time.deltaTime * so.chargeRate;
            throwDistance += time_Pressed;
            if (throwDistance < so.minDistance) throwDistance = so.minDistance;
        }

        protected override void HoldLocal(int targetsEntityIndexes, Vector3 targetPositions)
        {
            champion.ShowSkillShotIndicator(PlayerInputController.CursorWorldPos, (float)throwDistance);
        }

        protected override void Release(int targetsEntityIndexes, Vector3 targetPositions)
        {
            champion.DecreaseUpgradeCount();
        }

        protected override void ReleaseFeedback(int targetEntityIndex, Vector3 targetPositions)
        {
            targetPosition = GetClosestValidPoint(casterPos + (targetPositions - casterPos).normalized * (float)throwDistance);
            targetPosition.y = 1;

            var projectile = LocalPoolManager.PoolInstantiate(so.upgradeProjectile, champion.position, Quaternion.identity);
            var projectileTr = projectile.transform;
            
            champion.PlayThrowAnimation();
            
            Throw();

            time_Pressed = 0;
            throwDistance = so.minDistance;
            
            void Throw()
            {
                var startPosition = champion.position;
                var endPosition = targetPosition;
                var speed = so.projectileSpeed;
                float progress = 0;
                
                var height = so.height;
                var hasRandomBounceDirection = so.hasRandomBounceDirection;
                var randomRangeRadius = so.bounceRadius;
                var nbBounce = so.nbBounce;
                
                bool canPickup = false;

                var dir = (endPosition - startPosition).normalized;

                var distanceToDestination = (Vector3.Distance(endPosition, startPosition));

                var timeToDestination = distanceToDestination / so.projectileSpeed;
                float timer = 0f;
                
                gsm.OnUpdateFeedback += MoveBag;

                void MoveBag()
                {
                    if (progress > 0.5f && !canPickup)
                    {
                        canPickup = true;
                        projectile.OnEntityCollide += GiveUpgrade;
                    } 
                    
                    if (progress > 0.99f)
                    {
                        if (nbBounce > 0)
                        {
                            timer = 0f;
                            height /= 1.5f;
                            speed /= 4f;
                            startPosition = endPosition;

                            endPosition.x += (nbBounce * 0.5f) * dir.x + dir.x * (float)speed +
                                             Random.Range(-randomRangeRadius, randomRangeRadius) * ((hasRandomBounceDirection) ? 1 : 0);
                            endPosition.z += (nbBounce * 0.5f) * dir.z + dir.z * (float)speed +
                                             Random.Range(-randomRangeRadius, randomRangeRadius) * ((hasRandomBounceDirection) ? 1 : 0);

                            endPosition = GetClosestValidPoint(endPosition);
                            endPosition.y = 1;

                            distanceToDestination = Vector3.Distance(startPosition, endPosition);

                            timeToDestination = distanceToDestination / (so.projectileSpeed * 0.5f);
                            
                            nbBounce--;
                        }
                        else
                        {
                            gsm.OnUpdateFeedback -= MoveBag;
                            projectileTr.position = new Vector3(projectileTr.position.x, endPosition.y, projectileTr.position.z);
                            return;
                        }
                    }

                    timer += Time.deltaTime;
                    progress = (timer / timeToDestination);
                    
                    projectileTr.position = ParabolaClass.Parabola(startPosition, endPosition, height, progress);
                }
            }
            
            void GiveUpgrade(Entity entity)
            {
                if (!(entity is Champion.Champion champ)) return;

                champ.IncreaseUpgradeCount();
                projectile.DestroyProjectile(true);
            }
        }

        protected override void ReleaseLocal(int targetEntityIndex, Vector3 targetPositions)
        {
        }
    }
}