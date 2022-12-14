using System;
using Entities.Capacities;
using UnityEngine;

public class FighterThrowSO : ActiveCapacitySO
{
    public GameObject candyBagPrefab;
    public GameObject prefabJauge;
    public int nbBounce = 5;
    [Range(0.1f, 10f)] public float SpeedOnAir = 1.0f;
    public float height = 5.0f;
    public float HextechFlashSpeedScale = 1f;
    public float MinDistanceHFlash = 5.0f;
    public float MaxDistanceHFlash = 5.0f;

    public override Type AssociatedType()
    {
        return typeof(ScavengerThrow);
    }
}
