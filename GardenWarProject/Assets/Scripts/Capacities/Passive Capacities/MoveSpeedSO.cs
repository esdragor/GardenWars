using System;
using System.Collections;
using System.Collections.Generic;
using Entities;
using Entities.Capacities;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Capacity/PassiveCapacitySO/MoveSpeed", fileName = "MoveSpeed")]
public class MoveSpeedSO : PassiveCapacitySO
{
    public override Type AssociatedType()
    {
        return typeof(MoveSpeed);
    }
        
public float moveSpeed = 5f;
}

public class MoveSpeed : PassiveCapacity
{
    private MoveSpeedSO passiveCapacitySo;
    public override PassiveCapacitySO AssociatedPassiveCapacitySO()
    {
        return CapacitySOCollectionManager.Instance.GetPassiveCapacitySOByIndex(indexOfSo);
    }

    protected override void OnAddedEffects(Entity target)
    {
        passiveCapacitySo = (MoveSpeedSO)AssociatedPassiveCapacitySO();
        target.GetComponent<NavMeshAgent>().speed += passiveCapacitySo.moveSpeed;
    }

    protected override void OnAddedFeedbackEffects(Entity target)
    {
    }

    protected override void OnRemovedEffects(Entity target)
    {
        target.GetComponent<NavMeshAgent>().speed -= passiveCapacitySo.moveSpeed;
    }

    protected override void OnRemovedFeedbackEffects(Entity target)
    {
    }
}
