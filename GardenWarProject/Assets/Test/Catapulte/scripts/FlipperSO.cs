using System;
using System.Collections;
using System.Collections.Generic;
using Entities.Capacities;
using UnityEngine;

[CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/Flipper", fileName = "Flipper")]
public class FlipperSO : ActiveCapacitySO
{
    public GameObject CandyBagPrefab;
    public float CandyBagSpeed;
    public bool decreaseSpeed = false;
    [Range(0, 1)] public float ForceDecelerate = 0.03f;
    public bool StopByTimer;
    public float Timer;
    public bool StopByRebound;
    public int MaxRebound;


    public override Type AssociatedType()
    {
        return typeof(ActiveCapacity);
    }
}