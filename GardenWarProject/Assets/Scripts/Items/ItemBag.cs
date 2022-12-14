using System.Net;
using Entities;
using Entities.Capacities;
using Entities.Inventory;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class ItemBag : Bag
{
    [Header("Visual")]
    [SerializeField] protected Image image;
    [SerializeField] protected Transform canvasTr;
    
    protected Enums.Team team;
    protected byte itemSoIndex;

    public void SetItemBag(ScavengerThrowSO so,byte _itemSoIndex)
    {
        itemSoIndex = _itemSoIndex;
        team = thrower.team;

        if (!so) return;
        
        nbBounce = so.nbBounce;
        height = so.height;
        
        moveSpeed = so.SpeedOnAir;
    }
    
    public void SetItemBag(byte _itemSoIndex, Enums.Team _team = Enums.Team.Neutral)
    {
        itemSoIndex = _itemSoIndex;
        team = _team;
    }

    public override void ChangeVisuals(bool show)
    {
        gameObject.SetActive(show);
        if(!show) return;
        var canRotation = Camera.main.transform.rotation;
        canvasTr.LookAt(canvasTr.position + canRotation * Vector3.forward, canRotation * Vector3.up);
        image.sprite = ItemCollectionManager.Instance.GetItemSObyIndex(itemSoIndex).sprite;
    }

    private void ResetValue()
    {
        itemSoIndex = 0;
        team = Enums.Team.Neutral;
        
        nbBounce = 0;
        height = 0;
        
        moveSpeed = 0f; 
    }

    protected override void  RecoltBag(bool finished, Entity thrower)
    {
        if(collidedEntity.team != team) return;
        if(!collidedEntity.CanAddItem(itemSoIndex)) return;
        collidedEntity.AddItemRPC(itemSoIndex);

        ResetValue();
        
        if (isOffline)
        {
            ChangeVisualsRPC(false);
            return;
        }
        
        
        photonView.RPC("ChangeVisualsRPC",RpcTarget.All,false);
    }
}
