using System;
using UnityEngine;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/PassiveCapacitySO/SharkPassive", fileName = "SharkPassive")]
    public class SharkPassiveSO : PassiveCapacitySO
    {
        public double timeUntilBorrow = 3;
        public float borrowDamage = 10f;
        public double knockUpDuration = 1;
        public float knockUpDamage = 20f;
        
        public override Type AssociatedType()
        {
            return typeof(SharkPassive);
        }
    }

    public class SharkPassive : PassiveCapacity
    {
        private SharkPassiveSO so => (SharkPassiveSO) AssociatedPassiveCapacitySO();
        
        private double timeUnBorrowed;
        public bool borrowed;

        public override PassiveCapacitySO AssociatedPassiveCapacitySO()
        {
            return CapacitySOCollectionManager.Instance.GetPassiveCapacitySOByIndex(indexOfSo);
        }

        protected override void OnAddedEffects(Entity target)
        {
            timeUnBorrowed = 0;
            borrowed = false;

            gsm.OnUpdate += IncreaseTimeUnBorrowed;
            
            champion.OnAttack += ResetTimer;
            champion.OnAttack += UnBorrow;
            
            champion.OnDie += UnBorrow;
            champion.OnDie += ResetTimer;
        }

        private void IncreaseTimeUnBorrowed()
        {
            if(borrowed) return;
            
            timeUnBorrowed += Time.deltaTime;
            if (timeUnBorrowed >= so.timeUntilBorrow)
            {
                Borrow();
            }
        }

        private void Borrow()
        {
            if(borrowed) return;
            
            champion.SetCanBeTargetedRPC(false);
            
            borrowed = true;
        }
        
        private void UnBorrow(byte _,int __,Vector3 ___)
        {
            if(!borrowed) return;
            
            champion.SetCanBeTargetedRPC(true);
            
            borrowed = false;
        }
        
        private void UnBorrow(int _)
        {
            UnBorrow(0,0,Vector3.zero);
        }
        
        private void ResetTimer(byte _,int __,Vector3 ___)
        {
            timeUnBorrowed = 0;
        }
        
        private void ResetTimer(int _)
        {
            ResetTimer(0,0,Vector3.zero);
        }

        protected override void OnAddedFeedbackEffects(Entity target)
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


