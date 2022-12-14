using System.Linq;
using Entities.Inventory;
using GameStates;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

namespace Entities.Champion
{
    public partial class Champion : Entity
    {
        public bool isPlayerChampion => gsm.GetPlayerChampion() == this;

        [HideInInspector] public ChampionSO currentSo;
        public Enums.ChampionRole role;
        public bool isFighter => role == Enums.ChampionRole.Fighter;
        [SerializeField] public Transform rotateParent;
        public Vector3 forward => rotateParent.forward;
        public Quaternion rotation => rotateParent.localRotation;
        private Vector3 respawnPos;
        public Rigidbody rb;

        public CollisionBlocker blocker;
        private static readonly int Speed = Animator.StringToHash("Speed");

        protected override void OnStart()
        {
            agent = GetComponent<NavMeshAgent>();
            var obstacle = GetComponent<NavMeshObstacle>();
            blocker.characterColliderBlocker.enabled = false;
            obstacle.enabled = true;
            rb.isKinematic = true;
            agent.enabled = true;
        }

        protected override void OnUpdate()
        {
            CastHeldCapacities();
            CastHeldItems();
            UpdateAnimators();
            TryMoveToTarget();
        }

        private void UpdateAnimators()
        {
            if (!photonView.IsMine) return;
            foreach (var animator in animators)
            {
                animator.SetFloat(Speed, currentVelocity);
            }
        }

        protected override void OnFixedUpdate()
        {
        }

        public override void OnInstantiated()
        {
        }

        public override void OnInstantiatedFeedback()
        {
            isAlive = true;
        }

        public void ApplyChampionSO(byte championSoIndex, Enums.Team newTeam, Enums.ChampionRole newRole)
        {
            var so = gsm.allChampionsSo[championSoIndex];
            currentSo = so;
            maxHp = currentSo.maxHp;
            currentHp = maxHp;
            maxResource = currentSo.maxMana;
            currentResource = currentSo.maxMana;
            baseViewRange = 12.5f;
            viewRange = baseViewRange;
            referenceMoveSpeed = currentSo.baseMoveSpeed;
            currentMoveSpeed = referenceMoveSpeed;
            attackDamage = currentSo.attackDamage;
            baseAttackSpeed = currentSo.attackSpeed;
            attackRange = currentSo.attackRange;

            attackAbilityIndex = currentSo.attackAbilityIndex;
            abilitiesIndexes = currentSo.activeCapacitiesIndexes;
            var championMesh = Instantiate(currentSo.championMeshPrefab, rotateParent.position,
                Quaternion.identity, rotateParent);
            championMesh.transform.localEulerAngles = Vector3.zero;

            team = newTeam;
            role = newRole;

            var linker = championMesh.GetComponent<ChampionMeshLinker>();
            switch (team)
            {
                case Enums.Team.Team1:
                    linker.LinkTeamColor(currentSo.materialsBlueTeam);
                    break;
                case Enums.Team.Team2:
                    linker.LinkTeamColor(currentSo.materialsRedTeam);
                    break;
            }
            animators = linker.animators;

            elementsToShow.Add(championMesh);

            if (!isOffline)
            {
                so.SetIndexes();

                foreach (var index in so.passiveCapacitiesIndexes)
                {
                    AddPassiveCapacityRPC(index);
                }

                if (gsm.GetPlayerTeam() != team) championMesh.SetActive(false);
            }

            canDie = true;
            canMove = true;
            canAttack = true;
            canCast = true;
            canBeTargeted = true;
        }

        public void SetupSpawn()
        {
            var pos = transform;
            var mlm = MapLoaderManager.Instance;
            if (mlm == null)
            {
                respawnPos = transform.position = pos.position;
                rb.velocity = Vector3.zero;
                return;
            }

            switch (team)
            {
                case Enums.Team.Team1:
                {
                    for (int i = 0; i < mlm.firstTeamBasePoint.Length; i++)
                    {
                        if (mlm.firstTeamBasePoint[i].champion == null)
                        {
                            pos = mlm.firstTeamBasePoint[i].position;
                            mlm.firstTeamBasePoint[i].champion = this;
                            break;
                        }
                    }

                    break;
                }
                case Enums.Team.Team2:
                {
                    for (int i = 0; i < mlm.secondTeamBasePoint.Length; i++)
                    {
                        if (mlm.secondTeamBasePoint[i].champion == null)
                        {
                            pos = mlm.secondTeamBasePoint[i].position;
                            mlm.secondTeamBasePoint[i].champion = this;
                            break;
                        }
                    }

                    break;
                }
                default:
                    Debug.LogError("Team is not valid.");
                    pos = transform;
                    break;
            }

            respawnPos = transform.position = pos.position;
            rb.velocity = Vector3.zero;
        }

        public void SetupUI()
        {
            if (uiManager == null) return;
            uiManager.InstantiateHealthBarForEntity(this);
            uiManager.InstantiateResourceBarForEntity(this);
        }

        public void PlayThrowAnimation()
        {
            SetAnimatorTrigger("Throw");
        }

        public void LookAt(Vector3 pos)
        {
            pos.y = rotateParent.position.y;
            rotateParent.LookAt(pos);
        }

        public int selectedItemIndex = 0;

        public void RequestPressItem(byte itemIndexInInventory, int selectedEntities, Vector3 positions)
        {
            selectedItemIndex = itemIndexInInventory;
            if (!isFighter) return;
            if (itemIndexInInventory >= items.Count) return;
            if (isMaster)
            {
                PressItemRPC(itemIndexInInventory, selectedEntities, positions);
                return;
            }

            photonView.RPC("PressItemRPC", RpcTarget.MasterClient, itemIndexInInventory, selectedEntities,
                positions);
        }

        [PunRPC]
        public void PressItemRPC(byte itemIndexInInventory, int selectedEntities, Vector3 positions)
        {
            selectedItemIndex = itemIndexInInventory;
            if (!isFighter) return;
            if (itemIndexInInventory >= items.Count) return;
            var item = items[itemIndexInInventory];
            if (item == null) return;
            if (isOffline)
            {
                SyncPressItemRPC(itemIndexInInventory, selectedEntities, positions);
                return;
            }

            photonView.RPC("SyncPressItemRPC", RpcTarget.All, itemIndexInInventory, selectedEntities, positions);
        }

        [PunRPC]
        public void SyncPressItemRPC(byte itemIndexInInventory, int selectedEntities, Vector3 positions)
        {
            selectedItemIndex = itemIndexInInventory;
            if (!isFighter) return;
            targetedEntities = selectedEntities;
            targetedPositions = positions;
            if (itemIndexInInventory >= items.Count) return;
            var item = items[itemIndexInInventory];
            if (items[itemIndexInInventory] == null) return;
            foreach (var activeCapacity in item.activeCapacities)
            {
                activeCapacity.OnPress(targetedEntities, targetedPositions);
            }

            heldItems.Add(item);
        }

        private void CastHeldItems()
        {
            if (!isFighter) return;
            foreach (var capacity in heldItems.SelectMany(item => item.activeCapacities))
            {
                capacity.OnHold(targetedEntities, targetedPositions);
            }
        }

        public void RequestReleaseItem(byte itemIndexInInventory, int selectedEntities, Vector3 positions)
        {
            if (isMaster)
            {
                ReleaseItemRPC(itemIndexInInventory, selectedEntities, positions);
                return;
            }

            photonView.RPC("ReleaseItemRPC", RpcTarget.MasterClient, itemIndexInInventory, selectedEntities, positions);
        }

        [PunRPC]
        public void ReleaseItemRPC(byte itemIndexInInventory, int selectedEntities, Vector3 positions)
        {
            if (isOffline)
            {
                SyncReleaseItemRPC(itemIndexInInventory, selectedEntities, positions);
                return;
            }

            photonView.RPC("SyncReleaseItemRPC", RpcTarget.All, itemIndexInInventory, selectedEntities, positions);
        }

        [PunRPC]
        public void SyncReleaseItemRPC(byte itemIndexInInventory, int selectedEntities, Vector3 positions)
        {
            if (itemIndexInInventory >= items.Count) return;
            var item = items[itemIndexInInventory];
            if (item == null) return;

            if (item.activeCapacities.Any(activeCapacity => !activeCapacity.CanCast(selectedEntities, positions)))
                return;

            if (isFighter && !item.isOnCooldown)
            {
                item.OnItemActivated(selectedEntities, positions);
                foreach (var activeCapacity in item.activeCapacities)
                {
                    activeCapacity.OnRelease(selectedEntities, positions);
                }
            }

            if (isMaster) OnActivateItem?.Invoke(itemIndexInInventory, selectedEntities, positions);
            OnActivateItemFeedback?.Invoke(itemIndexInInventory, selectedEntities, positions);

            heldItems.Remove(item);
        }

        public event GlobalDelegates.ByteIntArrayVector3ArrayDelegate OnActivateItem;
        public event GlobalDelegates.ByteIntArrayVector3ArrayDelegate OnActivateItemFeedback;

        public Item PopSelectedItem()
        {
            var item = GetItem(selectedItemIndex);
            if (item != null) RemoveItemRPC(item);
            return item;
        }
    }
}