using System;
using System.Collections.Generic;
using System.Linq;
using Entities.Capacities;
using Entities.FogOfWar;
using GameStates;
using Photon.Pun;
using UnityEngine;

namespace Entities
{
    [RequireComponent(typeof(PhotonView)), RequireComponent(typeof(PhotonTransformView))]
    public abstract partial class Entity : MonoBehaviourPun, ITeamable
    {
        public static bool isOffline => !PhotonNetwork.IsConnected;
        public static bool isMaster => isOffline || PhotonNetwork.IsMasterClient;
        
        protected GameStateMachine gsm => GameStateMachine.Instance;
        protected UIManager uiManager => UIManager.Instance;
        public int entityIndex => photonView.ViewID;
        public Vector3 position => isVisible ? transform.position : lastSeenPosition;
        private Vector3 lastSeenPosition;
        
        [Header("Team")]
        public bool canChangeTeam;
        public Enums.Team team;
        public bool isEnemyOfPlayer => gsm.GetPlayerChampion().GetEnemyTeams().Contains(team);

        /// <summary>
        /// True if passiveCapacities can be added to the entity's passiveCapacitiesList. False if not.
        /// </summary>
        [Header("Passive Capacities")]
        [SerializeField] private bool canAddPassiveCapacity = true;

        /// <summary>
        /// True if passiveCapacities can be removed from the entity's passiveCapacitiesList. False if not.
        /// </summary>
        [SerializeField] private bool canRemovePassiveCapacity = true;

        /// <summary>
        /// The list of PassiveCapacity on the entity.
        /// </summary>
        public readonly List<PassiveCapacity> passiveCapacitiesList = new List<PassiveCapacity>();

        /// <summary>
        /// The transform of the UI of the entity.
        /// </summary>
        [Header("UI")]
        public Transform uiTransform;

        /// <summary>
        /// The offset of the UI of the entity.
        /// </summary>
        public Vector3 uiOffset = new Vector3(0, 2f, 0);

        [SerializeField] private List<Renderer> renderers = new List<Renderer>();

        protected Animator[] animators = Array.Empty<Animator>();

        private void Start()
        {
            EntityCollectionManager.AddEntity(this);
            OnStart();
        }
        
        protected abstract void OnStart();

        private void Update()
        {
            OnUpdate();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate();
        }
        
        protected virtual void OnUpdate() { }

        protected virtual void OnFixedUpdate() { }
        
        public void SyncInstantiate(Enums.Team newTeam)
        {
            InitEntity(newTeam);
            OnInstantiated();
            if (isOffline)
            {
                OnInstantiatedFeedback();
                return;
            }
            photonView.RPC("SyncInstantiateRPC", RpcTarget.All, transform.position, transform.rotation,(byte)team);
            
        }
        
        [PunRPC]
        public void SyncInstantiateRPC(Vector3 position, Quaternion rotation,byte newTeam)
        {
            transform.position = position;
            transform.rotation = rotation;
            if (!isMaster)
            {
                InitEntity((Enums.Team)newTeam);
                OnInstantiated();
            }
            OnInstantiatedFeedback();
        }

        public void InitEntity(Enums.Team newTeam)
        {
            EntityCollectionManager.AddEntity(this);
            FogOfWarManager.Instance.AddFOWViewable(this);
            FogOfWarManager.Instance.AddFOWShowable(this);
            
            isVisible = isEnemyOfPlayer;
            team = newTeam;

            canShow = true;
            canHide = true;
            canView = true;

            UpdateShow();
            ChangeColor();
        }

        public abstract void OnInstantiated();

        

        public PassiveCapacity GetPassiveCapacityBySOIndex(byte soIndex)
        {
            return passiveCapacitiesList.FirstOrDefault(item => item.indexOfSo == soIndex);
        }

        public virtual void OnInstantiatedFeedback() { }

        /// <summary>
        /// Sends an RPC to the master to set the value canAddPassiveCapacity.
        /// </summary>
        /// <param name="value">The value to set canAddPassiveCapacity to</param>
        [PunRPC]
        private void SyncSetCanAddPassiveCapacityRPC(bool value)
        {
            photonView.RPC("SetCanAddPassiveCapacityRPC", RpcTarget.All, value);
        }

        /// <summary>
        /// Sets if passiveCapacities can be added to the entity's passiveCapacitiesList.
        /// </summary>
        /// <param name="value">true if they can, false if not</param>
        [PunRPC]
        public void SetCanAddPassiveCapacityRPC(bool value)
        {
            canAddPassiveCapacity = value;
        }

        /// <summary>
        /// Sends an RPC to the master to set the value canRemovePassiveCapacity.
        /// </summary>
        /// <param name="value">The value to set canRemovePassiveCapacity to</param>
        [PunRPC]
        private void SyncSetCanRemovePassiveCapacityRPC(bool value)
        {
            photonView.RPC("SetCanRemovePassiveCapacityRPC", RpcTarget.All, value);
        }

        /// <summary>
        /// Sets if passiveCapacities can be removed the entity's passiveCapacitiesList.
        /// </summary>
        /// <param name="value">true if they can, false if not</param>
        [PunRPC]
        private void SetCanRemovePassiveCapacityRPC(bool value)
        {
            canRemovePassiveCapacity = value;
        }
        
        public void RequestAddPassiveCapacityByIndex(byte index)
        {
            if (isMaster)
            {
                AddPassiveCapacityRPC(index);
                return;
            }
            photonView.RPC("AddPassiveCapacityRPC",RpcTarget.MasterClient,index);
        }

        /// <summary>
        /// Adds a PassiveCapacity to the passiveCapacityList.
        /// </summary>
        /// <param name="index">The index of the PassiveCapacitySO of the PassiveCapacity to add</param>
        [PunRPC]
        public void AddPassiveCapacityRPC(byte index)
        {
            if (!canAddPassiveCapacity) return;
            if (isOffline)
            {
                SyncAddPassiveCapacityRPC(index);
                return;
            }
            photonView.RPC("SyncAddPassiveCapacityRPC",RpcTarget.All, index);
        }
        /// <summary>
        /// Sends an RPC to all clients to add a PassiveCapacity to the passiveCapacityList.
        /// </summary>
        /// <param name="capacity">The index of the PassiveCapacity to add</param>
        [PunRPC]
        public void SyncAddPassiveCapacityRPC(byte capacityIndex)
        {
            var capacity = CapacitySOCollectionManager.Instance.CreatePassiveCapacity(capacityIndex, this);
            if(capacity == null) return;
            if(!passiveCapacitiesList.Contains(capacity)) passiveCapacitiesList.Add(capacity);
            if (isMaster)
            {
                capacity.OnAdded(this);
                OnPassiveCapacityAdded?.Invoke(capacity);
            }

            capacity.OnAddedFeedback(this);
            OnPassiveCapacityAddedFeedback?.Invoke(capacity);
        }
        public event Action<PassiveCapacity> OnPassiveCapacityAdded;
        public event Action<PassiveCapacity> OnPassiveCapacityAddedFeedback;

        public void RequestRemovePassiveCapacityByIndex(byte index)
        {
            if (isMaster)
            {
                RemovePassiveCapacityByIndexRPC(index);
                return;
            }
            photonView.RPC("RemovePassiveCapacityByIndexRPC",RpcTarget.MasterClient,index);
        }
        
        /// <summary>
        /// Removes a PassiveCapacity from the passiveCapacityList.
        /// </summary>
        /// <param name="index">The index in the passiveCapacityList of the PassiveCapacity to remove</param>
        [PunRPC]
        public void RemovePassiveCapacityByIndexRPC(byte index)
        {
            if(isOffline)
            {
                SyncRemovePassiveCapacityRPC(index);
                return;
            }
            photonView.RPC("SyncRemovePassiveCapacityRPC",RpcTarget.All,index);
        }
        
        /// <summary>
        /// Sends an RPC to all clients to remove a PassiveCapacity from passiveCapacityList.
        /// </summary>
        /// <param name="index">The PassiveCapacity to remove</param>
        [PunRPC]
        public void SyncRemovePassiveCapacityRPC(byte index)
        {
            if(passiveCapacitiesList.Count == 0) return;
            var capacity = null as PassiveCapacity;
            for (int i = 0; i < passiveCapacitiesList.Count; i++)
            {
                if (passiveCapacitiesList[i].indexOfSo == index)
                {
                    capacity = passiveCapacitiesList[i];
                    break;
                }
            }
            if (capacity == null) return;

            if (capacity.stackable)
            {
                if(capacity.count == 1) passiveCapacitiesList.Remove(capacity);
            }
            else
            {
                passiveCapacitiesList.Remove(capacity);
            }
            
            if (isMaster)
            {
                capacity.OnRemoved(this);
                OnPassiveCapacityRemoved?.Invoke(capacity);
            }
            
            capacity.OnRemovedFeedback(this);
            OnPassiveCapacityRemovedFeedback?.Invoke(capacity);
        }
        
        public event Action<PassiveCapacity> OnPassiveCapacityRemoved;
        public event Action<PassiveCapacity> OnPassiveCapacityRemovedFeedback;
        

        public void ChangeColor()
        {
            foreach (var material in renderers.Select(xd => xd.material))
            {
                material.color = team switch
                {
                    Enums.Team.Neutral => Color.white,
                    Enums.Team.Team1 => Color.blue,
                    Enums.Team.Team2 => Color.red,
                    _ => material.color
                };
            }
        }

        public void SetAnimatorTrigger(string trigger)
        {
            if (!photonView.IsMine) return; 
            foreach(Animator animator in animators)
            {
                animator.SetTrigger(trigger);
            }
        }

        public void SetAnimatorTrigger(int id)
        {
            if (!photonView.IsMine) return;
            foreach (Animator animator in animators)
            {
                animator.SetTrigger(id);
            }
        }
    }
}