using System;
using Entities.FogOfWar;
using GameStates;
using Photon.Pun;
using UnityEngine;

namespace Entities.Champion
{
    public partial class Champion : IDeadable
    {
        [Header("Deadable")]
        public bool isAlive;
        public bool canDie;
        
        public float respawnDuration = 3;
        private double respawnTimer;

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
            if (isMaster)
            {
                SetCanDieRPC(value);
                return;
            }
            photonView.RPC("SetCanDieRPC", RpcTarget.MasterClient, value);
        }
        
        [PunRPC]
        public void SetCanDieRPC(bool value)
        {
            canDie = value;
            OnSetCanDie?.Invoke(value);
            if (isOffline)
            {
                SyncSetCanDieRPC(value);
                return;
            }
            photonView.RPC("SyncSetCanDieRPC", RpcTarget.All, value);
        }

        [PunRPC]
        public void SyncSetCanDieRPC(bool value)
        {
            canDie = value;
            OnSetCanDieFeedback?.Invoke(value);
        }

        

        public event GlobalDelegates.BoolDelegate OnSetCanDie;
        public event GlobalDelegates.BoolDelegate OnSetCanDieFeedback;

        public void RequestDie(int killerId)
        {
            photonView.RPC("DieRPC", RpcTarget.MasterClient, killerId);
            Debug.Log("Request to die");
        }

        [PunRPC]
        public void DieRPC(int killerId)
        {
            if (!canDie)
            {
                Debug.LogWarning($"{name} can't die!");
                return;
            }

            isAlive = false;
            canDie = false;

            canMove = false;
            canAttack = false;
            canCast = false;
            
            // TODO - Disable collision, etc...

            OnDie?.Invoke(killerId);
            GameStateMachine.Instance.OnTick += Revive;
            photonView.RPC("SyncDieRPC", RpcTarget.All, killerId);
        }
        
        
        [PunRPC]
        public void SyncDieRPC(int killerId)
        {
            if (photonView.IsMine)
            {
                InputManager.PlayerMap.Movement.Disable();
                InputManager.PlayerMap.Attack.Disable();
                InputManager.PlayerMap.Capacity.Disable();
                InputManager.PlayerMap.Inventory.Disable();
                agent.isStopped = true;
                agent.ResetPath();
                agent.ResetPath();

            }

            rotateParent.gameObject.SetActive(false); 
            uiTransform.gameObject.SetActive(false);
            FogOfWarManager.Instance.RemoveFOWViewable(this);

            OnDieFeedback?.Invoke(killerId);
        }

        public event Action<int> OnDie;
        public event Action<int> OnDieFeedback;

        public void RequestRevive()
        {
            photonView.RPC("ReviveRPC", RpcTarget.MasterClient);
        }
        
        [PunRPC]
        public void ReviveRPC()
        {
            if (isAlive) return;
            
            isAlive = true;
            canDie = true;
            canMove = true;
            canAttack = true;
            canCast = true;
            
            SetCurrentHpRPC(maxHp);
            SetCurrentResourceRPC(maxResource);
            OnRevive?.Invoke();
            photonView.RPC("SyncReviveRPC", RpcTarget.All);

        }
        
        [PunRPC]
        public void SyncReviveRPC()
        {
            transform.position = respawnPos;
            if (photonView.IsMine)
            {
                InputManager.PlayerMap.Movement.Enable();
                InputManager.PlayerMap.Attack.Enable();
                InputManager.PlayerMap.Capacity.Enable();
                InputManager.PlayerMap.Inventory.Enable();
                agent.isStopped = false;
                agent.destination = transform.position;
            }
            FogOfWarManager.Instance.AddFOWViewable(this);
            rotateParent.gameObject.SetActive(true);
            uiTransform.gameObject.SetActive(true);
            OnReviveFeedback?.Invoke();
        }

        private void Revive()
        {
            respawnTimer += 1 / GameStateMachine.Instance.tickRate;

            if (!(respawnTimer >= respawnDuration)) return;
            GameStateMachine.Instance.OnTick -= Revive;
            respawnTimer = 0f;
            RequestRevive();
        }

        public event GlobalDelegates.NoParameterDelegate OnRevive;
        public event GlobalDelegates.NoParameterDelegate OnReviveFeedback;
    }
}