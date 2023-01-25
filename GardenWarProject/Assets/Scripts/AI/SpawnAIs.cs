using System;
using System.Collections.Generic;
using System.Linq;
using BehaviourTree;
using Entities;
using Entities.Champion;
using GameStates;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class RespawnPinata
{
    public float delay;
    public Transform pos;
}

public class SpawnAIs : MonoBehaviourPun
{
    [HideInInspector] public List<RespawnPinata> PinatasRespawned = new List<RespawnPinata>();

    [Header("Wave Management")]
    public int minionsPerWave = 1;
    public double timeBetweenMinionSpawn = 0.3;
    public double timeBetweenWaves = 30;
    [SerializeField] private double timeBeforeFirstWave = 5;
    [SerializeField] private List<Transform> SpawnPoints = new List<Transform>();
    [SerializeField] private int minionLevel;
    
    [Header("Minions")]
    [SerializeField] private Transform[] waypointsTeamBlue;
    [SerializeField] private Transform[] waypointsTeamRed;
    [SerializeField] private Entity minion;
    [SerializeField] private Animator[] animatorsTraps;
    [SerializeField] private Transform[] BasketGoal;

    [Header("Towers")]
    [SerializeField] private Transform[] towerSpawnPoints;

    [Serializable]
    public struct OtherSpawnPoints
    {
        public Transform pinataSpawn;
        public List<Transform> candySpawn;
    }

    [SerializeField] private List<OtherSpawnPoints> spawnPoints = new List<OtherSpawnPoints>();
    private Dictionary<Transform, List<Transform>> candySpawnDict = new Dictionary<Transform, List<Transform>>();

    [Header("Pinata")]
    [SerializeField] private float timeBeforeSpawnPinata = 4;
    [SerializeField] private float timeBeforeRespawnPinata = 5;
    [SerializeField] private float timeBetweenPinataLevelUp = 30;
    [SerializeField] private int pinataLevel;

    [Header("Candy")]
    [SerializeField] private int candyPerBag = 20;
    [SerializeField] private float timeBeforeSpawnCandy = 4;
    [SerializeField] private float timeBetweenCandies = 5;

    [Header("Timers")]
    [SerializeField] private double minionTimer;
    [SerializeField] private double pinataTimer;
    [SerializeField] private double candyTimer;
    
    private Pinata currentPinata;
    private Transform previousSpawnPos;

    private GameStateMachine gsm => GameStateMachine.Instance;

    private readonly List<Entity> towers = new List<Entity>();

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

    public void Init()
    {
        towers.Clear();
        
        minionTimer = 0;
        pinataTimer = 0;
        pinataLevel = 0;
        minionLevel = -1;

        if (!PhotonNetwork.IsMasterClient) return;
        
        InitTowers();

        foreach (var other in spawnPoints)
        {
            candySpawnDict.Add(other.pinataSpawn,other.candySpawn);
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

        minionTimer = timeBetweenWaves - timeBeforeFirstWave;
        pinataTimer = timeBetweenPinataLevelUp - timeBeforeSpawnPinata;
        candyTimer = timeBetweenCandies - timeBeforeSpawnCandy;

        currentPinata = null;
        
        gsm.OnTick += IncreaseTimers;
    }

    private void IncreaseTimers()
    {
        minionTimer += gsm.increasePerTick;
        pinataTimer += gsm.increasePerTick;
        candyTimer += gsm.increasePerTick;
        
        SpawnMinionWaves();
        
        SpawnPinatas();

        SpawnCandies();
    }
    
    #region Minions

    private void SpawnMinionWaves()
    {
        if (minionTimer < timeBetweenWaves) return;
        minionTimer = 0;

        minionLevel++;
        Minion.level = minionLevel;
        
        for (var i = 0; i < minionsPerWave; i++)
        {
            var spawnTimer = i * timeBetweenMinionSpawn;

            gsm.OnTick += TrySpawnMinion;

            void TrySpawnMinion()
            {
                if (minionTimer < spawnTimer) return;
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

    #region Pinata

    private bool sentMessage = false;
    
    private void SpawnPinatas()
    {
        if (!sentMessage && pinataTimer >= timeBetweenPinataLevelUp - gsm.pinataSpawnMessageDelay)
        {
            sentMessage = true;
            gsm.DisplayMessage(gsm.messagePrePinataSpawn);
        }
        
        if(pinataTimer < timeBetweenPinataLevelUp) return;
        
        pinataTimer = 0;
        sentMessage = false;
        
        SpawnPinata(NextPinataSpawnPoint());
    }
    
    private void SpawnPinata(Transform tr)
    {
        pinataLevel++;
        Pinata.level = pinataLevel;
        
        if (currentPinata != null)
        {
            currentPinata.OnDie -= OnPinataDie;
            
            if(currentPinata.IsAlive()) currentPinata.DieRPC(currentPinata.entityIndex);
        }
        
        currentPinata = NetworkPoolManager.PoolInstantiate("Pinata", tr.position, Quaternion.identity).GetComponent<Pinata>();
        currentPinata.SyncInstantiate(Enums.Team.Neutral);
            
        currentPinata.OnDie += OnPinataDie;
        
    }

    private Transform NextPinataSpawnPoint()
    {
        if (currentPinata == null)
        {
            previousSpawnPos = spawnPoints[0].pinataSpawn;
            return previousSpawnPos;
        }
        
        var tr = previousSpawnPos;
        while (tr == previousSpawnPos)
        {
            tr = spawnPoints[Random.Range(0, spawnPoints.Count)].pinataSpawn;
        }

        previousSpawnPos = tr;
        return previousSpawnPos;
    }

    private void OnPinataDie(int _)
    {
        pinataTimer = timeBetweenPinataLevelUp - timeBeforeRespawnPinata;
    }

    #endregion

    #region Candy

    private void SpawnCandies()
    {
        if(candyTimer < timeBetweenCandies) return;
        
        candyTimer = 0;


        if (previousSpawnPos == null)
        {
            gsm.GetPlayerChampion().SpawnCandy(candyPerBag,candySpawnDict[spawnPoints[0].pinataSpawn]);
            return;
        }
        gsm.GetPlayerChampion().SpawnCandy(candyPerBag,candySpawnDict[previousSpawnPos]);
    }

    #endregion
    
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
        
        foreach (var entity in towers)
        {
            if(entity is Tower tower) tower.SetOutlineColor();
        }
    }

    #endregion
}