using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entities;
using Photon.Pun;
using UnityEngine;

public class Tower : Entity, IAttackable, IActiveLifeable, IDeadable
{
    
    [SerializeField] private bool canAttack = true;
    [SerializeField] private float attackValue = 5f;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float MaxHP = 100f;
    [SerializeField] private float CurrentHP = 100f;

    private bool isAlive = true;
    private bool canDie = true;
    
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
    }

    public Enums.Team GetTeam()
    {
        return team;
    }

    public List<Enums.Team> GetEnemyTeams()
    {
        return Enum.GetValues(typeof(Enums.Team)).Cast<Enums.Team>().Where(someTeam => someTeam != team)
            .ToList(); //returns all teams that are not 'team'
    }
    

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
        photonView.RPC("AttackRPC", RpcTarget.MasterClient, capacityIndex, targetedEntities, targetedPositions);
    }
    [PunRPC]
    public void SyncAttackRPC(byte capacityIndex, int[] targetedEntities, Vector3[] targetedPositions)
    {
        OnAttack?.Invoke(capacityIndex, targetedEntities, targetedPositions);
        OnAttackFeedback?.Invoke(capacityIndex, targetedEntities, targetedPositions);
    }
    [PunRPC]
    public void AttackRPC(byte capacityIndex, int[] targetedEntities, Vector3[] targetedPositions)
    {
        //attackValue = CapacitySOCollectionManager.GetActiveCapacitySOByIndex(capacityIndex).AtkValue;
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
        //Debug.Log("CurrentHP : " + CurrentHP);
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
        return isAlive;
    }

    public bool CanDie()
    {
        return canDie;
    }

    public void RequestSetCanDie(bool value)
    {
        photonView.RPC("SetCanDieRPC",RpcTarget.MasterClient,value);
    }

    public void SyncSetCanDieRPC(bool value)
    {
        canDie = value;
    }

    public void SetCanDieRPC(bool value)
    {
       canDie = value;
         photonView.RPC("SyncSetCanDieRPC",RpcTarget.All,value);
    }

    public event GlobalDelegates.BoolDelegate OnSetCanDie;
    public event GlobalDelegates.BoolDelegate OnSetCanDieFeedback;

    public void RequestDie()
    {
        photonView.RPC("DieRPC", RpcTarget.MasterClient);
    }

    [PunRPC]
    public void SyncDieRPC()
    {
        isAlive = false;
        Destroy(gameObject);
    }

    [PunRPC]
    public void DieRPC()
    {
        photonView.RPC("SyncDieRPC", RpcTarget.All);
    }

    public event GlobalDelegates.NoParameterDelegate OnDie;
    public event GlobalDelegates.NoParameterDelegate OnDieFeedback;

    public void RequestRevive()
    {
        
    }

    public void SyncReviveRPC()
    {
       
    }

    public void ReviveRPC()
    {
        
    }

    public event GlobalDelegates.NoParameterDelegate OnRevive;
    public event GlobalDelegates.NoParameterDelegate OnReviveFeedback;
}
