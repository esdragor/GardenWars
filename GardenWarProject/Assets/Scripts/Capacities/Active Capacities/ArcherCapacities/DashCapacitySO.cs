using System;
using UnityEngine;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/DashCapacity", fileName = "new DashCapacity")]
    public class DashCapacitySO : ActiveCapacitySO
    {
        [Header("Dash Settings")]
        public bool isBlink;
        [Tooltip("In Seconds")]public double dashTime;
        
        public override Type AssociatedType()
        {
            return typeof(DashCapacity);
        }
    }
}