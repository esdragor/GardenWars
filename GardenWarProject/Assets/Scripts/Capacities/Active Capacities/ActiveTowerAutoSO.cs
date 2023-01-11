using System;
using Entities;
using Entities.Capacities;
using GameStates;
using UnityEngine;

[CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/Auto Attack Tower", fileName = "AI Auto Attack for Tower")]
public class ActiveTowerAutoSO  : ActiveCapacitySO
{
    public int AtkValue;
    
    public override Type AssociatedType()
    {
        return typeof(ActiveTowerAuto);
    }
}

public class ActiveTowerAuto : ActiveCapacity
{
    private Entity _target;
    private double timer;
    
    private void DelayWaitingTick()
    {
        timer += 1 / GameStateMachine.Instance.tickRate;

        //   if (timer >= _minion.delayBeforeAttack) 
        {
            ApplyEffect();
            gsm.OnTick -= DelayWaitingTick;
        }
    }

    private void ApplyEffect()
    {
        //   if (Vector3.Distance(_target.transform.position, _minion.transform.position) < _minion.attackRange)
        {
            IActiveLifeable entityActiveLifeable = _target.GetComponent<IActiveLifeable>();
            //         entityActiveLifeable.DecreaseCurrentHpRPC(_minion.attackDamage);
        }
    }

    protected override bool AdditionalCastConditions(int targetsEntityIndexes, Vector3 targetPositions)
    {
        return true;
    }

    protected override void Press(int targetsEntityIndexes, Vector3 targetPositions)
    {
        gsm.OnTick += DelayWaitingTick;
    }

    protected override void PressFeedback(int targetsEntityIndexes, Vector3 targetPositions)
    {
        
    }

    protected override void PressClient(int targetsEntityIndexes, Vector3 targetPositions)
    {
        
    }

    protected override void Hold(int targetsEntityIndexes, Vector3 targetPositions)
    {
        
    }

    protected override void HoldFeedback(int targetsEntityIndexes, Vector3 targetPositions)
    {
        
    }

    protected override void HoldClient(int targetsEntityIndexes, Vector3 targetPositions)
    {
    }

    protected override void Release(int targetsEntityIndexes, Vector3 targetPositions)
    {

    }

    protected override void ReleaseFeedback(int targetEntityIndex, Vector3 targetPositions)
    {
        
    }

    protected override void ReleaseClient(int targetEntityIndex, Vector3 targetPositions)
    {
    }
}
