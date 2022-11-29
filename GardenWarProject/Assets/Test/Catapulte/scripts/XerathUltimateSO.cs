using System;
using System.Collections;
using System.Collections.Generic;
using Entities.Capacities;
using UnityEngine;

[CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/UltiXerath", fileName = "Ulti Xerath")]
public class XerathUltimateSO : ActiveCapacitySO
{
    public GameObject prefab = null;
    public int nbBounce = 5;
    [Range(0.1f, 10f)] public float SpeedOnAir = 1.0f;
    public float height = 5.0f;
    public bool RandomizeZone = false;
    public float RandomizeZoneRadius = 5.0f;

    public override Type AssociatedType()
    {
        return typeof(XerathUltimate);
    }
}
