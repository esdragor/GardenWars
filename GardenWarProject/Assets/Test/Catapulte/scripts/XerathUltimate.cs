using System;
using Entities;
using Entities.Capacities;
using ExitGames.Client.Photon.StructWrapping;
using GameStates;
using UnityEngine;
using Object = UnityEngine.Object;

public class XerathUltimate : ActiveCapacity
{
    private XerathUltimateSO activeCapa => (XerathUltimateSO)AssociatedActiveCapacitySO();
    private Vector3 startPosition;
    private Vector3 GoalPosition;

    private GameObject candyBag = null;
    private GameObject HelperDirection = null;
    private double Animation = 0f;
    private Plane plane = new Plane(Vector3.up, 0);

    private bool IsHextech = false;
    private float time_Pressed = 0f;
    private double hextechDistance;
    private bool PositiveJaugeHextech = true;
    private GameStateMachine sm;

    public void Init()
    {
        candyBag = Object.Instantiate(activeCapa.prefab, caster.transform.position + Vector3.up * 1, Quaternion.identity);
        sm = GameStateMachine.Instance;
        IsHextech = true;
        hextechDistance = 0f;
        if (IsHextech)
            hextechDistance = activeCapa.MinDistanceHFlash;
    }

 
    public Vector3 getDirByMousePosition()
    {
        float dist;
        Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray2, out dist))
        {
            Vector3 worldPosition = ray2.GetPoint(dist);
            return (new Vector3(worldPosition.x, 0, worldPosition.z) - startPosition);
        }

        return Vector3.zero;
    }

    public void Jauge()
    {
        if (PositiveJaugeHextech)
        {
            if (hextechDistance < activeCapa.MaxDistanceHFlash)
                hextechDistance += activeCapa.HextechFlashSpeedScale;
            else
                PositiveJaugeHextech = false;
        }
        else
        {
            if (hextechDistance > activeCapa.MinDistanceHFlash)
                hextechDistance -= activeCapa.HextechFlashSpeedScale;
            else
                PositiveJaugeHextech = true;
        }
        Debug.Log(hextechDistance);
    }

    protected override bool AdditionalCastConditions(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        return true;
    }

    protected override void Press(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        time_Pressed = Time.time;
        if (activeCapa.hextechMode == HextechMode.jauge)
        {
            PositiveJaugeHextech = true;
            hextechDistance = activeCapa.MinDistanceHFlash;
            GameStateMachine.Instance.OnTick += Jauge;
        }
    }

    protected override void PressFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        if(HelperDirection) HelperDirection.SetActive(true);
    }

    protected override void Hold(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        
    }

    protected override void HoldFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        if (!HelperDirection) return;
        if (activeCapa.hextechMode != HextechMode.mouseDistance)
            HelperDirection.transform.position = startPosition + getDirByMousePosition().normalized;
        else
            HelperDirection.transform.position = startPosition + getDirByMousePosition();
        ;
    }

    protected override void Release(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        Init();
        time_Pressed = Time.time - time_Pressed;
        switch (activeCapa.hextechMode)
        {
            case HextechMode.hold:
                hextechDistance += time_Pressed;
                if (hextechDistance > activeCapa.MaxDistanceHFlash)
                    hextechDistance = activeCapa.MaxDistanceHFlash;
                GoalPosition = GetClosestValidPoint(startPosition + getDirByMousePosition().normalized * (float)hextechDistance);
                break;

            case HextechMode.jauge:
                GameStateMachine.Instance.OnTick -= Jauge;
                GoalPosition = GetClosestValidPoint(startPosition + getDirByMousePosition().normalized * (float)hextechDistance);
                break;

            case HextechMode.mouseDistance:
                float mouseDist = Vector3.Distance(startPosition, getDirByMousePosition());
                
                if (activeCapa.RatioMouseDistance != 0f)
                    mouseDist /= activeCapa.RatioMouseDistance;
                
                GoalPosition = GetClosestValidPoint(startPosition + getDirByMousePosition().normalized * mouseDist);
                break;
        }

        candyBag.GetComponent<CandyBagXerath>().Init(caster, activeCapa, GoalPosition, hextechDistance);
    }

    protected override void ReleaseFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {

        if (HelperDirection)
            HelperDirection.SetActive(false);
    }
}