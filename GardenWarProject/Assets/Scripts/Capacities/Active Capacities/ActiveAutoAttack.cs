using Entities;
using Entities.Capacities;
using GameStates;
using UnityEngine;

public class ActiveAutoAttack : ActiveCapacity
{
    private ActiveAutoAttackSO activeAutoAttackSO; 
    private double attackTimer;
    private int target;

    protected override bool AdditionalCastConditions(int targetsEntityIndexes, Vector3 targetPositions)
    {
        return true;
    }

    protected override void Press(int targetsEntityIndexes, Vector3 targetPositions)
    {
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

    private void ApplyEffect()
    {
        IActiveLifeable activeLifeable = EntityCollectionManager.GetEntityByIndex(target).GetComponent<IActiveLifeable>();
        activeLifeable.DecreaseCurrentHpRPC(activeAutoAttackSO.damage);
    }

    public void DelayAutoAttack()
    {
        attackTimer += GameStateMachine.Instance.tickRate;
        if (attackTimer >= activeAutoAttackSO.attackSpeed)
        {
            attackTimer = 0;
            ApplyEffect();
        }
    }
    
}
