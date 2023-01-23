using System;
using UnityEngine;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/PassiveCapacitySO/StunPassive", fileName = "StunPassive")]
    public class StunPassiveSO : PassiveCapacitySO
    {
        public ParticleSystem stunFx;
        public float height = 1.75f;
        
        public override Type AssociatedType()
        {
            return typeof(StunPassive);
        }
    }

    public class StunPassive : PassiveCapacity
    {
        private StunPassiveSO so => (StunPassiveSO) AssociatedPassiveCapacitySO();

        private GameObject fxGo;
         
        private IMoveable moveable;
        private IAttackable attackable;
        private ICastable castable;
        
        protected override void OnAddedEffects(Entity target)
        {
            moveable = entity.GetComponent<IMoveable>();
            attackable = entity.GetComponent<IAttackable>();
            castable = entity.GetComponent<ICastable>();

            moveable?.SetCanMoveRPC(false);
            attackable?.SetCanAttackRPC(false);
            castable?.SetCanCastRPC(false);
        }

        protected override void OnAddedFeedbackEffects(Entity target)
        {
            var fx = LocalPoolManager.PoolInstantiate(so.stunFx, entity.transform.position + Vector3.up * so.height,
                Quaternion.Euler(-90, 0, 0),entity.parent);
            
            fx.Stop();
            var main = fx.main;
            main.duration = (float)duration;
            fx.Play();

            fxGo = fx.gameObject;
            
            fxGo.SetActive(true);
        }

        protected override void OnAddedLocalEffects(Entity target)
        {
            
        }

        protected override void OnRemovedEffects(Entity target)
        {
            moveable?.SetCanMoveRPC(true);
            attackable?.SetCanAttackRPC(true);
            castable?.SetCanCastRPC(true);
        }

        protected override void OnRemovedFeedbackEffects(Entity target)
        {
            fxGo.SetActive(false);
        }

        protected override void OnRemovedLocalEffects(Entity target)
        {
            
        }
    }
}


