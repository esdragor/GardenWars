using Entities.Inventory;
using UnityEngine;

namespace Entities.Capacities
{
    public class ScavengerThrow : ActiveCapacity
    {
        private ScavengerThrowSO so => (ScavengerThrowSO)AssociatedActiveCapacitySO();
        private Camera cam;
        private LayerMask layersToHit;
        
        private Vector3 targetPosition;
        
        private GameObject HelperDirection = null;
        private UIJauge UIJauge = null;
        private double Animation = 0f;

        private bool IsHextech = false;
        private float time_Pressed = 0f;
        private double hextechDistance;
        private bool PositiveJaugeHextech = true;
        
        protected override bool AdditionalCastConditions(int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            return true;
        }

        protected override void Press(int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            time_Pressed = Time.time;
            hextechDistance = so.MinDistanceHFlash;
        }

        protected override void PressFeedback(int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            cam = Camera.main;
            layersToHit = 1 << 9 | 1 << 29;
            
            if (HelperDirection) HelperDirection.SetActive(true);
            else HelperDirection = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (UIJauge) UIJauge.gameObject.SetActive(true);
            else UIJauge = Object.Instantiate(so.prefabJauge).GetComponent<UIJauge>();
        }

        protected override void Hold(int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            
        }

        protected override void HoldFeedback(int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            if (!HelperDirection) return;
            HelperDirection.transform.position = casterPos + targetPositions[0].normalized + Vector3.up;
            if (UIJauge) UIJauge.UpdateJaugeSlider(so.MinDistanceHFlash, so.MaxDistanceHFlash,
                hextechDistance + (Time.time - time_Pressed) * so.HextechFlashSpeedScale);
        }

        protected override void Release(int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            time_Pressed =  (Time.time - time_Pressed ) * so.HextechFlashSpeedScale;
            hextechDistance += time_Pressed;
            if (hextechDistance > so.MaxDistanceHFlash) hextechDistance = so.MaxDistanceHFlash;
            targetPosition = GetClosestValidPoint(casterPos + targetPositions[0].normalized * (float)hextechDistance);
            targetPosition.y = 1;
            
            InitItemBag().ThrowBag(targetPosition,so.nbBounce,so.height,so.SpeedOnAir * 0.02f,new HealthModItem()); //TODO - Select Item
            if (UIJauge) UIJauge.gameObject.SetActive(false);
        }

        private ItemBag InitItemBag()
        {
            return Object.Instantiate(so.itemBagPrefab, caster.transform.position + Vector3.up, Quaternion.identity).GetComponent<ItemBag>();
        }

        protected override void ReleaseFeedback(int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            if (HelperDirection) HelperDirection.SetActive(false);
            if (UIJauge) UIJauge.gameObject.SetActive(false);
        }
    }
}


