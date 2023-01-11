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

        protected override bool AdditionalCastConditions(int targetsEntityIndexes, Vector3 targetPositions)
        {
            Debug.Log($"Passive : {sharkPassive}");
            return true;
            return !sharkPassive.borrowed;
        }

        protected override void Press(int targetsEntityIndexes, Vector3 targetPositions)
        {
            
        }

        protected override void PressFeedback(int targetsEntityIndexes, Vector3 targetPositions)
        {
            
        }

        protected override void Hold(int targetsEntityIndexes, Vector3 targetPositions)
        {
            
        }

        protected override void HoldFeedback(int targetsEntityIndexes, Vector3 targetPositions)
        {
            
        }

        protected override void Release(int targetsEntityIndexes, Vector3 targetPositions)
        {
            Debug.Log("Trying to Borrow");
            sharkPassive.Borrow();
        }

        protected override void ReleaseFeedback(int targetEntityIndex, Vector3 targetPositions)
        {
            
        }
    }
}