using System;
using System.Collections;
using System.Collections.Generic;
using Entities;
using Entities.Capacities;
using GameStates;
using UnityEngine;
using UnityEngine.AI;

public class CandyBagXerath : MonoBehaviour
{
    private GameObject HelperDirection = null;
    private double Animation = 0f;
    private Plane plane = new Plane(Vector3.up, 0);
    
    private bool IsHextech = false;
    private int nbBounce = 0;
    private float height = 0;
    private float radiusRandom;
    private float ReduceSpeed;
    private Vector3 dir = Vector3.zero;
    private double hextechDistance;
    private bool PositiveJaugeHextech = true;
    private GameStateMachine sm;
    private Entity caster;
    private Vector3 startPosition;
    private Vector3 GoalPosition;
    private bool RandomizeZone = false;
    
    public void Init(Entity _caster, XerathUltimateSO activeCapa, Vector3 Goal, double dist)
    {
        GoalPosition = Goal;
        

        caster = _caster;
        sm = GameStateMachine.Instance;
        IsHextech = true;
        startPosition = caster.transform.position;
        nbBounce = activeCapa.nbBounce;
        height = activeCapa.height;
        radiusRandom = activeCapa.RandomizeZoneRadius;
        ReduceSpeed = activeCapa.SpeedOnAir * 0.02f;
        dir = (GoalPosition - startPosition).normalized;
        hextechDistance = 0f;
        if (IsHextech)
            hextechDistance = dist;
        RandomizeZone = activeCapa.RandomizeZone;
        
        GameStateMachine.Instance.OnUpdate += MoveBag;
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
        if (Animation > 0.99f)
        {
            if (nbBounce > 0)
            {
                Animation = 0f;
                height /= 1.5f;
                radiusRandom /= 2f;
                hextechDistance /= 4f;
                startPosition = GoalPosition;
                GoalPosition = new Vector3(GoalPosition.x + (nbBounce *0.5f) * dir.x + dir.x * (float)hextechDistance,
                    GoalPosition.y,
                    GoalPosition.z + (nbBounce *0.5f) * dir.z + dir.z * (float)hextechDistance);
                if (RandomizeZone)
                    GoalPosition += (UnityEngine.Random.insideUnitSphere * radiusRandom);
                GoalPosition = ActiveCapacity.GetClosestValidPoint(new Vector3(GoalPosition.x, 0, GoalPosition.z));
                nbBounce--;
                ReduceSpeed *= 1.1f;
            }
            else
            {
                GameStateMachine.Instance.OnUpdate -= MoveBag;
            }
        }
        Animation += (1 - sm.tickRate / 100) * ReduceSpeed;
        transform.position =
            ParabolaClass.Parabola(startPosition, GoalPosition, height, Animation);
    }
}
