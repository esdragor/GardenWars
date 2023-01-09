using System;
using System.Collections.Generic;
using BehaviourTree;
using Entities;
using Entities.Capacities;
using Photon.Pun;
using UnityEngine;

public class Tower : Entity, IAttackable, IActiveLifeable, IDeadable
{
    public ActiveCapacitySO activeTowerAutoSO;
    private ActiveCapacity capacity;

    [SerializeField] private bool canAttack = true;
    [SerializeField] private float attackValue = 50f;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float MaxHP = 100f;
    [SerializeField] private float CurrentHP = 100f;
    [SerializeField] private ParticleSystem HitFX;

    [SerializeField] private GameObject TowerModel;
    [SerializeField] private GameObject HairDryer;
    [SerializeField] private TowerBT BT;
    [SerializeField] private Animator[] animatorss;
    [SerializeField] private Material[] towersMaterials;

    private bool isAlive = true;
    private bool canDie = true;

    public float GetAttackSpeed()
    {
        return attackSpeed;
    }

    protected override void OnStart()
    {
        CurrentHP = MaxHP;
        animators = animatorss;
        if (team == Enums.Team.Team1)
        {
            TowerModel.GetComponent<Renderer>().material = towersMaterials[0];
        }
        else
        {
            TowerModel.transform.Rotate(Vector3.back, 90);
            TowerModel.GetComponent<Renderer>().material = towersMaterials[1];
        }
    }

    public override void OnInstantiated()
    {
        gameObject.SetActive(true);
    }

    public bool CanAttack()
    {
        return canAttack;
    }

    public void RequestSetCanAttack(bool value)
    {
        if (isMaster)
        {
            SetCanAttackRPC(value);
            return;
        }
        photonView.RPC("SetCanAttackRPC", RpcTarget.MasterClient, value);
    }
        
    [PunRPC]
    public void SetCanAttackRPC(bool value)
    {
        canAttack = value;
        OnSetCanAttack?.Invoke(value);
        if (isOffline)
        {
            SyncSetCanAttackRPC(value);
            return;
        }
        photonView.RPC("SyncSetCanAttackRPC", RpcTarget.All, value);
    }

    [PunRPC]
    public void SyncSetCanAttackRPC(bool value)
    {
        canAttack = value;
        OnSetCanAttackFeedback?.Invoke(value);
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

    public void RequestAttack(byte attackIndex, int targetedEntities, Vector3 targetedPositions)
    {
        photonView.RPC("AttackRPC", RpcTarget.MasterClient, attackIndex, targetedEntities, targetedPositions);
    }

    [PunRPC]
    public void SyncAttackRPC(byte capacityIndex, int targetedEntities, Vector3 targetedPositions)
    {
        capacity ??= CapacitySOCollectionManager.CreateActiveCapacity(capacityIndex, this);

        capacity.OnRelease(targetedEntities, targetedPositions);

        if (isMaster) OnAttack?.Invoke(capacityIndex, targetedEntities, targetedPositions);
        OnAttackFeedback?.Invoke(capacityIndex, targetedEntities, targetedPositions);
    }

    [PunRPC]
    public void AttackRPC(byte attackIndex, int targetedEntities, Vector3 targetedPositions)
    {
        photonView.RPC("SyncAttackRPC", RpcTarget.All, attackIndex, targetedEntities, targetedPositions);
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

    public void RequestIncreaseMaxHp(float amount, int killerId)
    {
        IncreaseMaxHpRPC(amount, killerId);
    }

    [PunRPC]
    public void SyncIncreaseMaxHpRPC(float amount, int killerId)
    {
        MaxHP += amount;
        OnIncreaseMaxHp?.Invoke(MaxHP);
        OnIncreaseMaxHpFeedback?.Invoke(MaxHP);
    }

    [PunRPC]
    public void IncreaseMaxHpRPC(float amount, int killerId)
    {
        MaxHP += amount;
        photonView.RPC("SyncIncreaseMaxHpRPC", RpcTarget.All, amount, killerId);
    }

    public event GlobalDelegates.FloatDelegate OnIncreaseMaxHp;
    public event GlobalDelegates.FloatDelegate OnIncreaseMaxHpFeedback;

    public void RequestDecreaseMaxHp(float amount, int killerId)
    {
        DecreaseMaxHpRPC(amount, killerId);
    }

    [PunRPC]
    public void SyncDecreaseMaxHpRPC(float amount, int killerId)
    {
        MaxHP -= amount;
        OnDecreaseMaxHp?.Invoke(MaxHP);
        OnDecreaseMaxHpFeedback?.Invoke(MaxHP);
    }

    [PunRPC]
    public void DecreaseMaxHpRPC(float amount, int killerId)
    {
        MaxHP -= amount;
        photonView.RPC("SyncDecreaseMaxHpRPC", RpcTarget.All, amount, killerId);
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

    public void RequestIncreaseCurrentHp(float amount, int killerId)
    {
        IncreaseCurrentHpRPC(amount, killerId);
    }

    [PunRPC]
    public void SyncIncreaseCurrentHpRPC(float amount, int killerId)
    {
        CurrentHP = amount;
        OnIncreaseCurrentHpFeedback?.Invoke(amount,killerId);
    }

    [PunRPC]
    public void IncreaseCurrentHpRPC(float amount, int killerId)
    {
        CurrentHP += amount;
        if (CurrentHP > MaxHP) CurrentHP = MaxHP;
        OnIncreaseCurrentHp?.Invoke(amount,killerId);

        photonView.RPC("SyncIncreaseCurrentHpRPC", RpcTarget.All, CurrentHP, killerId);
    }

    public event Action<float,int> OnIncreaseCurrentHp;
    public event Action<float,int> OnIncreaseCurrentHpFeedback;

    public void RequestDecreaseCurrentHp(float amount, int killerId)
    {
        photonView.RPC("DecreaseCurrentHpRPC", RpcTarget.MasterClient, amount, killerId);
    }

    [PunRPC]
    public void SyncDecreaseCurrentHpRPC(float amount, int killerId)
    {
        CurrentHP = amount;
        //Debug.Log("CurrentHP : " + CurrentHP);
        if (CurrentHP <= 0)
        {
            //Instantiate(MinionDieFX, transform.position, Quaternion.identity);
            CurrentHP = 0;
            SetAnimatorTrigger("Death");
            TowerModel.SetActive(false);

            RequestDie(killerId);
        }
        else
        {
            HitFX.Play();
        }

        OnDecreaseCurrentHpFeedback?.Invoke(amount,killerId);
    }

    [PunRPC]
    public void DecreaseCurrentHpRPC(float amount, int killerId)
    {
        CurrentHP -= amount;
        OnDecreaseCurrentHp?.Invoke(amount,killerId);
        photonView.RPC("SyncDecreaseCurrentHpRPC", RpcTarget.All, CurrentHP, killerId);
    }

    public event Action<float,int> OnDecreaseCurrentHp;
    public event Action<float,int> OnDecreaseCurrentHpFeedback;

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
        photonView.RPC("SetCanDieRPC", RpcTarget.MasterClient, value);
    }

    public void SyncSetCanDieRPC(bool value)
    {
        canDie = value;
    }

    public void SetCanDieRPC(bool value)
    {
        canDie = value;
        photonView.RPC("SyncSetCanDieRPC", RpcTarget.All, value);
    }

    public event GlobalDelegates.BoolDelegate OnSetCanDie;
    public event GlobalDelegates.BoolDelegate OnSetCanDieFeedback;

    public void RequestDie(int killerId)
    {
        photonView.RPC("DieRPC", RpcTarget.MasterClient, killerId);
    }

    [PunRPC]
    public void SyncDieRPC(int killerId)
    {
        isAlive = false;
        canView = false;
        enabled = false;
        BT.enabled = false;
        BT.Poussin.parent = null;
        gameObject.SetActive(false);
        HairDryer.SetActive(false);
    }

    [PunRPC]
    public void DieRPC(int killerId)

    {
        photonView.RPC("SyncDieRPC", RpcTarget.All, killerId);
    }

    public event Action<int> OnDie;
    public event Action<int> OnDieFeedback;

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