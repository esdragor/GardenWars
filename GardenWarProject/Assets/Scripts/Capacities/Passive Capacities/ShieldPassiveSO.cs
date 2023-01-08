using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;

namespace Entities.Capacities
{
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
        public float currentShieldAmount;
        
        public override PassiveCapacitySO AssociatedPassiveCapacitySO()
        {
            return CapacitySOCollectionManager.Instance.GetPassiveCapacitySOByIndex(indexOfSo);
        }

        protected override void OnAddedEffects(Entity target)
        {
            var lifeable = target.GetComponent<IActiveLifeable>();
            
            if(lifeable == null) return;

            lifeable.OnDecreaseCurrentHp += DecreaseShiedAmount;
        }

        protected override void OnAddedFeedbackEffects(Entity target)
        {
            
        }

        private void DecreaseShiedAmount(float damage,int source)
        {
            
        }

        protected override void OnRemovedEffects(Entity target)
        {
            
        }

        protected override void OnRemovedFeedbackEffects(Entity target)
        {
            
        }
    }
}


