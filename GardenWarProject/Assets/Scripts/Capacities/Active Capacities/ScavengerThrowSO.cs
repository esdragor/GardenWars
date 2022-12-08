using System;
using UnityEngine;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/ScavengerThrow", fileName = "new ScavengerThrow")]
    public class ScavengerThrowSO : ActiveCapacitySO
    {
        public GameObject itemBagPrefab;
        public GameObject prefabJauge;
        public int nbBounce = 5;
        [Range(0.1f, 10f)] public float SpeedOnAir = 1.0f;
        public float height = 5.0f;
        public float HextechFlashSpeedScale = 1f;
        public float MinDistanceHFlash = 5.0f;
        public float MaxDistanceHFlash = 5.0f;
        public float RatioMouseDistance = 10.0f;

        public override Type AssociatedType()
        {
            return typeof(ScavengerThrow);
        }
    }
}
