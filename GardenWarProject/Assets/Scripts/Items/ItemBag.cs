using System;
using Entities.Capacities;
using Entities.Champion;
using Entities.Inventory;
using GameStates;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class ItemBag : MonoBehaviourPun
{
    private static bool isOffline => !PhotonNetwork.IsConnected;
    private static bool isMaster => isOffline || PhotonNetwork.IsMasterClient;
    
    [Header("Visual")]
    [SerializeField] private Image image;
    [SerializeField] private Transform canvasTr;

    private Enums.Team team;
    private byte associatedSO;
    private bool canBePickedUp;

    private double Animation = 0f;
    private int nbBounce = 0;
    private float height = 0;
    private float reduceSpeed;
    private Vector3 dir = Vector3.zero;
    private double hextechDistance;
    private Vector3 startPosition;
    private Vector3 endPosition;

    private GameStateMachine gsm => GameStateMachine.Instance;
    
    public void ThrowBag(Vector3 targetPosition, ScavengerThrowSO so, byte associatedTeam, byte itemSoIndex, double _speed)
    {
        startPosition = transform.position;
        endPosition = targetPosition;
        
        nbBounce = so.nbBounce;
        height = so.height;
        
        reduceSpeed = so.SpeedOnAir * 0.02f;

        canBePickedUp = false;

        hextechDistance = _speed;
        dir = (endPosition - startPosition).normalized;
        
        team = (Enums.Team)associatedTeam;
        associatedSO = itemSoIndex;

        GameStateMachine.Instance.OnUpdate += MoveBag;

        if (isOffline)
        {
            ChangeVisualsRPC(itemSoIndex,true);
            return;
        }
        
        photonView.RPC("ChangeVisualsRPC",RpcTarget.All,itemSoIndex,true);
    }

    [PunRPC]
    private void ChangeVisualsRPC(byte itemSoIndex,bool show)
    {
        gameObject.SetActive(show);
        if (!show) return;
        var canRotation = Camera.main.transform.rotation;
        canvasTr.LookAt(canvasTr.position + canRotation * Vector3.forward, canRotation * Vector3.up);
        image.sprite = ItemCollectionManager.Instance.GetItemSObyIndex(itemSoIndex).sprite;
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
        if (Animation > 0.5f) canBePickedUp = true;
        
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
                endPosition = ActiveCapacity.GetClosestValidPoint(endPosition);
                endPosition.y = 1f;
                nbBounce--;
                reduceSpeed *= 0.9f;
            }
            else
            {
                GameStateMachine.Instance.OnUpdate -= MoveBag;
            }
        }
        Animation += (1 / gsm.tickRate) * reduceSpeed;
        transform.position = ParabolaClass.Parabola(startPosition, endPosition, height, Animation);
    }

    private void OnCollisionEnter(Collision other)
    {
        if(!canBePickedUp) return;
        var champion = other.collider.GetComponent<Champion>();
        Debug.Log($"Collided with {champion}");
        if(champion == null) return;
        if(champion.team != team) return;
        if(!champion.CanAddItem(associatedSO)) return;
        champion.AddItemRPC(associatedSO);
        if (isOffline)
        {
            ChangeVisualsRPC(associatedSO,false);
            return;
        }
        photonView.RPC("ChangeVisualsRPC",RpcTarget.All,associatedSO,false);
    }
}
