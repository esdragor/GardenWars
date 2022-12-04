using Entities.Capacities;
using GameStates;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

namespace Entities.Champion
{
    public partial class Champion : Entity
    {
        private static bool isOffline => !PhotonNetwork.IsConnected;
        private static bool isMaster => isOffline || PhotonNetwork.IsMasterClient;
        public ChampionSO championSo;
        public Transform rotateParent;
        private Vector3 respawnPos;

        private GameStateMachine gsm;
        private CapacitySOCollectionManager capacityCollection;
        private UIManager uiManager;
        public Rigidbody rb;

        public CollisionBlocker blocker;
        protected override void OnStart()
        {
            gsm = GameStateMachine.Instance;
            capacityCollection = CapacitySOCollectionManager.Instance;
            uiManager = UIManager.Instance;
            uiManager = UIManager.Instance;

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
            if (isFollowing) FollowEntity(); // Lol
            if (!photonView.IsMine) return;
            CheckMoveDistance();
        }

        protected override void OnFixedUpdate()
        {
            
        }

        public override void OnInstantiated()
        {
            
        }

        public override void OnInstantiatedFeedback()
        {
        }

        public void ApplyChampionSO(byte championSoIndex, Enums.Team newTeam, ChampionSO forcedSo = null)
        {
            var so = (forcedSo == null) ? gsm.allChampionsSo[championSoIndex] : forcedSo;
            championSo = so;
            maxHp = championSo.maxHp;
            currentHp = maxHp;
            maxResource = championSo.maxRessource;
            currentResource = championSo.maxRessource;
            viewRange = championSo.viewRange;
            referenceMoveSpeed = championSo.referenceMoveSpeed;
            currentMoveSpeed = referenceMoveSpeed;
            attackDamage = championSo.attackDamage;
            attackAbilityIndex = championSo.attackAbilityIndex;
            abilitiesIndexes = championSo.activeCapacitiesIndexes;
            var championMesh = Instantiate(championSo.championMeshPrefab, rotateParent.position,
                Quaternion.identity, rotateParent);
            championMesh.transform.localEulerAngles = Vector3.zero;

            team = newTeam;
            
            championMesh.GetComponent<ChampionMeshLinker>().LinkTeamColor(team);
            
            elementsToShow.Add(championMesh);
            
            if (!GameStateMachine.isOffline)
            {
                so.SetIndexes();
                
                foreach (var index in so.passiveCapacitiesIndexes)
                {
                    AddPassiveCapacityRPC(index);
                }
                
                if (gsm.GetPlayerTeam() != team) championMesh.SetActive(false);
            }
            
            canDie = true;
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
            uiManager.InstantiateHealthBarForEntity(entityIndex);
            uiManager.InstantiateResourceBarForEntity(entityIndex);
        }
    }
}