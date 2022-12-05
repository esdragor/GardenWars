using System;
using System.Collections;
using System.Collections.Generic;
using Entities.Capacities;
using UnityEngine;

[CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/Flipper", fileName = "Flipper")]
public class FlipperSO : ActiveCapacitySO
{
    public GameObject CandyBagPrefab;
    [Range(0.1f, 10f)]public float CandyBagSpeed = 3f;
    public bool decreaseSpeedDuring = false;
    [Range(0, 1)] public float ForceDecelerate = 0.03f;
    public bool StopByTimer;
    public float MaxTimer;
    public bool StopByRebound;
    public int MaxRebound;
    public bool StopBagWithoutDelay = false;
    public int nbCandy = 1;
    public bool speedByNbCandy = false;
    public bool ScaleBagByNbCandy = false;

    public override Type AssociatedType()
    {
        return typeof(Flipper);
    }
}