using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/DashCapacity", fileName = "new DashCapacity")]
    public class DashCapacitySO : ActiveCapacitySO
    {
        [Header("Dash Settings")] public bool isBlink;
        [Tooltip("In Seconds")] public double dashTime;
        public bool fixedDistance;
        public bool canPassWalls;
        public ParticleSystem FXDashBlue;
        public ParticleSystem FXDashRed;


        public override Type AssociatedType()
        {
            return typeof(DashCapacity);
        }
    }

    public class DashCapacity : ActiveCapacity
    {
        private DashCapacitySO so => (DashCapacitySO) AssociatedActiveCapacitySO();
        private bool isBlink => so.isBlink;
        private double dashDuration => so.dashTime;

        private int charges = -1;

        private LayerMask collisionLayers;
        private ParticleSystem FXDash;
        private GameObject FXDashGO;
        
        private ArcherPassive passive;
        private ArcherPassive archerPassive => passive ??= champion.GetPassiveCapacity<ArcherPassive>();

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
            if (champion != null)
            {
                champion.canMove = false;
                
                champion.SetAnimatorTrigger("Ability1");
            }
            

            collisionLayers = 1 << 9 | 1 << 22 | 1 << 30;

            var destination = targetPositions;
            destination.y = casterPos.y;

            var direction = (destination - casterPos).normalized;
            var distance = Vector3.Distance(casterPos, destination);

            if (distance > so.maxRange || so.fixedDistance) distance = so.maxRange;

            destination = casterPos + direction * distance;

            if (level == 1)
            {
                if (Physics.Raycast(casterPos, direction, out var hit, distance, collisionLayers) && !so.canPassWalls &&
                    !isBlink)
                {
                    destination = hit.point;
                }
            }
            

            destination = GetClosestValidPoint(destination);
            
            champion.LookAt(destination);

            if (isBlink)
            {
                Blink(destination);
                return;
            }
            
            if (!FXDash)
            {
                FXDash = champion.team == gsm.GetPlayerTeam()? so.FXDashBlue : so.FXDashRed;
                FXDashGO = LocalPoolManager.PoolInstantiate(FXDash, champion.championMesh.transform).gameObject;
                FXDashGO.transform.localPosition = Vector3.zero;
                FXDashGO.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
            }
            FXDashGO.SetActive(false);
            FXDashGO.SetActive(true);
            champion.championMesh.transform.LookAt(destination);
            
            StartDash(casterPos, destination);
        }

        protected override void ReleaseLocal(int targetEntityIndex, Vector3 targetPositions)
        {
        }

        private void Blink(Vector3 destination)
        {
            if (champion == null) return;
            if (champion == gsm.GetPlayerChampion())
            {
                champion.agent.enabled = true;
                champion.agent.Warp(destination);
                champion.canMove = true;
            }
            

            if(level <3) return;
            
            charges--;
            if(charges <= -2) StartStackingDashes();
            if (charges > 0)
            {
                cooldownTimer = 0;
            }
        }

        private void StartStackingDashes()
        {
            charges = 2;

            var chargeTimer = so.cooldown;

            gsm.OnUpdateFeedback += GainCharge;

            void GainCharge()
            {
                if(isOnCooldown || charges >= 3) return;
                
                chargeTimer -= Time.deltaTime;
                
                if(chargeTimer > 0) return;
                chargeTimer = so.cooldown;
                charges++;
            }
        }

        private void StartDash(Vector3 start, Vector3 destination)
        {
            champion.agent.enabled = false;

            double timeInDash = 0;

            if (archerPassive != null)  archerPassive.holdStacks = true;

            gsm.OnUpdateFeedback += DashTowardsDestination;

            void DashTowardsDestination()
            {
                timeInDash += Time.deltaTime;

                caster.transform.position = Vector3.Lerp(start, destination, (float) (timeInDash / dashDuration));
                
                if (timeInDash < dashDuration) return;

                Blink(destination);
                
                if (archerPassive != null)  archerPassive.holdStacks = false;
                
                gsm.OnUpdateFeedback -= DashTowardsDestination;
            }
        }
    }
}