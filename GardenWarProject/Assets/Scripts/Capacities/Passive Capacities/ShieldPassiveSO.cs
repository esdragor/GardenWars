using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/PassiveCapacitySO/Shield", fileName = "SharkShield")]
    public class ShieldPassiveSO : PassiveCapacitySO
    {
        public float shieldAmount;
        public ParticleSystem FXShieldBlue;
        public ParticleSystem FXShieldRed;

        public ParticleSystem FXShieldBreakBlue;
        public ParticleSystem FXShieldBreakRed;

        public override Type AssociatedType()
        {
            return typeof(ShieldPassive);
        }
    }

    public class ShieldPassive : PassiveCapacity
    {
        private ShieldPassiveSO so => (ShieldPassiveSO)AssociatedPassiveCapacitySO();

        private float currentShieldAmount = 0;
        private IActiveLifeable lifeable;
        private IDeadable deadable;
        private ParticleSystem FXShield;
        private GameObject FXShieldGO;
        private ParticleSystem FXShieldBreak;
        private GameObject FXShieldBreakGO;

        public override PassiveCapacitySO AssociatedPassiveCapacitySO()
        {
            return CapacitySOCollectionManager.Instance.GetPassiveCapacitySOByIndex(indexOfSo);
        }

        protected override void OnAddedEffects(Entity target)
        {
            currentShieldAmount += so.shieldAmount;

            lifeable = target.GetComponent<IActiveLifeable>();
            deadable = target.GetComponent<IDeadable>();

            if (lifeable == null || count > 1) return;

            lifeable.OnDecreaseCurrentHp += DecreaseShiedAmount;

            deadable?.SetCanDieRPC(false); //Todo - immune to death plus propre
        }

        protected override void OnAddedFeedbackEffects(Entity target)
        {
            LaunchShieldFX();
        }

        protected override void OnAddedLocalEffects(Entity target)
        {
            
        }

        public void LaunchShieldFX()
        {
            if (!FXShield)
            {
                FXShield = champion.team == gsm.GetPlayerTeam() ? so.FXShieldBlue : so.FXShieldRed;
                FXShieldGO = LocalPoolManager.PoolInstantiate(FXShield, champion.transform).gameObject;
                FXShieldGO.transform.localPosition = Vector3.up * 0.5f;
            }

            FXShieldGO.SetActive(true);
        }

        public void LaunchBreakFX()
        {
            FXShieldGO.SetActive(false);
            if (!FXShieldBreak)
            {
                FXShieldBreak = champion.team == gsm.GetPlayerTeam() ? so.FXShieldBreakBlue : so.FXShieldBreakRed;
                FXShieldBreakGO = LocalPoolManager.PoolInstantiate(FXShieldBreak, champion.gameObject.transform).gameObject;
                FXShieldBreakGO.transform.localPosition = Vector3.up * 0.5f;
            }

            FXShieldBreakGO.SetActive(false);
            FXShieldBreakGO.SetActive(true);
        }

        private void DecreaseShiedAmount(float damage, int source)
        {
            currentShieldAmount -= damage;

            entity.RemovePassiveCapacityByIndexRPC(indexOfSo);

            lifeable.IncreaseCurrentHpRPC(damage, entity.entityIndex);

            if (currentShieldAmount > 0) return;
            
            var overflowDamage = -currentShieldAmount;
            
            deadable.SetCanDieRPC(true);

            lifeable.DecreaseCurrentHpRPC(overflowDamage, source);
        }

        protected override void OnRemovedEffects(Entity target)
        {
            if (count > 0) return;

            LaunchBreakFX();
            
            lifeable.OnDecreaseCurrentHp -= DecreaseShiedAmount;

            deadable.SetCanDieRPC(true);
        }

        protected override void OnRemovedFeedbackEffects(Entity target)
        {
        }

        protected override void OnRemovedLocalEffects(Entity target)
        {
            
        }
    }
}