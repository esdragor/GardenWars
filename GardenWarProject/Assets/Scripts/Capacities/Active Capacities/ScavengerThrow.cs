using Entities.Inventory;
using Photon.Pun;
using UnityEngine;

namespace Entities.Capacities
{
    public class ScavengerThrow : ActiveCapacity
    {
        private ScavengerThrowSO so => (ScavengerThrowSO)AssociatedActiveCapacitySO();
        private Champion.Champion champion;

        private Vector3 targetPosition;

        private GameObject HelperDirection = null;
        private UIJauge UIJauge = null;

        private float time_Pressed = 0f;
        private double bagSpeed;

        private Item itemToThrow;
        
        protected override bool AdditionalCastConditions(int targetsEntityIndexes, Vector3 targetPositions)
        {
            champion = caster.GetComponent<Champion.Champion>();
            if (champion == null) return false;
            if (champion.isFighter) return false;
            return champion.GetItem(champion.selectedItemIndex) != null;
        }

        protected override void Press(int targetsEntityIndexes, Vector3 targetPositions)
        {
            time_Pressed = Time.time;
            bagSpeed = so.MinDistanceHFlash;
        }

        protected override void PressFeedback(int targetsEntityIndexes, Vector3 targetPositions)
        {
            if (HelperDirection) HelperDirection.SetActive(true);
            else HelperDirection = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (UIJauge) UIJauge.gameObject.SetActive(true);
            else UIJauge = Object.Instantiate(so.prefabJauge).GetComponent<UIJauge>();
        }

        protected override void Hold(int targetsEntityIndexes, Vector3 targetPositions)
        {
            
        }

        protected override void HoldFeedback(int targetsEntityIndexes, Vector3 targetPositions)
        {
            if (!HelperDirection) return;
            HelperDirection.transform.position = casterPos + (targetPositions - casterPos).normalized + Vector3.up;
            if (UIJauge) UIJauge.UpdateJaugeSlider(so.MinDistanceHFlash, so.MaxDistanceHFlash,
                bagSpeed + (Time.time - time_Pressed) * so.HextechFlashSpeedScale);
        }

        private ItemBag InitItemBag()
        {
            return !PhotonNetwork.IsConnected ? Object.Instantiate(so.itemBagPrefab, caster.transform.position + Vector3.up, Quaternion.identity).GetComponent<ItemBag>() : PhotonNetwork.Instantiate(so.itemBagPrefab.name, caster.transform.position + Vector3.up, Quaternion.identity).GetComponent<ItemBag>();
        }

        
        protected override void Release(int targetsEntityIndexes, Vector3 targetPositions)
        {
            time_Pressed =  (Time.time - time_Pressed ) * so.HextechFlashSpeedScale;
            bagSpeed += time_Pressed;
            if (bagSpeed > so.MaxDistanceHFlash) bagSpeed = so.MaxDistanceHFlash;
            targetPosition = GetClosestValidPoint(casterPos + (targetPositions - casterPos).normalized * (float)bagSpeed);
            targetPosition.y = 1;

            itemToThrow = champion.PopSelectedItem();
            var itemBag = InitItemBag();
            itemBag.InitBag(targetPosition, bagSpeed, caster);
            itemBag.SetItemBag(so,itemToThrow.indexOfSOInCollection);
            itemBag.ThrowBag();
            if (UIJauge) UIJauge.gameObject.SetActive(false);
        }
        
        protected override void ReleaseFeedback(int targetEntityIndex, Vector3 targetPositions)
        {
            champion.PlayThrowAnimation();
            if (HelperDirection) HelperDirection.SetActive(false);
            if (UIJauge) UIJauge.gameObject.SetActive(false);
        }
    }
}


