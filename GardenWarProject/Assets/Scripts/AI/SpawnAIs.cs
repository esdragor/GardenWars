using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BehaviourTree;
using Entities;
using GameStates;
using Photon.Pun;
using UnityEngine;

public class SpawnAIs : MonoBehaviourPun
{
    [Header("Wave Management")]
    public int minionsPerWave = 1;
    public double timeBetweenMinionSpawn = 0.3;
    public double timeBetweenWaves = 30;
    [SerializeField] private double timeBeforeFirstWave = 5;
    [SerializeField] private List<Transform> SpawnPoints;
    [SerializeField] private Transform[] waypointsTeamBlue;
    [SerializeField] private Transform[] waypointsTeamRed;
    [SerializeField] private Entity minion;
    [SerializeField] private double timer;

    [Header("Towers")]
    [SerializeField] private Transform[] towerSpawnPoints;

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

    private void Start()
    {
        timer = 0;
        gsm.OnTick += OnTick;
    }
    
    public void Init()
    {
        towers.Clear();
        if(PhotonNetwork.IsMasterClient) InitTowers();
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
        
        timer = timeBetweenWaves-timeBeforeFirstWave;
        gsm.OnTick += SpawnWaves;

    }

    private void OnTick()
    {
        timer += gsm.increasePerTick;
    }

    #region Towers

    private void InitTowers()
    {
        var redTower = PhotonNetwork.Instantiate("NewTower", towerSpawnPoints[0].position, Quaternion.identity)
            .GetComponent<Entity>();
        var blueTower = PhotonNetwork.Instantiate("NewTower", towerSpawnPoints[1].position, Quaternion.identity)
            .GetComponent<Entity>();
        
        blueTower.SyncInstantiate(Enums.Team.Team1);
        redTower.SyncInstantiate(Enums.Team.Team2);
        
        towers.Add(blueTower);
        towers.Add(redTower);
    }
    
    private void SyncTowerData()
    {
        var towerIds = towers.Select(tower => tower.entityIndex).ToArray();
        var towerTeams = towers.Select(tower => (byte)tower.team).ToArray();
        photonView.RPC("SyncDataTowerRPC", RpcTarget.All,towerIds,towerTeams);
    }
    
    [PunRPC]
    public void SyncDataTowerRPC(int[] towerIndexes,byte[] teams)
    {
        for (var i = 0; i < towerIndexes.Length; i++)
        {
            var index = towerIndexes[i];
            var entity = EntityCollectionManager.GetEntityByIndex(index);
            //entity.InitEntity((Enums.Team) teams[i]);
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
        if(timer<timeBetweenWaves) return;
        timer = 0;
        for (var i = 0; i < minionsPerWave; i++)
        {
            var spawnTimer = i * timeBetweenMinionSpawn;

            gsm.OnTick += TrySpawnMinion;
            
            void TrySpawnMinion()
            {
                if(timer<spawnTimer) return;
                SpawnMinion();
                gsm.OnTick -= TrySpawnMinion;
            }
        }
    }

    private void SpawnMinion()
    {
        var blueMinion = PhotonNetwork.Instantiate(minion.gameObject.name, SpawnPoints[0].position, Quaternion.identity)
            .GetComponent<Entity>();
        var redMinion = PhotonNetwork.Instantiate(minion.gameObject.name, SpawnPoints[1].position, Quaternion.identity)
            .GetComponent<Entity>();
        blueMinion.SyncInstantiate(Enums.Team.Team1);
        redMinion.SyncInstantiate(Enums.Team.Team2);
        var blueBt = blueMinion.GetComponent<MyAIBT>();
        var redBt = redMinion.GetComponent<MyAIBT>();
        blueBt.enabled = true;
        redBt.enabled = true;
        blueBt.waypoints = waypointsTeamBlue;
        redBt.waypoints = waypointsTeamRed;
        blueBt.OnStart();
        redBt.OnStart();
    }

    #endregion
}