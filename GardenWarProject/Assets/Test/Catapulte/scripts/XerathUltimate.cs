using System;
using Entities;
using Entities.Capacities;
using GameStates;
using UnityEngine;

public class XerathUltimate : ActiveCapacity
{
    private XerathUltimateSO activeCapa = new XerathUltimateSO();
    private Vector3 startPosition;
    private Vector3 GoalPosition;

    private GameObject candyBag = null;
    private GameObject HelperDirection = null;
    private double Animation = 0f;
    private Plane plane = new Plane(Vector3.up, 0);

    private bool IsHextech = false;
    private int nbBounce = 0;
    private float height = 0;
    private float radiusRandom;
    private float ReduceSpeed;
    private Vector3 dir = Vector3.zero;
    private float time_Pressed = 0f;
    private double hextechDistance;
    private bool PositiveJaugeHextech = true;
    private GameStateMachine sm;

    public void Init(int casterIndex)
    {
        //candyBag = GameObject.Instantiate();
        
        caster = EntityCollectionManager.GetEntityByIndex(casterIndex);
        sm = GameStateMachine.Instance;
        IsHextech = activeCapa.IsHextechFlash;
        startPosition = caster.transform.position;
        nbBounce = activeCapa.nbBounce;
        height = activeCapa.height;
        radiusRandom = activeCapa.RandomizeZoneRadius;
        ReduceSpeed = activeCapa.SpeedOnAir;
        dir = (GoalPosition - startPosition).normalized;
        hextechDistance = 0f;
        if (IsHextech)
            hextechDistance = activeCapa.MinDistanceHFlash;
    }

    public class ParabolaClass
    {
        public static Vector3 Parabola(Vector3 start, Vector3 end, float height, double t)
        {
            Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

            var mid = Vector3.Lerp(start, end, (float)t);

            return new Vector3(mid.x, f((float)t) + Mathf.Lerp(start.y, end.y, (float)t), mid.z);
        }
    }

    public void MoveBag()
    {
        if (Vector3.Distance(candyBag.transform.position, GoalPosition) < 0.3f)
        {
            if (nbBounce > 0)
            {
                Animation = 0f;
                height /= 1.5f;
                radiusRandom /= 2f;
                startPosition = candyBag.transform.position;
                GoalPosition = new Vector3(GoalPosition.x + nbBounce * dir.x + dir.x * (float)hextechDistance,
                    GoalPosition.y,
                    GoalPosition.z + nbBounce * dir.z + dir.z * (float)hextechDistance);
                if (activeCapa.RandomizeZone)
                    GoalPosition += (UnityEngine.Random.insideUnitSphere * radiusRandom);
                GoalPosition = GetClosestValidPoint(new Vector3(GoalPosition.x, 0, GoalPosition.z));
                nbBounce--;
                ReduceSpeed *= 1.1f;
            }
            else
            {
                GameStateMachine.Instance.OnTick -= MoveBag;
            }

            Animation += sm.tickRate;
            Animation %= ReduceSpeed;
            candyBag.transform.position =
                ParabolaClass.Parabola(startPosition, GoalPosition, height, Animation / ReduceSpeed);
        }
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


    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        activeCapa = (XerathUltimateSO)AssociatedActiveCapacitySO();
        candyBag = PoolLocalManager.Instance.PoolInstantiate(activeCapa.prefab,
            caster.transform.position + Vector3.up * 2f, Quaternion.identity);
        Debug.Log("Ulti Xerath launched");

        if (!candyBag) return;

        GameStateMachine.Instance.OnTick += MoveBag;
    }


    public override bool TryCast(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        if (!base.TryCast(casterIndex, targetsEntityIndexes, targetPositions)) return false;
        Debug.Log("Performed Ulti Xerath at " + Time.time);

        //if (Vector3.Distance(targetPositions[0], caster.transform.position) > activeCapa.maxRange){return false;}

        GoalPosition = GetClosestValidPoint(targetPositions[0]);
        startPosition = caster.transform.position;


        return true;
    }

    public void Jauge()
    {
        if (PositiveJaugeHextech)
        {
            if (hextechDistance < activeCapa.MaxDistanceHFlash)
                hextechDistance += sm.tickRate * activeCapa.HextechFlashSpeedScale;
            else
                PositiveJaugeHextech = false;
        }
        else
        {
            if (hextechDistance > activeCapa.MinDistanceHFlash)
                hextechDistance -= sm.tickRate * activeCapa.HextechFlashSpeedScale;
            else
                PositiveJaugeHextech = true;
        }
    }

    protected override void Press(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        time_Pressed = Time.time;
        if (activeCapa.hextechMode == HextechMode.jauge)
        {
            PositiveJaugeHextech = true;
            GameStateMachine.Instance.OnTick += Jauge;
        }
    }

    protected override void PressFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        if(HelperDirection)
        HelperDirection.SetActive(true);
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
        Init(casterIndex);
        time_Pressed = Time.time - time_Pressed;
        switch (activeCapa.hextechMode)
        {
            case HextechMode.hold:
                hextechDistance += time_Pressed;
                if (hextechDistance > activeCapa.MaxDistanceHFlash)
                    hextechDistance = activeCapa.MaxDistanceHFlash;
                break;

            case HextechMode.jauge:
                GameStateMachine.Instance.OnTick -= Jauge;
                break;

            case HextechMode.mouseDistance:
                float mouseDist = Vector3.Distance(startPosition, getDirByMousePosition());
                
                if (activeCapa.RatioMouseDistance != 0f)
                    mouseDist /= activeCapa.RatioMouseDistance;
                
                GoalPosition = startPosition + getDirByMousePosition().normalized * mouseDist;
                break;
        }

        GameStateMachine.Instance.OnTick += MoveBag;
    }

    protected override void ReleaseFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {

        if (HelperDirection)
            HelperDirection.SetActive(false);
    }
}