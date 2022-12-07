using System;
using System.Collections;
using System.Collections.Generic;
using Entities.Capacities;
using UnityEngine;

public enum HextechMode
{
    hold,
    jauge,
    mouseDistance

}

[CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/UltiXerath", fileName = "Ulti Xerath")]
public class XerathUltimateSO : ActiveCapacitySO
{
    public GameObject prefab = null;
    public GameObject prefabJauge = null;
    public int nbBounce = 5;
    [Range(0.1f, 10f)] public float SpeedOnAir = 1.0f;
    public float height = 5.0f;
    public bool RandomizeZone = false;
    public float RandomizeZoneRadius = 5.0f;
    public HextechMode hextechMode = HextechMode.hold;
    public float HextechFlashSpeedScale = 1f;
    public float MinDistanceHFlash = 5.0f;
    public float MaxDistanceHFlash = 5.0f;
    public float RatioMouseDistance = 10.0f;
    public int nbCandy = 1;
    public bool speedByNbCandy = false;
    public bool ScaleBagByNbCandy = false;

    public override Type AssociatedType()
    {
        return typeof(XerathUltimate);
    }
}
