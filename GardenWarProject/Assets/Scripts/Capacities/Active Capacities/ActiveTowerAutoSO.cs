using System;
using System.Collections;
using System.Collections.Generic;
using Entities.Capacities;
using UnityEngine;

[CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/Auto Attack Tower", fileName = "AI Auto Attack for Tower")]
public class ActiveTowerAutoSO  : ActiveCapacitySO
{
    public int AtkValue;
    
    public override Type AssociatedType()
    {
        return typeof(ActiveTowerAuto);
    }
}
