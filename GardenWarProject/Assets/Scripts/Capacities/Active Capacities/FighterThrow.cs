using System.Collections;
using System.Collections.Generic;
using Entities.Capacities;
using Entities.Inventory;
using Photon.Pun;
using UnityEngine;

public class FighterThrow : ActiveCapacity
{
    private FighterThrowSO so => (FighterThrowSO)AssociatedActiveCapacitySO();
    private Entities.Champion.Champion champion;

    private Vector3 targetPosition;

    private GameObject HelperDirection = null;
    private UIJauge UIJauge = null;

    private float time_Pressed = 0f;
    private double hextechDistance;

    private Item itemToThrow;
    
    protected override bool AdditionalCastConditions(int targetsEntityIndexes, Vector3 targetPositions)
    {
        champion = caster.GetComponent<Entities.Champion.Champion>();
        if (champion == null) return false;
        if (!champion.isFighter) return false;
        return champion.GetItem(champion.selectedItemIndex) != null;
    }

    protected override void Press(int targetsEntityIndexes, Vector3 targetPositions)
    {
        time_Pressed = Time.time;
        hextechDistance = so.MinDistanceHFlash;
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
            hextechDistance + (Time.time - time_Pressed) * so.HextechFlashSpeedScale);
    }

    private CandyBag InitCandyBag()
    {
        return !PhotonNetwork.IsConnected ? 
            Object.Instantiate(so.candyBagPrefab, caster.transform.position + Vector3.up, Quaternion.identity).GetComponent<CandyBag>() : 
            PhotonNetwork.Instantiate(so.candyBagPrefab.name, caster.transform.position + Vector3.up, Quaternion.identity).GetComponent<CandyBag>();
    }
    
    protected override void Release(int targetsEntityIndexes, Vector3 targetPositions)
    {
        time_Pressed =  (Time.time - time_Pressed ) * so.HextechFlashSpeedScale;
        hextechDistance += time_Pressed;
        if (hextechDistance > so.MaxDistanceHFlash) hextechDistance = so.MaxDistanceHFlash;
        targetPosition = GetClosestValidPoint(casterPos + (targetPositions - casterPos).normalized * (float)hextechDistance);
        targetPosition.y = 1;
        if (UIJauge) UIJauge.gameObject.SetActive(false);
        
        
    }

    protected override void ReleaseFeedback(int targetEntityIndex, Vector3 targetPositions)
    {
        champion.PlayThrowAnimation();
        if (HelperDirection) HelperDirection.SetActive(false);
        if (UIJauge) UIJauge.gameObject.SetActive(false);
    }
}
