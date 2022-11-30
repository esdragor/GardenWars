using System;
using System.Collections;
using System.Collections.Generic;
using Entities.Capacities;
using UnityEngine;

[CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/Flipper", fileName = "Flipper")]
public class FlipperSO : ActiveCapacitySO
{
    public GameObject CandyBagPrefab;
    
    
    public override Type AssociatedType()
    {
        return typeof(ActiveCapacity);
    }
}
