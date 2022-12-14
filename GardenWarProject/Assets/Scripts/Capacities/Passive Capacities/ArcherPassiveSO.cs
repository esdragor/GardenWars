using System;
using UnityEngine;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/PassiveCapacitySO/ArcherPassive", fileName = "ArcherPassive")]
    public class ArcherPassiveSO : PassiveCapacitySO
    {
        public double timeToIncrease = 1.5f;
        public double timeToDecrease = 0.5f;
        public int maxStacks = 5;
        [SerializeField] private AttackSpeedBuffSO attackSpeedBuffSO;
        public byte attackSpeedBuffSOIndex => attackSpeedBuffSO.indexInCollection;

        public override Type AssociatedType()
        {
            return typeof(ArcherPassive);
        }
    }
    
    public class ArcherPassive : PassiveCapacity
    {
        private ArcherPassiveSO so => (ArcherPassiveSO) AssociatedPassiveCapacitySO();
        
        private int currentStacks;
        private int maxStacks;
        
        public double timeToIncrease;
        public double timeToDecrease;
        
        private double timer;

        private bool isMoving;

        public override PassiveCapacitySO AssociatedPassiveCapacitySO()
        {
            return CapacitySOCollectionManager.Instance.GetPassiveCapacitySOByIndex(indexOfSo);
        }

        protected override void OnAddedEffects(Entity target)
        {
            currentStacks = 0;
            maxStacks = so.maxStacks;

            timeToIncrease = so.timeToIncrease;
            timeToDecrease = so.timeToDecrease;
            
            timer = 0;
            
            champion.OnDie += ResetStacks;
            
        }
        
        protected override void OnAddedFeedbackEffects(Entity target)
        {
            if (!champion.isPlayerChampion) return;
            
            currentStacks = 0;
            maxStacks = so.maxStacks;

            timeToIncrease = so.timeToIncrease;
            timeToDecrease = so.timeToDecrease;
            
            timer = 0;
                
            gsm.OnUpdateFeedback += OnUpdate;
        }
        
        private void ResetStacks(int _)
        {
            for (int i = 0; i < currentStacks; i++)
            {
                champion.RequestRemovePassiveCapacityByIndex(so.attackSpeedBuffSOIndex);
            }

            currentStacks = 0;
        }

        private void OnUpdate()
        {
            if(!champion.isPlayerChampion) return;
            CheckSpeed();
            if (isMoving)
            {
                DecreaseStacks();
            }
            else
            {
                IncreaseStacks();
            }
        }
        
        private void CheckSpeed()
        {
            if(!champion.isAlive) return;
            
            timer += Time.deltaTime;
            
            if (champion.currentVelocity > 0 && !isMoving)
            {
                isMoving = true;
                timer = 0;
            }
            
            if (champion.currentVelocity <= 0 && isMoving)
            {
                isMoving = false;
                timer = 0;
            }
        }

        private void DecreaseStacks()
        {
            if(currentStacks <= 0) return;
            if(timer < timeToDecrease) return;
            timer = 0;
            
            currentStacks--;
            champion.RequestRemovePassiveCapacityByIndex(so.attackSpeedBuffSOIndex);
        }
        
        private void IncreaseStacks()
        {
            if(currentStacks >= maxStacks) return;
            if(timer < timeToIncrease) return;
            timer = 0;
            
            currentStacks++;
            champion.RequestAddPassiveCapacityByIndex(so.attackSpeedBuffSOIndex);
        }


        protected override void OnRemovedEffects(Entity target)
        {
            champion.OnDie -= ResetStacks;
        }

        protected override void OnRemovedFeedbackEffects(Entity target)
        {
            if(champion.isPlayerChampion) gsm.OnUpdateFeedback -= OnUpdate;
        }
        
        
    }
}


