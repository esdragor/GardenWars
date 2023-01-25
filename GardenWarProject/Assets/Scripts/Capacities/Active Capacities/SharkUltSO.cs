using System;
using System.Collections;
using System.Collections.Generic;
using Controllers.Inputs;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/SharkUlt", fileName = "new Shark Ult")]
    public class SharkUltSO : ActiveCapacitySO
    {
        [Header("Values")]
        public float animationDuration = 1f;
        public float impactZoneAppearance = 0.9f;
        public float sizeMultiplier = 1.5f;

        public float borrowDuration = 1f;
        
        public float damage = 50f;

        [Header("Fx")]
        public ParticleSystem leapStart;
        public ParticleSystem leapStartR;
        public ParticleSystem leapLand;
        public ParticleSystem leapLandR;
        public ParticleSystem leapIndicator;
        public ParticleSystem leapIndicatorR;

        public ProjectileOnCollideEffect impactArea;
        public StunPassiveSO stun;

        

        public override Type AssociatedType()
        {
            return typeof(SharkUlt);
        }
    }

    public class SharkUlt : ActiveCapacity
    {
        private SharkUltSO so => (SharkUltSO) AssociatedActiveCapacitySO();
        private float timer;
        private GameObject damageAreaGo;
        
        private GameObject leapStartGo;
        private GameObject leapEndGo;
        private GameObject leapMarkerGo;
        
        private ProjectileOnCollideEffect damageArea;
        
        private SharkPassive passive;
        private SharkPassive sharkPassive => passive ??= champion.GetPassiveCapacity<SharkPassive>();
        
        private bool ultBorrowed;

        protected override bool AdditionalCastConditions(int targetsEntityIndexes, Vector3 targetPositions)
        {
            return sharkPassive.borrowed;
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
            if (Vector3.Distance(targetPositions, casterPos) > so.maxRange)
            {
                var shotDirection = (targetPositions - casterPos).normalized;
                targetPositions = casterPos + shotDirection * so.maxRange;
            }
                
            champion.ShowAreaIndicator(GetClosestValidPoint(targetPositions),2);
            
            champion.ShowMaxRangeIndicator(so.maxRange);
        }

        protected override void Release(int targetsEntityIndexes, Vector3 targetPositions)
        {
            
        }

        protected override void ReleaseFeedback(int targetEntityIndex, Vector3 targetPositions)
        {
            InstantiateFx();

            if (ultBorrowed)
            {
                ExitUltBorrow();
                return;
            }
            
            if (isMaster)
            {
                champion.SetCanAttackRPC(false);
                champion.SetCanCastRPC(false);
                champion.SetCanMoveRPC(false);
            }
            
            Leap(targetPositions);

            timer = 0;

            ultBorrowed = false;

            gsm.OnUpdateFeedback += IncreaseTimerPart1;

            void IncreaseTimerPart1()
            {
                timer += Time.deltaTime;
                
                if (timer / so.animationDuration <= so.impactZoneAppearance) return;

                if (!damageAreaGo.activeSelf)
                {
                    if (champion.isVisible)
                    {
                        leapEndGo.transform.position = champion.position;
                        leapEndGo.SetActive(true);
                    }
                    
                    damageAreaGo.SetActive(true);
                    damageArea.OnEntityCollide += DamageAndStun;
                }

                if (timer <= so.animationDuration) return;

                timer = 0;

                damageAreaGo.SetActive(false);
                
                UltBorrow();
                    
                gsm.OnUpdateFeedback -= IncreaseTimerPart1;
                gsm.OnUpdateFeedback += IncreaseTimerPart2;
            }

            void UltBorrow()
            {
                if (isMaster)
                {
                    //champion.SetCanCastRPC(true);
                    champion.SetCanMoveRPC(true);
                }
                
                leapMarkerGo.SetActive(false);
                leapStartGo.SetActive(false);

                sharkPassive.Borrow(true);
                
                ultBorrowed = true;
            }

            void IncreaseTimerPart2()
            {
                timer += Time.deltaTime;

                var pos = PlayerInputController.CursorWorldPos;
                if (Vector3.Distance(pos, casterPos) > so.maxRange)
                {
                    var shotDirection = (pos - casterPos).normalized;
                    pos = casterPos + shotDirection * so.maxRange;
                }

                if (champion.isLocal)
                {
                    champion.ShowAreaIndicator(GetClosestValidPoint(pos),2);
                    champion.ShowMaxRangeIndicator(so.maxRange);
                }
                
                if(timer <= so.borrowDuration) return;
                
                
                if (champion.isLocal)
                {
                    champion.HideAreaIndicator();
                    champion.HideMaxRangeIndicator();
                }
                
                ExitUltBorrow();
            }

            void ExitUltBorrow()
            {
                if (isMaster)
                {
                    champion.SetCanAttackRPC(false);
                    champion.SetCanCastRPC(false);
                    champion.SetCanMoveRPC(false);
                }
                
                Leap(PlayerInputController.CursorWorldPos);

                timer = 0;
                
                ultBorrowed = false;
                
                gsm.OnUpdateFeedback -= IncreaseTimerPart2;
                gsm.OnUpdateFeedback += IncreaseTimerPart3;
            }

            void IncreaseTimerPart3()
            {
                timer += Time.deltaTime;
                    
                if (timer / so.animationDuration <= so.impactZoneAppearance) return;

                if (!damageAreaGo.activeSelf)
                {
                    if (champion.isVisible)
                    {
                        leapEndGo.SetActive(false);
                        leapEndGo.transform.position = champion.position;
                        leapEndGo.SetActive(true);
                        FMODUnity.RuntimeManager.PlayOneShot("event:/" + ((SharkPassiveSO)passive.AssociatedPassiveCapacitySO()).SFXSharkPassive, champion.position);
                    }
                    
                    damageAreaGo.SetActive(true);
                    damageArea.OnEntityCollide += DamageAndStun;
                }

                if (timer <= so.animationDuration) return;

                timer = 0;
                
                champion.rotateParent.localScale = Vector3.one;
                    
                damageAreaGo.SetActive(false);

                if (isMaster)
                {
                    champion.SetCanAttackRPC(true);
                    champion.SetCanCastRPC(true);
                    champion.SetCanMoveRPC(true);
                }

                gsm.OnUpdateFeedback -= IncreaseTimerPart3;
                
            }

            void DamageAndStun(Entity entity)
            {
                if (!caster.GetEnemyTeams().Contains(entity.team)) return;
                
                entity.AddPassiveCapacityRPC(so.stun.indexInCollection);
                
                var lifeable = entity.GetComponent<IActiveLifeable>();
                lifeable?.DecreaseCurrentHpRPC(so.damage,caster.entityIndex);
            }
        }

        private void Leap(Vector3 destination)
        {
            sharkPassive.ForceUnBorrow();
            
            if (champion.isVisible)
            {
                leapStartGo.transform.position = champion.position;
                leapStartGo.SetActive(true);
            }
            
            if (Vector3.Distance(champion.position, destination) > so.maxRange)
            {
                var dir = destination - champion.position;
                dir = dir.normalized * so.maxRange;
                destination = champion.position + dir;
            }
            
            destination = GetClosestValidPoint(destination);
            
            leapMarkerGo.transform.position = destination;
            leapMarkerGo.SetActive(true);
            
            champion.SetAnimatorTrigger("Ability3");
            champion.rotateParent.localScale = Vector3.one * so.sizeMultiplier;
            
            champion.LookAt(destination);

            if (isMaster)
            {
                champion.DisplaceRPC(destination,so.animationDuration);
            }
        }

        protected override void ReleaseLocal(int targetEntityIndex, Vector3 targetPositions)
        {
            champion.HideAreaIndicator();
        }

        private void InstantiateFx()
        {
            if (damageArea == null)
            {
                damageArea = LocalPoolManager.PoolInstantiate(so.impactArea,champion.rotateParent);
                damageArea.transform.localPosition = Vector3.zero;
                damageAreaGo = damageArea.gameObject;
                damageAreaGo.SetActive(false);
            }

            var isAlly = gsm.GetPlayerChampion().team == champion.team;
            var startFx = isAlly ? so.leapStart : so.leapStartR;
            var endFx = isAlly ? so.leapLand : so.leapLandR;
            var markerFx = isAlly ? so.leapIndicator : so.leapIndicatorR;

            if (leapStartGo == null)
            {
                leapStartGo = Object.Instantiate(startFx).gameObject;
                leapStartGo.transform.localScale = Vector3.one * 2f;
            }
            
            if (leapEndGo == null)
            {
                leapEndGo = Object.Instantiate(endFx).gameObject;
                leapEndGo.transform.localScale = Vector3.one * 2f;
            }

            if (leapMarkerGo == null)
            {
                leapMarkerGo = Object.Instantiate(markerFx).gameObject;
                leapMarkerGo.transform.localScale = Vector3.one * 3f;
            }
            
            leapStartGo.SetActive(false);
            leapMarkerGo.SetActive(false);
            leapEndGo.SetActive(false);
        }
    }
    
}


