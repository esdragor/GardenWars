using Entities.FogOfWar;
using GameStates;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

namespace Entities.Champion
{
    public partial class Champion : Entity
    {
        public ChampionSO championSo;
        public Transform rotateParent;
        private Vector3 respawnPos;

        private GameStateMachine gsm => GameStateMachine.Instance;
        private UIManager uiManager => UIManager.Instance;
        public Rigidbody rb;

        public CollisionBlocker blocker;
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

        public void ApplyChampionSO(byte championSoIndex, Enums.Team newTeam)
        {
            var so = gsm.allChampionsSo[championSoIndex];
            championSo = so;
            maxHp = championSo.maxHp;
            currentHp = maxHp;
            maxResource = championSo.maxMana;
            currentResource = championSo.maxMana;
            baseViewRange = 12.5f;
            viewRange = baseViewRange;
            referenceMoveSpeed = championSo.baseMoveSpeed;
            currentMoveSpeed = referenceMoveSpeed;
            attackDamage = championSo.attackDamage;
            attackSpeed = championSo.attackSpeed;
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
            canMove = true;
            canAttack = true;
            canCast = true;
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
    }
}