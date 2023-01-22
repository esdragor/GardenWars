using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/PassiveCapacitySO/RecallPassive", fileName = "RecallPassive")]
    public class RecallPassiveSO : PassiveCapacitySO
    {
        public ParticleSystem recallFxGo;
        public Vector3 fxRotation;
        public float fxHeight;
        
        public override Type AssociatedType()
        {
            return typeof(RecallPassive);
        }
    }
    
    public class RecallPassive : PassiveCapacity
    {
        private RecallPassiveSO so => (RecallPassiveSO) AssociatedPassiveCapacitySO();
        private GameObject recallObjectGo;
        private bool isCanceled;

        protected override void OnAddedEffects(Entity target)
        {
            champion.OnDisplace += CancelRecall;
            champion.OnMoving += CancelRecall;
            champion.OnAttack += CancelRecall;
            champion.OnCast += CancelRecall;
        }

        protected override void OnAddedFeedbackEffects(Entity target)
        {
            recallObjectGo = LocalPoolManager.PoolInstantiate(so.recallFxGo, champion.position+Vector3.up*so.fxHeight, Quaternion.Euler(so.fxRotation)).gameObject;
            recallObjectGo.transform.localScale = Vector3.one * 0.5f;
        }

        protected override void OnAddedLocalEffects(Entity target)
        {
            
        }

        protected override void OnRemovedEffects(Entity target)
        {
            champion.OnDisplace -= CancelRecall;
            champion.OnMoving -= CancelRecall;
            champion.OnAttack -= CancelRecall;
            champion.OnCast -= CancelRecall;
            
            if(isCanceled) return;
            
            champion.DisplaceRPC(champion.respawnPosition,0);
        }

        protected override void OnRemovedFeedbackEffects(Entity target)
        {
            recallObjectGo.SetActive(false);
        }

        protected override void OnRemovedLocalEffects(Entity target)
        {
            
        }

        private void CancelRecall(Vector3 _,float __)
        {
            CancelRecall();
        }
        
        private void CancelRecall(bool moving)
        {
            if(moving) CancelRecall();
        }
        
        private void CancelRecall(byte index,int _,Vector3 __)
        {
            if(index == champion.recallAbilityIndex) return;
            
            CancelRecall();
        }

        private void CancelRecall()
        {
            isCanceled = true;

            internalPassiveTimer = 0;
        }
    }
}


