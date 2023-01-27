using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/SharkBorrow", fileName = "new SharkBorrow")]
    public class SharkBorrowSO : ActiveCapacitySO
    {
        public SharkPassiveSO sharkPassive;
        public float aileronBonusDamage = 10f;
        
        public override Type AssociatedType()
        {
            return typeof(SharkBorrow);
        }
    }

    public class SharkBorrow : ActiveCapacity
    {
        private SharkBorrowSO so => (SharkBorrowSO)AssociatedActiveCapacitySO();
        private SharkPassive _sharkPassive;
        private SharkPassive sharkPassive => _sharkPassive ??= champion.GetPassiveCapacityBySOIndex<SharkPassive>(so.sharkPassive.indexInCollection);

        public override void Init()
        {
            isUnusable = true;
            sharkPassive.burrow = this;
            OnUsable?.Invoke(isUnusable);
        }
        
        public void UpdateUsable(bool value)
        {
            isUnusable = !value;
            OnUsable?.Invoke(!isUnusable);
        }

        protected override bool AdditionalCastConditions(int targetsEntityIndexes, Vector3 targetPositions)
        {
            return !sharkPassive.borrowed;
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
            sharkPassive.bonusDamage = so.aileronBonusDamage;
        }

        protected override void ReleaseFeedback(int targetEntityIndex, Vector3 targetPositions)
        {
            sharkPassive.Borrow();
        }

        protected override void ReleaseLocal(int targetEntityIndex, Vector3 targetPositions)
        {
        }
    }
}