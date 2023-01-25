using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/PassiveCapacitySO/CandySpeedBuff", fileName = "CandySpeedBuff")]
    public class CandySpeedBuffSO : PassiveCapacitySO
    {
        public ParticleSystem fx;
        public float moveSpeedIncrease = 0.5f;
        public float cooldownReduction = 0.3f;
        public float attackSpeedBonus = 0.25f;
        
        public override Type AssociatedType()
        {
            return typeof(CandySpeedBuff);
        }
    }
    
    public class CandySpeedBuff : PassiveCapacity
    {
        private CandySpeedBuffSO so => (CandySpeedBuffSO) AssociatedPassiveCapacitySO();
        
        private float msIncrease;
        private float attackSpeedIncrease;
        private float cdDecrease;
        
        protected override void OnAddedEffects(Entity target)
        {
            msIncrease = champion.baseMoveSpeed * so.moveSpeedIncrease;
            attackSpeedIncrease = (float)champion.baseAttackSpeed * so.attackSpeedBonus;
            cdDecrease = champion.currentCd * so.cooldownReduction;
            
            Debug.Log("Added passive");
            
            champion.ChangeCooldownReduction(cdDecrease);
            champion.IncreaseCurrentMoveSpeedRPC(msIncrease);
            champion.ChangeAttackSpeedRPC(attackSpeedIncrease);
        }

        protected override void OnAddedFeedbackEffects(Entity target)
        {
            var fx =LocalPoolManager.PoolInstantiate(so.fx, champion.position + Vector3.up, Quaternion.Euler(-90, 0, 0)).gameObject;
            fx.SetActive(false);
            fx.SetActive(true);
        }

        protected override void OnAddedLocalEffects(Entity target)
        {
            
        }

        protected override void OnRemovedEffects(Entity target)
        {
            champion.ChangeCooldownReduction(-cdDecrease);
            champion.DecreaseCurrentMoveSpeedRPC(msIncrease);
            champion.ChangeAttackSpeedRPC(-attackSpeedIncrease);
        }

        protected override void OnRemovedFeedbackEffects(Entity target)
        {
            
        }

        protected override void OnRemovedLocalEffects(Entity target)
        {
            
        }
    }
}

