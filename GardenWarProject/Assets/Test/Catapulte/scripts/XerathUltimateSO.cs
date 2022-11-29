using System;
using System.Collections;
using System.Collections.Generic;
using Entities.Capacities;
using UnityEngine;

[CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/UltiXerath", fileName = "Ulti Xerath")]
public class XerathUltimateSO : ActiveCapacitySO
{
    public int nbBounce = 5;
    public GameObject prefab = null;
    [Range(0.2f, 20f)] public float ReduceSpeed = 1.0f;
    public float height = 5.0f;
    public bool RandomizeZone = false;
    public float RandomizeZoneRadius = 5.0f;

    public override Type AssociatedType()
    {
        return typeof(XerathUltimate);
    }
}
