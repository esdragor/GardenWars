using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Entities.Capacities;
using GameStates;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class XerathUltimate : ActiveCapacity
{
    private XerathUltimateSO activeCapa;
    private Vector3 startPosition;
    private Vector3 GoalPosition;

    private GameObject candyBag = null;
    private float Animation = 0f;
    
    public class ParabolaClass
    {
        public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
        {
            Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

            var mid = Vector3.Lerp(start, end, t);

            return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
        }
    }

    public void MoveBag()
    {
        Animation += Time.deltaTime * activeCapa.ReduceSpeed;
        
        //Animation = Animation %activeCapa.ReduceSpeed;

        candyBag.transform.position = ParabolaClass.Parabola(caster.transform.position + Vector3.up * 4f, GoalPosition,
            activeCapa.height, Animation);
        if (Vector3.Distance(candyBag.transform.position, GoalPosition) < 1f)
            GameStateMachine.Instance.OnTick -= MoveBag;
    }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        activeCapa = (XerathUltimateSO)AssociatedActiveCapacitySO();
        candyBag = PoolLocalManager.Instance.PoolInstantiate(activeCapa.prefab, caster.transform.position + Vector3.up*2f, Quaternion.identity);
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
    
}
