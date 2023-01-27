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
        
        public float respawnDuration = 5;
        public float respawnDurationIncreasePerMinute = 2;
        
        private double respawnTimer;
        
        [SerializeField] private UIDeathTimer DeadCanvas;
        private UIDeathTimer deathTimer;
        private GameObject deadCanvasGO = null;

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
            if (!deadCanvasGO)
            {
                deathTimer = Instantiate(DeadCanvas);
                deadCanvasGO = deathTimer.gameObject;
            }
            deadCanvasGO.SetActive(true);
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
            
            var totalRespawnDuration = respawnDuration + ((UIManager.currentTime - gsm.startTime) / 60) * respawnDurationIncreasePerMinute;
            double secondsCounter = 0;
            int secondsPassed = 0;
            
            photonView.RPC("UpdateDeathTimerRPC",RpcTarget.All,(int)totalRespawnDuration);

            OnDie?.Invoke(killerId);
            GameStateMachine.Instance.OnTick += Revive;
            photonView.RPC("SyncDieRPC", RpcTarget.All, killerId);
            
            void Revive()
            {
                respawnTimer += 1 / gsm.tickRate;
                secondsCounter += 1 / gsm.tickRate;

                if (secondsCounter >= 1)
                {
                    secondsCounter = 0;
                    secondsPassed += 1;
                    int timeLeft = (int)totalRespawnDuration - secondsPassed;
                    photonView.RPC("UpdateDeathTimerRPC",RpcTarget.All,timeLeft);
                }
                

                if (respawnTimer < totalRespawnDuration) return;
            
                photonView.RPC("UpdateDeathTimerRPC",RpcTarget.All,-1);
                GameStateMachine.Instance.OnTick -= Revive;
                respawnTimer = 0f;
                RequestRevive();
            }
        }
        
        [PunRPC]
        private void UpdateDeathTimerRPC(int value)
        {
            deathTimer.UpdateTextTimer(value);
        }
        
        
        [PunRPC]
        public void SyncDieRPC(int killerId)
        {
            SetAnimatorBool("IsAlive", false);
            SetAnimatorTrigger("Death");
            if (photonView.IsMine)
            {
                InputManager.PlayerMap.Movement.Disable();
                InputManager.PlayerMap.Attack.Disable();
                InputManager.PlayerMap.Capacity.Disable();
                InputManager.PlayerMap.Inventory.Disable();
                agent.isStopped = true;
                agent.ResetPath();

                HideMaxRangeIndicator();
            }
            isAlive = false;
            uiTransform.gameObject.SetActive(false);
            FogOfWarManager.Instance.RemoveFOWViewable(this);
            
            coll.enabled = false;

            OnDieFeedback?.Invoke(killerId);
        }

        public event Action<int> OnDie;
        public event Action<int> OnDieFeedback;

        public void RequestRevive()
        {
            deadCanvasGO.SetActive(false);
            photonView.RPC("ReviveRPC", RpcTarget.MasterClient);
        }
        
        [PunRPC]
        public void ReviveRPC()
        {
            if (isAlive) return;
            
            //isAlive = true;
            canDie = true;
            canMove = true;
            canAttack = true;
            canCast = true;
            canBeDisplaced = true;
            
            SetCurrentHpRPC(maxHp);
            SetCurrentResourceRPC(maxResource);
            OnRevive?.Invoke();
            photonView.RPC("SyncReviveRPC", RpcTarget.All);

        }
        
        [PunRPC]
        public void SyncReviveRPC()
        {
            agent.enabled = true;
            agent.Warp(respawnPosition);
            if (photonView.IsMine)
            {
                InputManager.PlayerMap.Movement.Enable();
                InputManager.PlayerMap.Attack.Enable();
                InputManager.PlayerMap.Capacity.Enable();
                InputManager.PlayerMap.Inventory.Enable();
                agent.isStopped = false;
                agent.destination = transform.position;
            }
            isAlive = true;
            SetAnimatorBool("IsAlive", true);
            FogOfWarManager.Instance.AddFOWViewable(this);
            rotateParent.gameObject.SetActive(true);
            uiTransform.gameObject.SetActive(true);
            OnReviveFeedback?.Invoke();
            SetAnimatorTrigger("Respawn");
            coll.enabled = true;
        }

        public event GlobalDelegates.NoParameterDelegate OnRevive;
        public event GlobalDelegates.NoParameterDelegate OnReviveFeedback;
    }
}