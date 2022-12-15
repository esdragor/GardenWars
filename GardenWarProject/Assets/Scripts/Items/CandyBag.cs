using System;
using Entities.Champion;
using Photon.Pun;

public class CandyBag : Bag
{
    private int amount;
    
    public void SetCandyBag(FighterThrowSO so, int _amount)
    {
        nbBounce = so.nbBounce;
        height = so.height;
        
        speedDecreaseInAir = so.SpeedOnAir * 0.02f;
        amount = _amount;
    }
    
    protected override void ChangeVisuals(bool show)
    {
        gameObject.SetActive(show);
    }

    protected override void RecoltBag()
    {
        Champion champion = collidedEntity.GetComponent<Champion>();
        if (!champion) return;
        champion.IncreaseCurrentCandyRPC(amount);

        nbBounce = 0;
        height = 0;
        
        speedDecreaseInAir = 0.02f;
        amount = 0;
    }
}
