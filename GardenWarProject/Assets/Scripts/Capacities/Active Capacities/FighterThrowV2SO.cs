using System;
using System.Collections;
using System.Collections.Generic;
using Controllers.Inputs;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/FighterThrowV2", fileName = "Throw Fighter V2")]
    public class FighterThrowV2SO : ActiveCapacitySO
    {
        [Header("Throw Config")]
        public ProjectileOnCollideEffect candyBagProjectile;
        public float projectileSpeed = 1.0f;
        public float height = 5.0f;
        public int nbBounce = 1;
        public float bounceRadius = 0.5f;
        public bool hasRandomBounceDirection = false;

        [Header("Candy")]
        public int minCandy = 5;
        public int maxCandy = 10;
        public float candyAccelerationRate = 1f;
        public int candyPerGroup = 5;
        public float scalePerCandyGroup = 0.33f;

        [Header("Damage")]
        public int damagePerCandyGroup = 1; // damage per candy group (exclude candy beneath trigger requierement)
        public int minimumCandyForDamage = 1; // min candy for damage

        public ParticleSystem ThrowDestFx;

        public override Type AssociatedType()
        {
            return typeof(FighterThrowV2);
        }
    }

    public class FighterThrowV2 : ActiveCapacity
    {
        private FighterThrowV2SO so => (FighterThrowV2SO) AssociatedActiveCapacitySO();
        
        private float time_Pressed = 0f;
        private int nbCandyStocked = 0;

        private double acceleration = 0.1;
        private float distanceCandy = 10f;
        private ParticleSystem ThrowDestFx;
        private ParticleSystem ThrowDestFxGO;


        protected override bool AdditionalCastConditions(int targetsEntityIndexes, Vector3 targetPositions)
        {
            return champion.isFighter && champion.currentCandy >= so.minCandy;
        }

        protected override void Press(int targetsEntityIndexes, Vector3 targetPositions)
        {
        }

        protected override void PressFeedback(int targetsEntityIndexes, Vector3 targetPositions)
        {
            time_Pressed = 0;
            nbCandyStocked = so.minCandy;
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
            acceleration += (Time.deltaTime) * so.candyAccelerationRate;

            nbCandyStocked = so.minCandy + Mathf.RoundToInt((float) acceleration);
            

            if (nbCandyStocked > so.maxCandy) nbCandyStocked = so.maxCandy;
            if (nbCandyStocked >= champion.currentCandy) nbCandyStocked = champion.currentCandy;

            Debug.Log($"Stocked Candy : {nbCandyStocked} (size : {0.5f + 1 * ((nbCandyStocked / so.candyPerGroup) * so.scalePerCandyGroup)})");
            
            champion.ShowAreaIndicator(PlayerInputController.CursorWorldPos, 0.5f * (0.5f + 1 * ((nbCandyStocked / so.candyPerGroup) * so.scalePerCandyGroup))) ;
        }

        protected override void HoldLocal(int targetsEntityIndexes, Vector3 targetPositions)
        {
        }

        protected override void Release(int targetsEntityIndexes, Vector3 targetPositions)
        {
        }

        protected override void ReleaseFeedback(int targetEntityIndex, Vector3 targetPositions)
        {
            champion.HideAreaIndicator();
            
            var size = Vector3.one * 0.5f + Vector3.one * ((nbCandyStocked / so.candyPerGroup) * so.scalePerCandyGroup);
            
            targetPositions = GetClosestValidPoint(targetPositions);
            targetPositions.y = champion.position.y;
            
            champion.RequestDecreaseCurrentCandy(nbCandyStocked);
            
            var projectile = LocalPoolManager.PoolInstantiate(so.candyBagProjectile, champion.position, Quaternion.identity);
            var projectileTr = projectile.transform;

            projectileTr.localScale = size;
            
            champion.PlayThrowAnimation();

            Throw();
            
            time_Pressed = 0;
            nbCandyStocked = so.minCandy;
            acceleration = 0;

            void Throw()
            {
                var thrownCandy = nbCandyStocked;
                var damage = nbCandyStocked - so.minimumCandyForDamage <= 0 ? 0 : ((nbCandyStocked - so.minimumCandyForDamage)/so.candyPerGroup) * so.damagePerCandyGroup;
                
                var startPosition = champion.position;
                var endPosition = targetPositions;
                var speed = so.projectileSpeed;
                float progress = 0;

                var height = so.height;
                var hasRandomBounceDirection = so.hasRandomBounceDirection;
                var randomRangeRadius = so.bounceRadius;
                var nbBounce = so.nbBounce;

                bool canPickup = false;
                bool removedDamage = false;

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
                        projectile.OnEntityCollide += GiveCandy;
                        if (damage > 0)
                        {
                            removedDamage = false;
                            projectile.OnEntityCollide += DealDamage;
                        }
                    }

                    if (progress > 0.99f)
                    {
                        if (nbBounce > 0)
                        {
                            if (!removedDamage)
                            {
                                projectile.OnEntityCollide -= DealDamage;
                                removedDamage = true;
                            }
                            
                            timer = 0f;
                            height /= 1.5f;
                            speed /= 4f;
                            startPosition = endPosition;

                            endPosition.x += (nbBounce * 0.5f) * dir.x + dir.x * (float) speed +
                                             Random.Range(-randomRangeRadius, randomRangeRadius) * ((hasRandomBounceDirection) ? 1 : 0);
                            endPosition.z += (nbBounce * 0.5f) * dir.z + dir.z * (float) speed +
                                             Random.Range(-randomRangeRadius, randomRangeRadius) * ((hasRandomBounceDirection) ? 1 : 0);

                            endPosition = GetClosestValidPoint(endPosition);
                            endPosition.y = champion.position.y;

                            distanceToDestination = Vector3.Distance(startPosition, endPosition);

                            timeToDestination = distanceToDestination / so.projectileSpeed;

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
                
                void GiveCandy(Entity entity)
                {
                    if (!(entity is Champion.Champion champ)) return;

                    champ.IncreaseCurrentCandyRPC(thrownCandy);
                    projectile.DestroyProjectile(true);
                }

                void DealDamage(Entity entity)
                {
                    if(entity is Tower) return;

                    var lifeable = entity.GetComponent<IActiveLifeable>();
                    lifeable?.DecreaseCurrentHpRPC(damage,champion.entityIndex);
                }
            }
        }

        protected override void ReleaseLocal(int targetEntityIndex, Vector3 targetPositions)
        {
        }
    }
}