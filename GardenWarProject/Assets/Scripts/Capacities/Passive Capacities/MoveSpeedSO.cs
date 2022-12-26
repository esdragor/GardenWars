using System;
using System.Collections;
using System.Collections.Generic;
using Entities.Capacities;
using UnityEngine;

[CreateAssetMenu(menuName = "Capacity/PassiveCapacitySO/MoveSpeed", fileName = "MoveSpeed")]
public class MoveSpeedSO : PassiveCapacitySO
{
    public override Type AssociatedType()
    {
        return typeof(MoveSpeed);
    }
        
public float moveSpeed = 5f;
}
