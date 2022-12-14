using Entities.Capacities;
using Entities.Inventory;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class ItemBag : BagParent
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
        
        nbBounce = so.nbBounce;
        height = so.height;
        
        speedDecreaseInAir = so.SpeedOnAir * 0.02f;
    }

    protected override void ChangeVisuals(bool show)
    {
        gameObject.SetActive(show);
        if(!show) return;
        var canRotation = Camera.main.transform.rotation;
        canvasTr.LookAt(canvasTr.position + canRotation * Vector3.forward, canRotation * Vector3.up);
        image.sprite = ItemCollectionManager.Instance.GetItemSObyIndex(itemSoIndex).sprite;
    }

    protected override void RecoltBag()
    {
        if(collidedEntity.team != team) return;
        if(!collidedEntity.CanAddItem(itemSoIndex)) return;
        collidedEntity.AddItemRPC(itemSoIndex);
        if (isOffline)
        {
            ChangeVisualsRPC(false);
            return;
        }
        photonView.RPC("ChangeVisualsRPC",RpcTarget.All,false);
    }
}
