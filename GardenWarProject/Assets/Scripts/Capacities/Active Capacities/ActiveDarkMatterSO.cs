using System;
using GameStates;
using UnityEngine;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/Dark Matter", fileName = "new Dark Matter")]
    public class ActiveDarkMatterSO : ActiveCapacitySO
    {
        public float zoneRadius;
        public float damageAmount;
        public float delay;
    
        public override Type AssociatedType()
        {
            return typeof(ActiveDarkMatter);
        }
    }
    
    public class ActiveDarkMatter : ActiveCapacity
{
    private double timer;
    private ActiveDarkMatterSO activeCapacitySo;
    private Vector3 dir;

    protected override bool AdditionalCastConditions(int targetsEntityIndexes, Vector3 targetPositions)
    {
        return true;
    }

    protected override void Press(int targetsEntityIndexes, Vector3 position)
    {
        activeCapacitySo = (ActiveDarkMatterSO)AssociatedActiveCapacitySO();
        
        GameStateMachine.Instance.OnTick += DelayWaitingTick;
        
        dir = position;
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
        ITeamable casterTeam = caster.GetComponent<ITeamable>();
        
        Collider[] detected = Physics.OverlapSphere(dir, activeCapacitySo.zoneRadius);

        foreach (var hit in detected)
        {
            Entity entityTouch = hit.GetComponent<Entity>();

            if (entityTouch)
            {
                ITeamable entityTeam = entityTouch.GetComponent<ITeamable>();

                if (entityTeam.GetTeam() != casterTeam.GetTeam())
                {
                    IActiveLifeable entityActiveLifeable = entityTouch.GetComponent<IActiveLifeable>();

                    entityActiveLifeable.DecreaseCurrentHpRPC( activeCapacitySo.damageAmount, caster.entityIndex);
                }
            }
        }
        
        Debug.Log("Dark Matter is end at " + Time.time);
    }

    private void DelayWaitingTick()
    {
        timer += 1 / GameStateMachine.Instance.tickRate;

        if (timer >=  activeCapacitySo.delay)
        {
            Debug.Log("Delay is over at " + Time.time);
            ApplyEffect();
            GameStateMachine.Instance.OnTick -= DelayWaitingTick;
            timer = 0;
        }
    }
}

}


