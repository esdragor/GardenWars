using System.Collections;
using System.Collections.Generic;
using Entities;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class Pinata : Entity, IMoveable
{
    public ActiveMinionAutoSO activePinataAutoSO;
    
    [SerializeField] private NavMeshAgent agent;
    
    private bool canMove;
    private float referenceMoveSpeed;
    private float currentMoveSpeed;

    protected override void OnStart()
    {
        canMove = true;
    }

    public override void OnInstantiated()
    {
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
}