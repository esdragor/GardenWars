using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/PassiveCapacitySO/ArcherDebuff", fileName = "ArcherDebuff")]
    public class ArcherGrabDebuffSO : PassiveCapacitySO
    {
        public float moveSpeedMultiplier;
        public float attackSpeedMultiplier;
        
        public override Type AssociatedType()
        {
            return typeof(ArcherGrabDebuff);
        }
    }
    
    public class ArcherGrabDebuff : PassiveCapacity
    {
        private float removedMovespeed;
        private float removedAttackSpeed;

        private ArcherGrabDebuffSO so => (ArcherGrabDebuffSO) AssociatedPassiveCapacitySO();
        
        public override PassiveCapacitySO AssociatedPassiveCapacitySO()
        {
            return CapacitySOCollectionManager.Instance.GetPassiveCapacitySOByIndex(indexOfSo);
        }

        protected override void OnAddedEffects(Entity target)
        {
            removedMovespeed = (float) champion.baseAttackSpeed * (1-so.moveSpeedMultiplier);
            champion.DecreaseCurrentMoveSpeedRPC(removedMovespeed);
            removedAttackSpeed = (float) champion.baseAttackSpeed * (1-so.attackSpeedMultiplier);
            champion.ChangeAttackSpeedRPC(-removedAttackSpeed);
        }

        protected override void OnAddedFeedbackEffects(Entity target)
        {
            
        }

        protected override void OnAddedLocalEffects(Entity target)
        {
            
        }

        protected override void OnRemovedEffects(Entity target)
        {
            champion.IncreaseCurrentMoveSpeedRPC(removedMovespeed);
            champion.ChangeAttackSpeedRPC(removedAttackSpeed);
        }

        protected override void OnRemovedFeedbackEffects(Entity target)
        {
            
        }

        protected override void OnRemovedLocalEffects(Entity target)
        {
            
        }
    }
}

