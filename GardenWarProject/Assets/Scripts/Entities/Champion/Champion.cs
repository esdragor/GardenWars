using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Entities.Capacities;
using Entities.Inventory;
using GameStates;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace Entities.Champion
{
    public partial class Champion : Entity
    {
        public bool isPlayerChampion => isOffline || gsm.GetPlayerChampion() == this;
        
        [HideInInspector] public ChampionSO currentSo;
        public Enums.ChampionRole role;
        public bool isFighter => role == Enums.ChampionRole.Fighter;
        [SerializeField] public Transform rotateParent;
        [SerializeField] private Transform nonRotateParent;
        public Vector3 forward => rotateParent.forward;
        public Quaternion rotation => rotateParent.localRotation;

        public RawImage emotesImage;

        public override Transform parent => nonRotateParent;

        public Vector3 respawnPosition;
        public Rigidbody rb;
        [SerializeField] private CapsuleCollider coll;
        public Camera cam => GameStateMachine.mainCam;

        [HideInInspector] public GameObject championMesh;

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

            maxRangeIndicatorGo = Instantiate(areaIndicatorPrefab, transform);
            maxRangeIndicatorGo.GetComponent<Renderer>().material = maxRangeMat;
            maxRangeIndicatorTr = maxRangeIndicatorGo.transform;
            maxRangeIndicatorTr.localPosition = Vector3.up * indicatorHeight;

            areaIndicatorGo = Instantiate(areaIndicatorPrefab);
            areaIndicatorGo.GetComponent<Renderer>().material = areaMat;
            areaIndicatorTr = areaIndicatorGo.transform;
            
            skillShotIndicatorTr = skillShotIndicatorGo.transform;
            
            textIndicator = Instantiate(textIndicatorPrefab,UIManager.canvas);
            textIndicatorGo = textIndicator.gameObject;
            textIndicatorTr = textIndicator.GetComponent<RectTransform>();
            
            HideMaxRangeIndicator();
            HideAreaIndicator();
            HideSkillShotIndicator();
            HideTextIndicator();
            
            currentCandyOnMap.Clear();
            
            path = new NavMeshPath();
        }

        protected override void OnUpdate()
        {
            ChangingStateMoving();
            CastHeldCapacities();
            CastHeldItems();
            UpdateAnimators();
            TryMoveToTarget();
            LookAtDestination();
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
            baseViewRange = 6f;
            viewRange = baseViewRange;
            baseMoveSpeed = currentSo.baseMoveSpeed;
            bonusMoveSpeed = 0;
            attackDamage = currentSo.attackDamage;
            baseAttackSpeed = currentSo.attackSpeed;
            attackRange = currentSo.attackRange;

            baseDef = currentSo.baseDefense;
            
            team = newTeam;
            role = newRole;

            if (!agent) agent = GetComponent<NavMeshAgent>();
            agent.speed = moveSpeed;
            
            championMesh = Instantiate(currentSo.championMeshPrefab, rotateParent.position,
                Quaternion.identity, rotateParent);
            championMesh.transform.localEulerAngles = Vector3.zero;

            throwAbilityIndex = CapacitySOCollectionManager.GetThrowAbilityIndex(role);

            var linker = championMesh.GetComponent<ChampionMeshLinker>();
            var teamMat = team == gsm.GetPlayerTeam() ? currentSo.materialsBlueTeam : currentSo.materialsRedTeam;
            linker.LinkTeamColor(teamMat);

            foreach (var rend in linker.renderers)
            {
                AddRender(rend);
            }
            
            animators = linker.animators;

            elementsToShow.Add(championMesh);

            so.SetIndexes();

            foreach (var index in so.passiveCapacitiesIndexes)
            {
                AddPassiveCapacityRPC(index);
            }
            
            attackAbilityIndex = currentSo.attackAbilityIndex;

            abilitiesIndexes = new byte[3];
            
            for (byte i = 0; i < currentSo.activeCapacitiesIndexes.Length; i++)
            {
                ChangeActiveAbility(i, currentSo.activeCapacitiesIndexes[i]);
            }

            SetupAbility(recallAbilityIndex);
            SetupAbility(consumeAbilityIndex);
            SetupAbility(throwAbilityIndex);

            if (!isOffline)
            {
                if (gsm.GetPlayerTeam() != team) championMesh.SetActive(false);
            }

            canDie = true;
            canMove = true;
            canAttack = true;
            canCast = true;
            canBeTargeted = true;
            canBeDisplaced = true;

            cooldownReduction = 0;
            
            currentCandy = 0;
            upgradeCount = 0;
            
            if(!GameSettingsManager.isDebug) return;
            
            currentCandy = GameSettingsManager.candy;
            upgradeCount = GameSettingsManager.upgrades;
        }

        public void SetupSpawn()
        {
            var pos = transform;
            var mlm = MapLoaderManager.Instance;
            if (mlm == null)
            {
                transform.position = respawnPosition = pos.position;
                
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

            transform.position = respawnPosition = pos.position;
            rb.velocity = Vector3.zero;
        }

        public void SetupUI()
        {
            if (uiManager == null) return;
            uiManager.InstantiateHealthBarForEntity(this);
            uiManager.InstantiateEmoteForEntity(this);
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

        public void RequestPressEmote(byte indexOfEmote)
        {
            if(isOffline) return;
            
            if(indexOfEmote >= 6 ) return;

            photonView.RPC("SyncDisplayEmoteRPC", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, indexOfEmote);
        }

        [PunRPC]
        private void SyncDisplayEmoteRPC(int actorNumber,byte indexOfEmote)
        {

            
            emotesImage.texture = gsm.GetPlayerEmotes(actorNumber)[indexOfEmote];
            emotesImage.color = Color.clear;
            emotesImage.gameObject.SetActive(true);

            emotesImage.DOKill();
            emotesImage.DOColor(Color.white, 0.25f).OnComplete(() => emotesImage.DOColor(Color.white, 2f).OnComplete(()=>emotesImage.DOColor(Color.clear, 1f)));
        }
        

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