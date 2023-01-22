using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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
        public Vector2 offset;
        public int textSize = 100;

        [Header("Damage")]
        public int damagePerCandyGroup = 1;

        public int baseDamage = 20;
        public int minimumCandyForDamage = 1;

        public ParticleSystem ThrowDestFx;

        public string SFXCataLaunch;
        public string dropCandySFX;

        public override Type AssociatedType()
        {
            return typeof(FighterThrowV2);
        }
    }

    public class FighterThrowV2 : ActiveCapacity
    {
        private FighterThrowV2SO so => (FighterThrowV2SO)AssociatedActiveCapacitySO();

        private float time_Pressed = 0f;
        private int nbCandyStocked = 0;

        private double acceleration = 0.1;
        private float distanceCandy = 10f;

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

            nbCandyStocked = so.minCandy + Mathf.RoundToInt((float)acceleration);


            if (nbCandyStocked > so.maxCandy) nbCandyStocked = so.maxCandy;
            if (nbCandyStocked >= champion.currentCandy) nbCandyStocked = champion.currentCandy;
        }

        protected override void HoldLocal(int targetsEntityIndexes, Vector3 targetPositions)
        {
            champion.ShowAreaIndicator(PlayerInputController.CursorWorldPos, 0.5f * (0.5f + 1 * ((nbCandyStocked / so.candyPerGroup) * so.scalePerCandyGroup)));
            champion.ShowTextIndicator(PlayerInputController.mousePos+so.offset,$"<size={so.textSize}%>x{nbCandyStocked}");
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

            FMODUnity.RuntimeManager.PlayOneShot("event:/" + so.SFXCataLaunch, champion.position);

            
            projectileTr.localScale = size;

            champion.PlayThrowAnimation();

            var destinationFX = LocalPoolManager.PoolInstantiate(so.ThrowDestFx, targetPositions, Quaternion.identity);
            var destionFXTR = destinationFX.transform;
            destionFXTR.Rotate(Vector3.right * 90);
            destionFXTR.localScale = size;
            
            FMODUnity.RuntimeManager.PlayOneShot("event:/" + so.dropCandySFX, projectile.transform.position);

            Throw();

            time_Pressed = 0;
            nbCandyStocked = so.minCandy;
            acceleration = 0;
            
            void Throw()
            {
                var thrownCandy = nbCandyStocked;
                var damage = so.baseDamage + ((nbCandyStocked - so.minimumCandyForDamage) / so.candyPerGroup) *
                    so.damagePerCandyGroup;

                var startPosition = champion.position;
                var endPosition = targetPositions;
                var speed = so.projectileSpeed;
                float progress = 0;

                var height = so.height;
                var hasRandomBounceDirection = so.hasRandomBounceDirection;
                var randomRangeRadius = so.bounceRadius;
                var nbBounce = so.nbBounce;

                bool canPickup = false;
                bool firstBounce = false;

                var dir = (endPosition - startPosition).normalized;

                var distanceToDestination = (Vector3.Distance(endPosition, startPosition));

                var timeToDestination = distanceToDestination / so.projectileSpeed;
                float timer = 0f;

                var fxGo = destinationFX.gameObject;


                gsm.OnUpdateFeedback += MoveBag;

                void MoveBag()
                {
                    if (progress > 0.5f && !canPickup)
                    {
                        canPickup = true;

                        projectile.OnEntityCollideFeedback += GiveCandy;
                        projectile.OnEntityCollideFeedback += DealDamage;
                    }

                    if (progress > 0.99f)
                    {
                        if (nbBounce > 0)
                        {
                            if (!firstBounce)
                            {
                                projectile.OnEntityCollideFeedback -= DealDamage;

                                destinationFX.gameObject.SetActive(false);

                                firstBounce = true;
                            }

                            timer = 0f;
                            height /= 1.5f;
                            speed /= 4f;
                            startPosition = endPosition;

                            endPosition.x += (nbBounce * 0.5f) * dir.x + dir.x * (float)speed +
                                             Random.Range(-randomRangeRadius, randomRangeRadius) *
                                             ((hasRandomBounceDirection) ? 1 : 0);
                            endPosition.z += (nbBounce * 0.5f) * dir.z + dir.z * (float)speed +
                                             Random.Range(-randomRangeRadius, randomRangeRadius) *
                                             ((hasRandomBounceDirection) ? 1 : 0);

                            endPosition = GetClosestValidPoint(endPosition);
                            endPosition.y = champion.position.y;

                            distanceToDestination = Vector3.Distance(startPosition, endPosition);

                            timeToDestination = distanceToDestination / so.projectileSpeed;

                            nbBounce--;
                        }
                        else
                        {
                            gsm.OnUpdateFeedback -= MoveBag;
                            
                            projectileTr.position = new Vector3(projectileTr.position.x, endPosition.y,
                                projectileTr.position.z);
                            if(canPickup) projectile.OnEntityCollideFeedback -= DealDamage;
                            fxGo.SetActive(false);
                            
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

                    if (isMaster) champ.IncreaseCurrentCandyRPC(thrownCandy);

                    projectile.OnEntityCollideFeedback -= GiveCandy;
                    
                    projectile.DestroyProjectile(true);
                }

                void DealDamage(Entity entity)
                {
                    if (thrownCandy < so.minimumCandyForDamage || !isMaster) return;

                    if (entity is Tower) return;

                    var lifeable = entity.GetComponent<IActiveLifeable>();
                    lifeable?.DecreaseCurrentHpRPC(damage, champion.entityIndex);
                }
            }
        }

        protected override void ReleaseLocal(int targetEntityIndex, Vector3 targetPositions)
        {
        }
    }
}