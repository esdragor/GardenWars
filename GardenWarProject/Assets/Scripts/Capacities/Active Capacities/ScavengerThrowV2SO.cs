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
        public ProjectileOnCollideEffect upgradeProjectile;
        
        public float projectileSpeed = 1.0f;
        public float height = 5.0f;
        public bool canBounce = false;
        public int nbBounce = 5;
        public float bounceRadius = 0.5f;
        public float chargeRate = 1f;
        public float minDistance = 5.0f;
        public float maxDistance = 5.0f;
        
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
        
        
        private double progress = 0f;
        private int nbBounce = 0;
        private float height = 0;
        private Vector3 dir = Vector3.zero;
        private double speed;
        private float timeForOneUnit;
        private float distanceToDestination;
        private Vector3 startPosition;
        private Vector3 endPosition;
        private bool isRandom = false;
        private float randomRangeRadius = 0f;
        private bool Finished = false;
        
        

        protected override bool AdditionalCastConditions(int targetsEntityIndexes, Vector3 targetPositions)
        {
            return !champion.isFighter && champion.upgrades > 0;
        }

        protected override void Press(int targetsEntityIndexes, Vector3 targetPositions)
        {
        }

        protected override void PressFeedback(int targetsEntityIndexes, Vector3 targetPositions)
        {
            
        }

        protected override void PressLocal(int targetsEntityIndexes, Vector3 targetPositions)
        {
            time_Pressed = 0;
            throwDistance = so.minDistance;
        }

        protected override void Hold(int targetsEntityIndexes, Vector3 targetPositions)
        {
            
        }

        protected override void HoldFeedback(int targetsEntityIndexes, Vector3 targetPositions)
        {
            time_Pressed += Time.deltaTime * so.chargeRate;
            throwDistance += time_Pressed;
            if (throwDistance < so.minDistance) throwDistance = so.minDistance;
            if (throwDistance > so.maxDistance) throwDistance = so.maxDistance;
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
            if (throwDistance > so.maxDistance) throwDistance = so.maxDistance;
            targetPosition = GetClosestValidPoint(casterPos + (targetPositions - casterPos).normalized * (float)throwDistance);
            targetPosition.y = 1;

            var projectile = LocalPoolManager.PoolInstantiate(so.upgradeProjectile, targetPosition, Quaternion.identity);
            var projectileTr = projectile.transform;

            Throw();

            time_Pressed = 0;
            throwDistance = so.minDistance;
            
            void Throw()
            {
                startPosition = champion.position;
                endPosition = targetPosition;
                speed = throwDistance;
                progress = 0;
                
                timeForOneUnit = so.projectileSpeed;
                height = so.height;
                isRandom = so.canBounce;
                randomRangeRadius = so.bounceRadius;
                nbBounce = so.nbBounce;
                
                bool canPickup = false;

                dir = (endPosition - startPosition).normalized;

                distanceToDestination = (Vector3.Distance(endPosition, startPosition));

                var timeToDestination = distanceToDestination / so.projectileSpeed;
                float timer = 0f;
                
                Debug.Log("Throw");

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
                                             Random.Range(-randomRangeRadius, randomRangeRadius) * ((isRandom) ? 1 : 0);
                            endPosition.z += (nbBounce * 0.5f) * dir.z + dir.z * (float)speed +
                                             Random.Range(-randomRangeRadius, randomRangeRadius) * ((isRandom) ? 1 : 0);

                            endPosition = GetClosestValidPoint(endPosition);
                            endPosition.y = 1;

                            distanceToDestination = Vector3.Distance(startPosition, endPosition);

                            timeToDestination = distanceToDestination / so.projectileSpeed;
                            
                            nbBounce--;
                            
                            Debug.Log($"Done bounce {nbBounce+1}");
                        }
                        else
                        {
                            gsm.OnUpdateFeedback -= MoveBag;
                            projectileTr.position = new Vector3(projectileTr.position.x, endPosition.y, projectileTr.position.z);
                            Finished = true;
                            Debug.Log("Done");
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