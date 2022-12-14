using System;
using Entities;
using Entities.Capacities;
using Entities.Inventory;
using GameStates;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class Bag : MonoBehaviourPun
{
    protected static bool isOffline => !PhotonNetwork.IsConnected;
    protected static bool isMaster => isOffline || PhotonNetwork.IsMasterClient;

    protected Entity thrower;
    
    protected Entity collidedEntity;
    
    protected bool canBePickedUp;
    
    protected double Animation = 0f;
    protected int nbBounce = 0;
    protected float height = 0;
    protected float moveSpeed;
    protected Vector3 dir = Vector3.zero;
    protected double speed;
    protected float timeForOneUnit;
    protected float distanceToDestination;
    protected Vector3 startPosition;
    protected Vector3 endPosition;
    protected bool isRandom = false;
    protected float randomRangeRadius = 0f;
    private bool Finished = false;
    private GameStateMachine gsm => GameStateMachine.Instance;

    public void InitBag(Vector3 targetPosition,float _timeForOneUnit,double _speed, bool RandomPos, float randomRadius, Entity spawner)
    {
        thrower = spawner;
        
        startPosition = transform.position;
        endPosition = targetPosition;
        speed = _speed;
        timeForOneUnit = _timeForOneUnit;
        isRandom = RandomPos;
        randomRangeRadius = randomRadius;
        canBePickedUp = false;
    }

    public void ThrowBag()
    {
        dir = (endPosition - startPosition).normalized;

        distanceToDestination = (Vector3.Distance(endPosition, startPosition));

        gsm.OnUpdate += MoveBag;
        
        if (isOffline)
        {
            ChangeVisualsRPC(true);
            return;
        }
        
        photonView.RPC("ChangeVisualsRPC",RpcTarget.All,true);
        
    }

    [PunRPC]
    public void ChangeVisualsRPC(bool show)
    {
        ChangeVisuals(show);
    }

    public abstract void ChangeVisuals(bool show);

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
        if (Animation > 0.5f) IsCollectible();
        
        if (Animation > 0.99f)
        {
            if (nbBounce > 0)
            {
                Animation = 0f;
                height /= 1.5f;
                speed /= 4f;
                startPosition = endPosition;
                
                endPosition.x += (nbBounce *0.5f) * dir.x + dir.x * (float)speed + Random.Range(-randomRangeRadius, randomRangeRadius) * ((isRandom) ? 1 : 0);
                endPosition.z += (nbBounce *0.5f) * dir.z + dir.z * (float)speed + Random.Range(-randomRangeRadius, randomRangeRadius) * ((isRandom) ? 1 : 0);
                
                endPosition = ActiveCapacity.GetClosestValidPoint(endPosition);
                nbBounce--;
                moveSpeed *= 0.9f;
            }
            else
            {
                gsm.OnUpdate -= MoveBag;
                transform.position = new Vector3(transform.position.x, endPosition.y, transform.position.z);
                Finished = true;
                return;
            }
        }
        
        Animation += moveSpeed *(timeForOneUnit / distanceToDestination) * Time.deltaTime ;
        
        transform.position = ParabolaClass.Parabola(startPosition, endPosition, height, Animation);
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if(!canBePickedUp) return;

        collidedEntity = other.collider.GetComponent<Entity>();
        if (collidedEntity == null) return;

        Debug.Log($"Collided with {collidedEntity}");

        RecoltBag(Finished, thrower);

        
    }

    public void IsCollectible()
    {
        canBePickedUp = true;
    }
    
    protected abstract void RecoltBag(bool Finished, Entity thrower);
}
