using System;
using Entities.Capacities;
using UnityEngine;

[CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/FighterThrow", fileName = "new FighterThrow")]
public class FighterThrowSO : ActiveCapacitySO
{
    public GameObject candyBagPrefab;
    public int nbBounce = 5;
    [Range(0.1f, 10f)] public float SpeedOnAir = 1.0f;
    public float height = 5.0f;
    public float Speed = 1f;
    public int MinCandy = 5;
    public int MaxCandy = 10;
    public float accelerationJauge = 1f;
    public bool RandomizeRebound = false;
    public float RandomizeReboundRadius = 0.5f;
    // public bool ScalebyNbCandy = false;
    // public int damageCandyScale = 1; // damage if not palier, damage * (scaleByCandy * nbCandy)
    // public float scaleByCandy = 0.33f; // if not palier
    // public float scaleAndDamageByNbCandyOnBag = 5f; //if palier
    // public int NbCandyPerPalier = 5; // nb candy trigger palier
    // public int NbCandyTriggerDamage = 1; // nb candy need to trigger damage
    // public Vector3 DecalCenterRandomize = Vector3.zero; // nb candy need to trigger damage

    public override Type AssociatedType()
    {
        return typeof(FighterThrow);
    }
}
