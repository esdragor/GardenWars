using System;
using Entities;
using Entities.Capacities;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class Minion : Entity, IMoveable, IAttackable, IActiveLifeable, IDeadable
{
    [SerializeField] private bool canMove = true;
    [SerializeField] private bool canAttack = true;
    [SerializeField] private float attackValue = 5f;
    [SerializeField] private float referenceMoveSpeed;
    [SerializeField] private MyAIBT myAIBT;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float MaxHP = 100f;
    [SerializeField] private float CurrentHP = 100f;

    private float currentMoveSpeed;

    public float GetAttackSpeed()
    {
        return attackSpeed;
    }

    protected override void OnStart()
    {
        CurrentHP = MaxHP;
    }

    public override void OnInstantiated()
    {
        gameObject.SetActive(true);
        //myAIBT.enabled = true;
        currentMoveSpeed = referenceMoveSpeed;
        agent.speed = referenceMoveSpeed;
    }

    public bool CanMove()
    {
        return canMove;
    }

    public float GetReferenceMoveSpeed()
    {
        return referenceMoveSpeed;
    }

    public float GetCurrentMoveSpeed()
    {
        return currentMoveSpeed;
    }

    public void RequestSetCanMove(bool value)
    {
        photonView.RPC("SetCanMoveRPC", RpcTarget.MasterClient, value);
    }
    [PunRPC]
    public void SyncSetCanMoveRPC(bool value)
    {
        canMove = value;
        OnSetCanMove?.Invoke(canMove);
        OnSetCanMoveFeedback?.Invoke(canMove);
    }
    [PunRPC]
    public void SetCanMoveRPC(bool value)
    {
        canMove = value;
        photonView.RPC("SyncSetCanMoveRPC", RpcTarget.All, value);
    }

    public event GlobalDelegates.BoolDelegate OnSetCanMove;
    public event GlobalDelegates.BoolDelegate OnSetCanMoveFeedback;

    public void RequestSetReferenceMoveSpeed(float value)
    {
        photonView.RPC("SetReferenceMoveSpeedRPC", RpcTarget.MasterClient, value);
    }
    [PunRPC]
    public void SyncSetReferenceMoveSpeedRPC(float value)
    {
        referenceMoveSpeed = value;
        OnSetReferenceMoveSpeed?.Invoke(referenceMoveSpeed);
        OnSetReferenceMoveSpeedFeedback?.Invoke(referenceMoveSpeed);
    }
    [PunRPC]
    public void SetReferenceMoveSpeedRPC(float value)
    {
        referenceMoveSpeed = value;
        photonView.RPC("SyncSetReferenceMoveSpeedRPC", RpcTarget.All, value);
    }

    public event GlobalDelegates.FloatDelegate OnSetReferenceMoveSpeed;
    public event GlobalDelegates.FloatDelegate OnSetReferenceMoveSpeedFeedback;

    public void RequestIncreaseReferenceMoveSpeed(float amount)
    {
        photonView.RPC("IncreaseReferenceMoveSpeedRPC", RpcTarget.MasterClient, amount);
    }
    [PunRPC]
    public void SyncIncreaseReferenceMoveSpeedRPC(float amount)
    {
        referenceMoveSpeed += amount;

        OnIncreaseReferenceMoveSpeed?.Invoke(referenceMoveSpeed);
        OnIncreaseReferenceMoveSpeedFeedback?.Invoke(referenceMoveSpeed);
    }
    [PunRPC]
    public void IncreaseReferenceMoveSpeedRPC(float amount)
    {
        referenceMoveSpeed += amount;
        photonView.RPC("SyncIncreaseReferenceMoveSpeedRPC", RpcTarget.All, amount);
    }

    public event GlobalDelegates.FloatDelegate OnIncreaseReferenceMoveSpeed;
    public event GlobalDelegates.FloatDelegate OnIncreaseReferenceMoveSpeedFeedback;

    public void RequestDecreaseReferenceMoveSpeed(float amount)
    {
        photonView.RPC("DecreaseReferenceMoveSpeedRPC", RpcTarget.MasterClient, amount);
    }
    [PunRPC]
    public void SyncDecreaseReferenceMoveSpeedRPC(float amount)
    {
        referenceMoveSpeed -= amount;
        OnDecreaseReferenceMoveSpeed?.Invoke(referenceMoveSpeed);
        OnDecreaseReferenceMoveSpeedFeedback?.Invoke(referenceMoveSpeed);
    }
    [PunRPC]
    public void DecreaseReferenceMoveSpeedRPC(float amount)
    {
        referenceMoveSpeed -= amount;
        photonView.RPC("SyncDecreaseReferenceMoveSpeedRPC", RpcTarget.All, amount);
    }

    public event GlobalDelegates.FloatDelegate OnDecreaseReferenceMoveSpeed;
    public event GlobalDelegates.FloatDelegate OnDecreaseReferenceMoveSpeedFeedback;

    public void RequestSetCurrentMoveSpeed(float value)
    {
        photonView.RPC("SetCurrentMoveSpeedRPC", RpcTarget.MasterClient, value);
    }
    [PunRPC]
    public void SyncSetCurrentMoveSpeedRPC(float value)
    {
        currentMoveSpeed = value;
        agent.speed = currentMoveSpeed;
        OnSetCurrentMoveSpeed?.Invoke(referenceMoveSpeed);
        OnSetCurrentMoveSpeedFeedback?.Invoke(referenceMoveSpeed);
    }
    [PunRPC]
    public void SetCurrentMoveSpeedRPC(float value)
    {
        currentMoveSpeed = value;
        agent.speed = currentMoveSpeed;
        photonView.RPC("SyncSetCurrentMoveSpeedRPC", RpcTarget.All, value);
    }

    public event GlobalDelegates.FloatDelegate OnSetCurrentMoveSpeed;
    public event GlobalDelegates.FloatDelegate OnSetCurrentMoveSpeedFeedback;

    public void RequestIncreaseCurrentMoveSpeed(float amount)
    {
        photonView.RPC("IncreaseCurrentMoveSpeedRPC", RpcTarget.MasterClient, amount);
    }
    [PunRPC]
    public void SyncIncreaseCurrentMoveSpeedRPC(float amount)
    {
        currentMoveSpeed += amount;
        agent.speed = currentMoveSpeed;
        OnIncreaseCurrentMoveSpeed?.Invoke(referenceMoveSpeed);
        OnIncreaseCurrentMoveSpeedFeedback?.Invoke(referenceMoveSpeed);
    }
    [PunRPC]
    public void IncreaseCurrentMoveSpeedRPC(float amount)
    {
        currentMoveSpeed += amount;
        agent.speed = currentMoveSpeed;
        photonView.RPC("SyncIncreaseCurrentMoveSpeedRPC", RpcTarget.All, amount);
    }

    public event GlobalDelegates.FloatDelegate OnIncreaseCurrentMoveSpeed;
    public event GlobalDelegates.FloatDelegate OnIncreaseCurrentMoveSpeedFeedback;

    public void RequestDecreaseCurrentMoveSpeed(float amount)
    {
        photonView.RPC("DecreaseCurrentMoveSpeedRPC", RpcTarget.MasterClient, amount);
    }
    [PunRPC]
    public void SyncDecreaseCurrentMoveSpeedRPC(float amount)
    {
        currentMoveSpeed -= amount;
        agent.speed = currentMoveSpeed;
        OnDecreaseCurrentMoveSpeed?.Invoke(referenceMoveSpeed);
        OnDecreaseCurrentMoveSpeedFeedback?.Invoke(referenceMoveSpeed);
    }
    [PunRPC]
    public void DecreaseCurrentMoveSpeedRPC(float amount)
    {
        currentMoveSpeed -= amount;
        agent.speed = currentMoveSpeed;
        photonView.RPC("SyncDecreaseCurrentMoveSpeedRPC", RpcTarget.All, amount);
    }

    public event GlobalDelegates.FloatDelegate OnDecreaseCurrentMoveSpeed;
    public event GlobalDelegates.FloatDelegate OnDecreaseCurrentMoveSpeedFeedback;
    public event GlobalDelegates.Vector3Delegate OnMove;
    public event GlobalDelegates.Vector3Delegate OnMoveFeedback;

    public bool CanAttack()
    {
        return canAttack;
    }

    public void RequestSetCanAttack(bool value)
    {
        SetCanAttackRPC(value);
    }
    [PunRPC]
    public void SetCanAttackRPC(bool value)
    {
        canAttack = value;
        photonView.RPC("SyncSetCanAttackRPC", RpcTarget.All, value);
    }
    [PunRPC]
    public void SyncSetCanAttackRPC(bool value)
    {
        canAttack = value;
        OnSetCanAttack?.Invoke(canAttack);
        OnSetCanAttackFeedback?.Invoke(canAttack);
    }

    public event GlobalDelegates.BoolDelegate OnSetCanAttack;
    public event GlobalDelegates.BoolDelegate OnSetCanAttackFeedback;

    public float GetAttackDamage()
    {
        return attackValue;
    }

    public void RequestSetAttackDamage(float value)
    {
        SetAttackDamageRPC(value);
    }
    [PunRPC]
    public void SyncSetAttackDamageRPC(float value)
    {
        attackValue = value;
        OnSetAttackDamage?.Invoke(attackValue);
        OnSetAttackDamageFeedback?.Invoke(attackValue);
    }
    [PunRPC]
    public void SetAttackDamageRPC(float value)
    {
        attackValue = value;
        photonView.RPC("SyncSetAttackDamageRPC", RpcTarget.All, value);
    }

    public event GlobalDelegates.FloatDelegate OnSetAttackDamage;
    public event GlobalDelegates.FloatDelegate OnSetAttackDamageFeedback;

    public void RequestAttack(byte capacityIndex, int[] targetedEntities, Vector3[] targetedPositions)
    {
        AttackRPC(capacityIndex, targetedEntities, targetedPositions);
    }
    [PunRPC]
    public void SyncAttackRPC(byte capacityIndex, int[] targetedEntities, Vector3[] targetedPositions)
    {
        //int atkValue = CapacitySOCollectionManager.GetActiveCapacitySOByIndex(capacityIndex).AtkValue;
        for (int i = 0; i < targetedEntities.Length; i++)
        {
            Entity entity = EntityCollectionManager.GetEntityByIndex(targetedEntities[i]);
            entity.GetComponent<IActiveLifeable>().DecreaseCurrentHpRPC(attackValue);
        }

        OnAttack?.Invoke(capacityIndex, targetedEntities, targetedPositions);
        OnAttackFeedback?.Invoke(capacityIndex, targetedEntities, targetedPositions);
    }
    [PunRPC]
    public void AttackRPC(byte capacityIndex, int[] targetedEntities, Vector3[] targetedPositions)
    {
        //int atkValue = CapacitySOCollectionManager.GetActiveCapacitySOByIndex(capacityIndex).AtkValue;
        for (int i = 0; i < targetedEntities.Length; i++)
        {
            Entity entity = EntityCollectionManager.GetEntityByIndex(targetedEntities[i]);
            entity.GetComponent<IActiveLifeable>().DecreaseCurrentHpRPC(attackValue);
        }
        photonView.RPC("SyncAttackRPC", RpcTarget.All, capacityIndex, targetedEntities, targetedPositions);
    }

    public event GlobalDelegates.ByteIntArrayVector3ArrayDelegate OnAttack;
    public event GlobalDelegates.ByteIntArrayVector3ArrayDelegate OnAttackFeedback;

    public float GetMaxHp()
    {
        return MaxHP;
    }

    public float GetCurrentHp()
    {
        return CurrentHP;
    }

    public float GetCurrentHpPercent()
    {
        if (MaxHP == 0) return 0;
        return CurrentHP / MaxHP * 100;
    }

    public void RequestSetMaxHp(float value)
    {
        SetMaxHpRPC(value);
    }
    [PunRPC]
    public void SyncSetMaxHpRPC(float value)
    {
        MaxHP = value;
        OnSetMaxHp?.Invoke(MaxHP);
        OnSetMaxHpFeedback?.Invoke(MaxHP);
    }
    [PunRPC]
    public void SetMaxHpRPC(float value)
    {
        MaxHP = value;
        photonView.RPC("SyncSetMaxHpRPC", RpcTarget.All, value);
    }

    public event GlobalDelegates.FloatDelegate OnSetMaxHp;
    public event GlobalDelegates.FloatDelegate OnSetMaxHpFeedback;

    public void RequestIncreaseMaxHp(float amount)
    {
        IncreaseMaxHpRPC(amount);
    }
    [PunRPC]
    public void SyncIncreaseMaxHpRPC(float amount)
    {
        MaxHP += amount;
        OnIncreaseMaxHp?.Invoke(MaxHP);
        OnIncreaseMaxHpFeedback?.Invoke(MaxHP);
    }
    [PunRPC]
    public void IncreaseMaxHpRPC(float amount)
    {
        MaxHP += amount;
        photonView.RPC("SyncIncreaseMaxHpRPC", RpcTarget.All, amount);
    }

    public event GlobalDelegates.FloatDelegate OnIncreaseMaxHp;
    public event GlobalDelegates.FloatDelegate OnIncreaseMaxHpFeedback;

    public void RequestDecreaseMaxHp(float amount)
    {
        DecreaseMaxHpRPC(amount);
    }
    [PunRPC]
    public void SyncDecreaseMaxHpRPC(float amount)
    {
        MaxHP -= amount;
        OnDecreaseMaxHp?.Invoke(MaxHP);
        OnDecreaseMaxHpFeedback?.Invoke(MaxHP);
    }
    [PunRPC]
    public void DecreaseMaxHpRPC(float amount)
    {
        MaxHP -= amount;
        photonView.RPC("SyncDecreaseMaxHpRPC", RpcTarget.All, amount);
    }

    public event GlobalDelegates.FloatDelegate OnDecreaseMaxHp;
    public event GlobalDelegates.FloatDelegate OnDecreaseMaxHpFeedback;

    public void RequestSetCurrentHp(float value)
    {
        SetCurrentHpRPC(value);
    }
    [PunRPC]
    public void SyncSetCurrentHpRPC(float value)
    {
        CurrentHP = value;
        OnSetCurrentHp?.Invoke(CurrentHP);
        OnSetCurrentHpFeedback?.Invoke(CurrentHP);
    }
    [PunRPC]
    public void SetCurrentHpRPC(float value)
    {
        CurrentHP = value;
        photonView.RPC("SyncSetCurrentHpRPC", RpcTarget.All, value);
    }

    public event GlobalDelegates.FloatDelegate OnSetCurrentHp;
    public event GlobalDelegates.FloatDelegate OnSetCurrentHpFeedback;

    public void RequestSetCurrentHpPercent(float value)
    {
        SetCurrentHpPercentRPC(value);
    }
    [PunRPC]
    public void SyncSetCurrentHpPercentRPC(float value)
    {
        CurrentHP = (value / 100) * MaxHP;
        OnSetCurrentHpPercent?.Invoke(CurrentHP);
        OnSetCurrentHpPercentFeedback?.Invoke(CurrentHP);
    }
    [PunRPC]
    public void SetCurrentHpPercentRPC(float value)
    {
        CurrentHP = (value / 100) * MaxHP;
        photonView.RPC("SyncSetCurrentHpPercentRPC", RpcTarget.All, value);
    }

    public event GlobalDelegates.FloatDelegate OnSetCurrentHpPercent;
    public event GlobalDelegates.FloatDelegate OnSetCurrentHpPercentFeedback;

    public void RequestIncreaseCurrentHp(float amount)
    {
        IncreaseCurrentHpRPC(amount);
    }

    [PunRPC]
    public void SyncIncreaseCurrentHpRPC(float amount)
    {
        CurrentHP = amount;
        OnIncreaseCurrentHpFeedback?.Invoke(amount);
    }

    [PunRPC]
    public void IncreaseCurrentHpRPC(float amount)
    {
        CurrentHP += amount;
        if (CurrentHP > MaxHP) CurrentHP = MaxHP;
        OnIncreaseCurrentHp?.Invoke(amount);
        
        photonView.RPC("SyncIncreaseCurrentHpRPC",RpcTarget.All,CurrentHP);
    }

    public event GlobalDelegates.FloatDelegate OnIncreaseCurrentHp;
    public event GlobalDelegates.FloatDelegate OnIncreaseCurrentHpFeedback;

    public void RequestDecreaseCurrentHp(float amount)
    {
        photonView.RPC("DecreaseCurrentHpRPC",RpcTarget.MasterClient,amount);
    }

    [PunRPC]
    public void SyncDecreaseCurrentHpRPC(float amount)
    {
        CurrentHP = amount;
        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            RequestDie();
        }
        OnDecreaseCurrentHpFeedback?.Invoke(amount);
    }

    [PunRPC]
    public void DecreaseCurrentHpRPC(float amount)
    {
        CurrentHP -= amount;
        OnDecreaseCurrentHp?.Invoke(amount);
        photonView.RPC("SyncDecreaseCurrentHpRPC",RpcTarget.All,CurrentHP);
    }

    public event GlobalDelegates.FloatDelegate OnDecreaseCurrentHp;
    public event GlobalDelegates.FloatDelegate OnDecreaseCurrentHpFeedback;

    public bool IsAlive()
    {
        throw new System.NotImplementedException();
    }

    public bool CanDie()
    {
        throw new System.NotImplementedException();
    }

    public void RequestSetCanDie(bool value)
    {
        throw new System.NotImplementedException();
    }

    public void SyncSetCanDieRPC(bool value)
    {
        throw new System.NotImplementedException();
    }

    public void SetCanDieRPC(bool value)
    {
        throw new System.NotImplementedException();
    }

    public event GlobalDelegates.BoolDelegate OnSetCanDie;
    public event GlobalDelegates.BoolDelegate OnSetCanDieFeedback;

    public void RequestDie()
    {
        throw new System.NotImplementedException();
    }

    public void SyncDieRPC()
    {
        throw new System.NotImplementedException();
    }

    public void DieRPC()
    {
        throw new System.NotImplementedException();
    }

    public event GlobalDelegates.NoParameterDelegate OnDie;
    public event GlobalDelegates.NoParameterDelegate OnDieFeedback;

    public void RequestRevive()
    {
        throw new System.NotImplementedException();
    }

    public void SyncReviveRPC()
    {
        throw new System.NotImplementedException();
    }

    public void ReviveRPC()
    {
        throw new System.NotImplementedException();
    }

    public event GlobalDelegates.NoParameterDelegate OnRevive;
    public event GlobalDelegates.NoParameterDelegate OnReviveFeedback;
}