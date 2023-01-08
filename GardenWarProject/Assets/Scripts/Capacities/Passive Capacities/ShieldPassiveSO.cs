using System;
using UnityEngine;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/PassiveCapacitySO/Shield", fileName = "SharkShield")]
    public class ShieldPassiveSO : PassiveCapacitySO
    {
        public float shieldAmount;
        
        public override Type AssociatedType()
        {
            return typeof(ShieldPassive);
        }
    }

    public class ShieldPassive : PassiveCapacity
    {
        private ShieldPassiveSO so => (ShieldPassiveSO) AssociatedPassiveCapacitySO();
        
        private float currentShieldAmount = 0;
        private IActiveLifeable lifeable;
        private IDeadable deadable;
        
        public override PassiveCapacitySO AssociatedPassiveCapacitySO()
        {
            return CapacitySOCollectionManager.Instance.GetPassiveCapacitySOByIndex(indexOfSo);
        }

        protected override void OnAddedEffects(Entity target)
        {
            currentShieldAmount += so.shieldAmount;

            lifeable = target.GetComponent<IActiveLifeable>();
            deadable = target.GetComponent<IDeadable>();
            
            if(lifeable == null || count > 1) return;

            lifeable.OnDecreaseCurrentHp += DecreaseShiedAmount;
            
            deadable?.SetCanDieRPC(false); //Todo - immune to death plus propre
        }

        protected override void OnAddedFeedbackEffects(Entity target)
        {
            
        }

        private void DecreaseShiedAmount(float damage,int source)
        {
            currentShieldAmount -= damage;

            lifeable.IncreaseCurrentHpRPC(damage,entity.entityIndex);

            if (currentShieldAmount > 0) return;

            var overflowDamage = -currentShieldAmount;
            
            entity.RemovePassiveCapacityByIndexRPC(indexOfSo);
            
            deadable.SetCanDieRPC(true);
            
            lifeable.DecreaseCurrentHpRPC(overflowDamage,source);
        }

        protected override void OnRemovedEffects(Entity target)
        {
            if(count > 0)  return;

            lifeable.OnDecreaseCurrentHp -= DecreaseShiedAmount;
            
            deadable.SetCanDieRPC(true);
        }

        protected override void OnRemovedFeedbackEffects(Entity target)
        {
            
        }
    }
}


