using Entities;
using Entities.Capacities;
using GameStates;
using UnityEngine;

public class ActiveAutoAttack : ActiveCapacity
{
    
    private ActiveAutoAttackSO activeAutoAttackSO; 
    private double attackTimer;
    private int target;

    public override bool TryCast(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        return base.TryCast(casterIndex, targetsEntityIndexes, targetPositions);
    }

    protected override void Press(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        throw new System.NotImplementedException();
    }

    protected override void PressFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        throw new System.NotImplementedException();
    }

    protected override void Hold(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        throw new System.NotImplementedException();
    }

    protected override void HoldFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        throw new System.NotImplementedException();
    }

    protected override void Release(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        throw new System.NotImplementedException();
    }

    protected override void ReleaseFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        throw new System.NotImplementedException();
    }

    public override void PlayFeedback(int entityIndex, int[] targets, Vector3[] position)
    {
        Debug.Log("AutoAtk Feedback");
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
