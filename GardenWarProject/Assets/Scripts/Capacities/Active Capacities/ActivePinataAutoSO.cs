using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Entities.Capacities;
using UnityEngine;

[CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/Auto Attack Pinata", fileName = "AI Auto Attack for Pinata")]
public class ActivePinataAutoSO : ActiveCapacitySO
{
    
    public int AtkValue;
    public GameObject ItemBagPrefab;
    public override Type AssociatedType()
    {
        return typeof(ActivePinataAutoSO);
    }
}
