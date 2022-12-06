using System;
using UnityEngine;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/Ranged Auto Attack", fileName = "new Ranged Auto Attack")]
    public class RangedAACapacitySO : ActiveCapacitySO
    {
        public override Type AssociatedType()
        {
            return typeof(RangedAACapacity);
        }
    }

}
