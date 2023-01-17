using System.Collections.Generic;
using System.Linq;
using BehaviourTree;
using Entities;
using GameStates;
using Photon.Pun;
using UnityEngine;

public class RespawnPinata
{
    public float delay;
    public Transform pos;
}

public class SpawnAIs : MonoBehaviourPun
{
    [HideInInspector] public List<RespawnPinata> PinatasRespawned = new List<RespawnPinata>();

    [Header("Wave Management")] public int minionsPerWave = 1;
    public double timeBetweenMinionSpawn = 0.3;
    public double timeBetweenWaves = 30;
    [SerializeField] private double timeBeforeFirstWave = 5;
    [SerializeField] private List<Transform> SpawnPoints;
    [Header("Minions")] [SerializeField] private Transform[] waypointsTeamBlue;
    [SerializeField] private Transform[] waypointsTeamRed;
    [SerializeField] private Entity minion;
    [SerializeField] private double timer;
    [SerializeField] private Animator[] animatorsTraps;
    [SerializeField] private Transform[] BasketGoal;

    [Header("Towers")] [SerializeField] private Transform[] towerSpawnPoints;

    [Header("Pinata")] [SerializeField] private Transform[] pinataSpawnPoints;
    [SerializeField] float timeBeforeSpawnPinata = 4;
    [SerializeField] float timeBeforeRespawnPinata = 5;

    private GameStateMachine gsm => GameStateMachine.Instance;

    private readonly List<Entity> towers = new List<Entity>();
    private readonly List<Entity> Pinatas = new List<Entity>();
    private double timerPinata;


    public static SpawnAIs Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }
        
        Instance = this;
    }

    private void Start()
    {
        timer = 0;
        timerPinata = 0;
        gsm.OnTick += OnTick;
    }

    public void Init()
    {
        towers.Clear();
        if (PhotonNetwork.IsMasterClient)
        {
            InitTowers();
        }
    }

    public void Sync()
    {
        SyncTowerData();
    }

    public void StartSpawns()
    {
        foreach (var tower in towers)
        {
            tower.GetComponent<TowerBT>().OnStart();
        }

        timer = timeBetweenWaves - timeBeforeFirstWave;
        gsm.OnTick += SpawnWaves;
        gsm.OnTick += InitPinata;
        gsm.OnTick += RespawnPinata;
    }

    private void OnTick()
    {
        timer += gsm.increasePerTick;
        timerPinata += gsm.increasePerTick;
    }

    #region Towers

    private void InitTowers()
    {
        var redTower = NetworkPoolManager.PoolInstantiate("NewTower", towerSpawnPoints[0].position, Quaternion.identity)
            .GetComponent<Entity>();
        var blueTower = NetworkPoolManager
            .PoolInstantiate("NewTower", towerSpawnPoints[1].position, Quaternion.identity)
            .GetComponent<Entity>();

        blueTower.SyncInstantiate(Enums.Team.Team1);
        redTower.SyncInstantiate(Enums.Team.Team2);

        towers.Add(blueTower);
        towers.Add(redTower);
    }

    public void SpawnPinata(Transform pos)
    {
        var Pinata = NetworkPoolManager.PoolInstantiate("Pinata", pos.position, Quaternion.identity)
            .GetComponent<Entity>();


        Pinatas.Add(Pinata);
        Pinata.SyncInstantiate(Enums.Team.Neutral);
    }

    private void InitPinata()
    {
        if (timerPinata < timeBeforeSpawnPinata) return;
        gsm.OnTick -= InitPinata;

        foreach (Transform pos in pinataSpawnPoints)
        {
            SpawnPinata(pos);
        }
    }

    public void RespawnPinata()
    {
        return;
        for (int i = 0; i < PinatasRespawned.Count; i++)
        {
            PinatasRespawned[i].delay += (float) gsm.increasePerTick;
            if (PinatasRespawned[i].delay >= timeBeforeRespawnPinata)
            {
                SpawnPinata(PinatasRespawned[i].pos);
                PinatasRespawned.RemoveAt(i);
            }
        }
    }

    private void SyncTowerData()
    {
        var towerIds = towers.Select(tower => tower.entityIndex).ToArray();
        var towerTeams = towers.Select(tower => (byte) tower.team).ToArray();
        photonView.RPC("SyncDataTowerRPC", RpcTarget.All, towerIds, towerTeams);
    }

    [PunRPC]
    public void SyncDataTowerRPC(int[] towerIndexes, byte[] teams)
    {
        foreach (var index in towerIndexes)
        {
            var entity = EntityCollectionManager.GetEntityByIndex(index);
            if (!towers.Contains(entity))
            {
                towers.Add(entity);
            }
        }
    }

    #endregion

    #region Minions

    private void SpawnWaves()
    {
        if (timer < timeBetweenWaves) return;
        timer = 0;
        for (var i = 0; i < minionsPerWave; i++)
        {
            var spawnTimer = i * timeBetweenMinionSpawn;

            gsm.OnTick += TrySpawnMinion;

            void TrySpawnMinion()
            {
                if (timer < spawnTimer) return;
                SpawnMinion();
                gsm.OnTick -= TrySpawnMinion;
            }
        }
    }

    private void SpawnMinion()
    {
        var blueMinion = NetworkPoolManager
            .PoolInstantiate(minion.gameObject.name, SpawnPoints[0].position, Quaternion.identity)
            .GetComponent<Minion>();
        var redMinion = NetworkPoolManager
            .PoolInstantiate(minion.gameObject.name, SpawnPoints[1].position, Quaternion.identity)
            .GetComponent<Minion>();
        blueMinion.SyncInstantiate(Enums.Team.Team1);
        redMinion.SyncInstantiate(Enums.Team.Team2);
        var blueBt = blueMinion.GetComponent<MinionBT>();
        var redBt = redMinion.GetComponent<MinionBT>();
        blueBt.enabled = true;
        redBt.enabled = true;
        blueBt.waypoints = waypointsTeamBlue;
        redBt.waypoints = waypointsTeamRed;
        blueMinion.animatorTrap = animatorsTraps[0];
        redMinion.animatorTrap = animatorsTraps[1];
        blueMinion.BasketGoal = BasketGoal[0];
        redMinion.BasketGoal = BasketGoal[1];
        blueBt.OnStart();
        redBt.OnStart();
    }

    #endregion
}