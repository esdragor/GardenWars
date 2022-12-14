using System;
using Entities;
using Entities.Capacities;
using Entities.Inventory;
using GameStates;
using Photon.Pun;
using UnityEngine;

public abstract class BagParent : MonoBehaviourPun
{
    protected static bool isOffline => !PhotonNetwork.IsConnected;
    protected static bool isMaster => isOffline || PhotonNetwork.IsMasterClient;

    protected Entity thrower;
    
    protected Entity collidedEntity;
    protected IInventoryable entityInventory;
    
    protected bool canBePickedUp;
    
    protected double Animation = 0f;
    protected int nbBounce = 0;
    protected float height = 0;
    protected float speedDecreaseInAir;
    protected Vector3 dir = Vector3.zero;
    protected double speed;
    protected Vector3 startPosition;
    protected Vector3 endPosition;
    private GameStateMachine gsm => GameStateMachine.Instance;

    public void InitBag(Vector3 targetPosition,double _speed, Entity spawner)
    {
        thrower = spawner;
        
        startPosition = transform.position;
        endPosition = targetPosition;
        speed = _speed;
        
        canBePickedUp = false;
    }

    public void ThrowBag()
    {
        dir = (endPosition - startPosition).normalized;

        GameStateMachine.Instance.OnUpdate += MoveBag;
        
        if (isOffline)
        {
            ChangeVisualsRPC(true);
            return;
        }
        
        photonView.RPC("ChangeVisualsRPC",RpcTarget.All,true);
    }

    [PunRPC]
    protected void ChangeVisualsRPC(bool show)
    {
        ChangeVisuals(show);
    }

    protected abstract void ChangeVisuals(bool show);

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
        if (Animation > 0.5f) canBePickedUp = true;
        
        if (Animation > 0.99f)
        {
            if (nbBounce > 0)
            {
                Animation = 0f;
                height /= 1.5f;
                speed /= 4f;
                startPosition = endPosition;
                endPosition = new Vector3(endPosition.x + (nbBounce *0.5f) * dir.x + dir.x * (float)speed,
                    endPosition.y,
                    endPosition.z + (nbBounce *0.5f) * dir.z + dir.z * (float)speed);
                endPosition = ActiveCapacity.GetClosestValidPoint(endPosition);
                endPosition.y = 1f;
                nbBounce--;
                speedDecreaseInAir *= 0.9f;
            }
            else
            {
                GameStateMachine.Instance.OnUpdate -= MoveBag;
            }
        }
        Animation += (1 / gsm.tickRate) * speedDecreaseInAir;
        transform.position = ParabolaClass.Parabola(startPosition, endPosition, height, Animation);
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if(!canBePickedUp) return;
        collidedEntity = other.collider.GetComponent<Entity>();
        if (collidedEntity == null) return;
        entityInventory = collidedEntity.GetComponent<IInventoryable>();
        if(entityInventory == null) return;
        
        Debug.Log($"Collided with {collidedEntity}");

        RecoltBag();
    }
    
    protected abstract void RecoltBag();
}
