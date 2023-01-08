using System;
using System.Collections;
using System.Collections.Generic;
using Entities;
using Entities.Capacities;
using Entities.FogOfWar;
using Entities.Inventory;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using Object = System.Object;
using Random = UnityEngine.Random;

public class Pinata : Entity, IMoveable, IAttackable, IActiveLifeable, IDeadable
{
    public ActivePinataAutoSO activePinataAutoSO;
    
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private bool canAttack = true;
    [SerializeField] private float attackValue = 5f;
    [SerializeField] private Transform FalseFX;
    [SerializeField] private GameObject Mesh;
    
    [SerializeField] private float MaxHP = 100f;
    [SerializeField] private float currentHp = 100f;
    [SerializeField] private GameObject PinataDieFX;
    [SerializeField] private ParticleSystem HitFX;



    private bool canMove;
    private float referenceMoveSpeed;
    private float currentMoveSpeed;
    private bool isAlive = true;
    private bool canDie = true;

    protected override void OnStart()
    {
        canMove = true;
        Item item = AssignRandomItem();
        Mesh.GetComponent<Renderer>().material.color = item.AssociatedItemSO().itemColor;
        RequestAddItem(item.indexOfSOInCollection);
        OnInstantiated();
    }

    public override void OnInstantiated()
    {
        referenceMoveSpeed = activePinataAutoSO.Speed;
        currentMoveSpeed = referenceMoveSpeed;
        agent.speed = referenceMoveSpeed;
        UIManager.Instance.InstantiateHealthBarForEntity(this);
    }

    private Item AssignRandomItem()
    {
        ItemCollectionManager im = ItemCollectionManager.Instance;
        int randomItem = Random.Range(0, im.allItemSOs.Count);

        return im.CreateItem((byte)randomItem, this);
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

    public void RequestAttack(byte attackIndex, int targetedEntities, Vector3 targetedPositions)
    {
        photonView.RPC("AttackRPC", RpcTarget.MasterClient, attackIndex, targetedEntities, targetedPositions);
    }
    [PunRPC]
    public void SyncAttackRPC(byte capacityIndex, int targetedEntities, Vector3 targetedPositions)
    {
        if(FalseFX)
        FalseFX.gameObject.SetActive(true);
        OnAttack?.Invoke(capacityIndex, targetedEntities, targetedPositions);
        OnAttackFeedback?.Invoke(capacityIndex, targetedEntities, targetedPositions);
    }
    [PunRPC]
    public void AttackRPC(byte attackIndex, int targetedEntities, Vector3 targetedPositions)
    {
        attackValue = ((ActivePinataAutoSO)CapacitySOCollectionManager.GetActiveCapacitySOByIndex(attackIndex)).AtkValue;
        var entity = EntityCollectionManager.GetEntityByIndex(targetedEntities);
        entity.GetComponent<IActiveLifeable>().DecreaseCurrentHpRPC(attackValue, entityIndex);
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
        return currentHp;
    }

    public float GetCurrentHpPercent()
    {
        if (MaxHP == 0) return 0;
        return currentHp / MaxHP * 100;
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
        currentHp = value;
        OnSetCurrentHp?.Invoke(currentHp);
        OnSetCurrentHpFeedback?.Invoke(currentHp);
    }
    [PunRPC]
    public void SetCurrentHpRPC(float value)
    {
        currentHp = value;
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
        currentHp = (value / 100) * MaxHP;
        OnSetCurrentHpPercent?.Invoke(currentHp);
        OnSetCurrentHpPercentFeedback?.Invoke(currentHp);
    }
    [PunRPC]
    public void SetCurrentHpPercentRPC(float value)
    {
        currentHp = (value / 100) * MaxHP;
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
        currentHp = amount;
        OnIncreaseCurrentHpFeedback?.Invoke(amount,killerId);
    }

    [PunRPC]
    public void IncreaseCurrentHpRPC(float amount, int killerId)
    {
        currentHp += amount;
        if (currentHp > MaxHP) currentHp = MaxHP;
        OnIncreaseCurrentHp?.Invoke(amount,killerId);
        
        photonView.RPC("SyncIncreaseCurrentHpRPC",RpcTarget.All,currentHp, killerId);
    }

    public event Action<float,int> OnIncreaseCurrentHp;
    public event Action<float,int> OnIncreaseCurrentHpFeedback;

    public void RequestDecreaseCurrentHp(float amount, int killerId)
    {
        photonView.RPC("DecreaseCurrentHpRPC",RpcTarget.MasterClient,amount, killerId);
    }

    [PunRPC]
    public void SyncDecreaseCurrentHpRPC(float amount, int killerId)
    {
        currentHp = amount;
        //Debug.Log("CurrentHP : " + CurrentHP);
        if (currentHp <= 0)
        {
            currentHp = 0;
            Destroy(Instantiate(PinataDieFX, transform.position, Quaternion.identity), 2f);
            
            SetAnimatorTrigger("Death");
            
            GameObject item = PhotonNetwork.Instantiate(activePinataAutoSO.ItemBagPrefab.name, transform.position, Quaternion.identity);
            ItemBag bag = item.GetComponent<ItemBag>();
            bag.SetItemBag(items[0].indexOfSOInCollection, EntityCollectionManager.GetEntityByIndex(killerId).team);
            bag.ChangeVisuals(true);
            bag.IsCollectible();
            
            if(isMaster) DieRPC(killerId);
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
        currentHp -= amount;
        OnDecreaseCurrentHp?.Invoke(amount,killerId);
        if (isOffline)
        {
            SyncDecreaseCurrentHpRPC(currentHp, killerId);
            return;
        }
        photonView.RPC("SyncDecreaseCurrentHpRPC",RpcTarget.All,currentHp, killerId);
    }

    public event Action<float,int> OnDecreaseCurrentHp;
    public event Action<float,int> OnDecreaseCurrentHpFeedback;

    private void DropItem()
    {
        
    }
    
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

    public void RequestDie(int KillerID)
    {
        photonView.RPC("DieRPC", RpcTarget.MasterClient, KillerID);
    }
    
    [PunRPC]
    public void DieRPC(int KillerID)
    {
        var entity = EntityCollectionManager.GetEntityByIndex(KillerID);
        if (entity)
        {
        }

        
        if (isOffline)
        {
            SyncDieRPC(KillerID);
            
            return;
        }
        photonView.RPC("SyncDieRPC", RpcTarget.All, KillerID);
    }
    
    [PunRPC]
    public void SyncDieRPC(int KillerID)
    {
        isAlive = false;
        FogOfWarManager.Instance.RemoveFOWViewable(this);

        gameObject.SetActive(false);
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