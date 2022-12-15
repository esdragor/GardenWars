using System.Collections;
using System.Collections.Generic;
using Entities;
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
    private int nbCandyStocked = 0;

    private double acceleration = 0.1;
    private float distanceCandy = 10f;

    protected override bool AdditionalCastConditions(int targetsEntityIndexes, Vector3 targetPositions)
    {
        champion = caster.GetComponent<Entities.Champion.Champion>();
        if (champion == null) return false;
        if (!champion.isFighter) return false;
        return (champion.currentCandy > 0);
    }

    protected override void Press(int targetsEntityIndexes, Vector3 targetPositions)
    {
        time_Pressed = Time.time;
        nbCandyStocked = so.MinCandy;
    }

    protected override void PressFeedback(int targetsEntityIndexes, Vector3 targetPositions)
    {
        if (HelperDirection) HelperDirection.SetActive(true);
        else HelperDirection = GameObject.CreatePrimitive(PrimitiveType.Cube);
    }

    protected override void Hold(int targetsEntityIndexes, Vector3 targetPositions)
    {
    }

    protected override void HoldFeedback(int targetsEntityIndexes, Vector3 targetPositions)
    {
        if (!HelperDirection) return;
        HelperDirection.transform.position = casterPos + (targetPositions - casterPos).normalized + Vector3.up;

        acceleration += (Time.time - time_Pressed) * so.accelerationJauge;

        nbCandyStocked = so.MinCandy + Mathf.RoundToInt((float)acceleration);
    }

    private CandyBag InitCandyBag()
    {
        return !PhotonNetwork.IsConnected
            ? Object.Instantiate(so.candyBagPrefab, caster.transform.position + Vector3.up, Quaternion.identity)
                .GetComponent<CandyBag>()
            : PhotonNetwork
                .Instantiate(so.candyBagPrefab.name, caster.transform.position + Vector3.up, Quaternion.identity)
                .GetComponent<CandyBag>();
    }

    protected override void Release(int targetsEntityIndexes, Vector3 targetPositions)
    {
        if (nbCandyStocked > so.MaxCandy) nbCandyStocked = so.MaxCandy;
        if (nbCandyStocked > champion.currentCandy) nbCandyStocked = champion.currentCandy;
        targetPosition.x *= so.DecalRandomizeCenter.x;
        targetPosition.z *= so.DecalRandomizeCenter.z;
        targetPosition = GetClosestValidPoint(targetPositions);
        targetPosition.y = 1;

        champion.RequestDecreaseCurrentCandy(nbCandyStocked);

        var candyBag = InitCandyBag();
        candyBag.transform.localScale =  Vector3.one * (!so.ScalebyNbCandy ? so.scaleByCandy * nbCandyStocked : (nbCandyStocked / so.NbCandyPerPalier) * so.scaleAndDamageByNbCandyOnBag);
        candyBag.InitBag(targetPosition, distanceCandy, so.RandomizeRebound, so.RandomizeReboundRadius, caster);
        candyBag.SetCandyBag(so, nbCandyStocked);
        candyBag.ThrowBag();
        if (UIJauge) UIJauge.gameObject.SetActive(false);
    }

    protected override void ReleaseFeedback(int targetEntityIndex, Vector3 targetPositions)
    {
        champion.PlayThrowAnimation();
        if (HelperDirection) HelperDirection.SetActive(false);
        if (UIJauge) UIJauge.gameObject.SetActive(false);
    }
}