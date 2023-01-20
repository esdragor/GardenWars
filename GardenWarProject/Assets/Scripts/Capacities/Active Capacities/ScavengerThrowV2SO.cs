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
        public GameObject prefabJauge;
        public int nbBounce = 5;
        [Range(0.1f, 10f)] public float airSpeed = 1.0f;
        [Range(0.01f, 2f)] public float timeForOneUnit = 1.0f;
        public float height = 5.0f;
        public bool RandomizeRebound = false;
        public float RandomizeReboundRadius = 0.5f;
        public float HextechFlashSpeedScale = 1f;
        public float MinDistanceHFlash = 5.0f;
        public float MaxDistanceHFlash = 5.0f;
        public float accelerationJauge = 1f; //linear (not used lol)
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
        private double bagSpeed;

        private double acceleration = 0.1;
        private ParticleSystem ThrowDestFx;
        private ParticleSystem ThrowDestFxGO;
        
        
        private double progress = 0f;
        private int nbBounce = 0;
        private float height = 0;
        private float moveSpeed;
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
            time_Pressed = Time.time;
            bagSpeed = so.MinDistanceHFlash;
            acceleration = 0;
        }

        protected override void PressLocal(int targetsEntityIndexes, Vector3 targetPositions)
        {
        }

        protected override void Hold(int targetsEntityIndexes, Vector3 targetPositions)
        {
        }

        protected override void HoldFeedback(int targetsEntityIndexes, Vector3 targetPositions)
        {
            acceleration += (Time.time - time_Pressed) * so.accelerationJauge;
        }

        protected override void HoldLocal(int targetsEntityIndexes, Vector3 targetPositions)
        {
            time_Pressed = (Time.time - time_Pressed) * so.HextechFlashSpeedScale;
            bagSpeed += time_Pressed;
            if (bagSpeed < so.MinDistanceHFlash) bagSpeed = so.MinDistanceHFlash;
            if (bagSpeed > so.MaxDistanceHFlash) bagSpeed = so.MaxDistanceHFlash;
            
            champion.ShowSkillShotIndicator(PlayerInputController.CursorWorldPos, (float)bagSpeed);
        }

        protected override void Release(int targetsEntityIndexes, Vector3 targetPositions)
        {
            champion.DecreaseUpgradeCount();
        }

        protected override void ReleaseFeedback(int targetEntityIndex, Vector3 targetPositions)
        {
            time_Pressed = (Time.time - time_Pressed) * so.HextechFlashSpeedScale;
            bagSpeed += time_Pressed;
            if (bagSpeed > so.MaxDistanceHFlash) bagSpeed = so.MaxDistanceHFlash;
            targetPosition = GetClosestValidPoint(casterPos + (targetPositions - casterPos).normalized * (float)bagSpeed);
            targetPosition.y = 1;

            var projectile = LocalPoolManager.PoolInstantiate(so.upgradeProjectile, targetPosition, Quaternion.identity);
            var projectileTr = projectile.transform;

            Throw();
            
            void Throw()
            {
                startPosition = champion.position;
                endPosition = targetPosition;
                speed = bagSpeed;
                progress = 0;
                
                timeForOneUnit = so.timeForOneUnit;
                moveSpeed = so.airSpeed;
                height = so.height;
                isRandom = so.RandomizeRebound;
                randomRangeRadius = so.RandomizeReboundRadius;
                nbBounce = so.nbBounce;
                
                bool canPickup = false;

                dir = (endPosition - startPosition).normalized;

                distanceToDestination = (Vector3.Distance(endPosition, startPosition));

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
                            progress = 0f;
                            height /= 1.5f;
                            speed /= 4f;
                            startPosition = endPosition;

                            endPosition.x += (nbBounce * 0.5f) * dir.x + dir.x * (float)speed +
                                             Random.Range(-randomRangeRadius, randomRangeRadius) * ((isRandom) ? 1 : 0);
                            endPosition.z += (nbBounce * 0.5f) * dir.z + dir.z * (float)speed +
                                             Random.Range(-randomRangeRadius, randomRangeRadius) * ((isRandom) ? 1 : 0);

                            endPosition = GetClosestValidPoint(endPosition);
                            endPosition.y = 1;
                            nbBounce--;
                            moveSpeed *= 0.9f;
                        }
                        else
                        {
                            gsm.OnUpdateFeedback -= MoveBag;
                            projectileTr.position = new Vector3(projectileTr.position.x, endPosition.y, projectileTr.position.z);
                            Finished = true;
                            return;
                        }
                    }

                    progress += moveSpeed * (timeForOneUnit / distanceToDestination) * Time.deltaTime;
                    
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