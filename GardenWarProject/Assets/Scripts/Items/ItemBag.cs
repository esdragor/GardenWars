using System;
using Entities.Capacities;
using Entities.Inventory;
using GameStates;
using UnityEngine;

public class ItemBag : MonoBehaviour
{
    private double Animation = 0f;

    private int nbBounce = 0;
    private float height = 0;
    private float reduceSpeed;
    private Vector3 dir = Vector3.zero;
    private double hextechDistance;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private bool RandomizeZone = false;
    
    private GameStateMachine gsm => GameStateMachine.Instance;
    
    public void ThrowBag(Vector3 targetPosition,int bounceCount,float heightValue,float speed, Item item)
    {
        startPosition = transform.position;
        endPosition = targetPosition;
        
        nbBounce = bounceCount;
        height = heightValue;
        
        reduceSpeed = speed * 0.02f;
        
        
        GameStateMachine.Instance.OnUpdate += MoveBag;
    }

    private class ParabolaClass
    {
        public static Vector3 Parabola(Vector3 start, Vector3 end, float height, double t)
        {
            Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

            var mid = Vector3.Lerp(start, end, (float)t);

            return new Vector3(mid.x, f((float)t) + Mathf.Lerp(start.y, end.y, (float)t), mid.z);
        }
    }

    private void MoveBag()
    {
        if (Animation > 0.99f)
        {
            if (nbBounce > 0)
            {
                Animation = 0f;
                height /= 1.5f;
                hextechDistance /= 4f;
                startPosition = endPosition;
                endPosition = new Vector3(endPosition.x + (nbBounce *0.5f) * dir.x + dir.x * (float)hextechDistance,
                    endPosition.y,
                    endPosition.z + (nbBounce *0.5f) * dir.z + dir.z * (float)hextechDistance);
                endPosition = ActiveCapacity.GetClosestValidPoint(new Vector3(endPosition.x, 0, endPosition.z));
                nbBounce--;
                reduceSpeed *= 1.1f;
            }
            else
            {
                GameStateMachine.Instance.OnUpdate -= MoveBag;
            }
        }
        Animation += (1 - gsm.tickRate / 100) * reduceSpeed;
        transform.position = ParabolaClass.Parabola(startPosition, endPosition, height, Animation);
    }
}
