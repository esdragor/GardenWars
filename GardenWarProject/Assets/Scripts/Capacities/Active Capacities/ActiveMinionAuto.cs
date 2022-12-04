using Entities;
using Entities.Capacities;
using GameStates;
using UnityEngine;

public class ActiveMinionAuto : ActiveCapacity
{
    private Entity _target;
    private MinionTest _minion;
    private double timer;
    
    public override bool TryCast(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        return true;
    }

    protected override void Press(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        _minion = caster.GetComponent<MinionTest>();
        _target = _minion.currentAttackTarget.GetComponent<Entity>();

        if (Vector3.Distance(_minion.transform.position, _target.transform.position) > _minion.attackRange) return;
        
        GameStateMachine.Instance.OnTick += DelayWaitingTick;
    }

    protected override void PressFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        
    }

    protected override void Hold(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        
    }

    protected override void HoldFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        
    }

    protected override void Release(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        
    }

    protected override void ReleaseFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        
    }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
    }
    
    private void DelayWaitingTick()
    {
        timer += 1 / GameStateMachine.Instance.tickRate;

        if (timer >= _minion.delayBeforeAttack) 
        {
            ApplyEffect();
            GameStateMachine.Instance.OnTick -= DelayWaitingTick;
        }
    }

    private void ApplyEffect()
    {
        if (Vector3.Distance(_target.transform.position, _minion.transform.position) < _minion.attackRange)
        {
            IActiveLifeable entityActiveLifeable = _target.GetComponent<IActiveLifeable>();
            entityActiveLifeable.DecreaseCurrentHpRPC(_minion.attackDamage);
        }
    }
}