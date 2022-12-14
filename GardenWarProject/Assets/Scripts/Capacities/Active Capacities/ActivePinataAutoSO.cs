using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Entities;
using Entities.Capacities;
using GameStates;
using UnityEngine;

[CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/Auto Attack Pinata", fileName = "AI Auto Attack for Pinata")]
public class ActivePinataAutoSO : ActiveCapacitySO
{
    
    public int AtkValue;
    public GameObject ItemBagPrefab;
    public float Speed = 2.5f;
    public override Type AssociatedType()
    {
        return typeof(ActivePinataAutoSO);
    }
}

public class ActivePinataAuto : ActiveCapacity
{
    private Entity _target;
    private double timer;

    protected override bool AdditionalCastConditions(int targetsEntityIndexes, Vector3 targetPositions)
    {
        return true;
    }

    protected override void Press(int targetsEntityIndexes, Vector3 targetPositions)
    {
        GameStateMachine.Instance.OnTick += DelayWaitingTick;
    }

    protected override void PressFeedback(int targetsEntityIndexes, Vector3 targetPositions)
    {
        
    }

    protected override void Hold(int targetsEntityIndexes, Vector3 targetPositions)
    {
        
    }

    protected override void HoldFeedback(int targetsEntityIndexes, Vector3 targetPositions)
    {
        
    }

    protected override void Release(int targetsEntityIndexes, Vector3 targetPositions)
    {
        
    }

    protected override void ReleaseFeedback(int targetEntityIndex, Vector3 targetPositions)
    {
        
    }

    private void DelayWaitingTick()
    {
        timer += 1 / GameStateMachine.Instance.tickRate;

        //   if (timer >= _minion.delayBeforeAttack) 
        {
            ApplyEffect();
            GameStateMachine.Instance.OnTick -= DelayWaitingTick;
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
}
