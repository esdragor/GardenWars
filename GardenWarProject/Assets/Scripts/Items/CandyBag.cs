using System;
using Entities;
using Entities.Champion;
using Photon.Pun;

public class CandyBag : Bag
{
    private int amount;
    private FighterThrowSO so; 

    public void SetCandyBag(FighterThrowSO so, int _amount)
    {
        nbBounce = so.nbBounce;
        height = so.height;
        
        speedDecreaseInAir = so.SpeedOnAir;
        amount = _amount;
        this.so = so;
    }

    protected override void ChangeVisuals(bool show)
    {
        gameObject.SetActive(show);
    }

    protected override void RecoltBag(bool finished, Entity thrower)
    {
        IActiveLifeable entityLife = collidedEntity.GetComponent<IActiveLifeable>();
        if (!finished && entityLife != null)
        {
            float damage = (so.NbCandyTriggerDamage <= amount) ? so.scaleAndDamageByNbCandyOnBag * (so.scaleByCandy * amount) : so.damageCandyScale * (so.scaleByCandy * amount);
            entityLife.DecreaseCurrentHpRPC(damage, thrower.entityIndex);
        }
        Champion champion = collidedEntity.GetComponent<Champion>();
        if (!champion) return;
        champion.IncreaseCurrentCandyRPC(amount);
        


        nbBounce = 0;
        height = 0;
        
        speedDecreaseInAir = 0f;
        amount = 0;
        
        photonView.RPC("ChangeVisualsRPC",RpcTarget.All,false);
    }
}
