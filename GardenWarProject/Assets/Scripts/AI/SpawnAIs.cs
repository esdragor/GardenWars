using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using Entities;
using GameStates;
using Photon.Pun;
using UnityEngine;

public class SpawnAIs : MonoBehaviourPun
{
    public int nb = 1;
    public float delayBetweenSpawn = 0.3f;

    [SerializeField] private Entity minion;
    [SerializeField] private List<Transform> SpawnPoints;
    [SerializeField] private Transform[] waypointsTeamBlue;
    [SerializeField] private Transform[] waypointsTeamRed;
    [SerializeField] private Transform[] SpawnerTower;
    [SerializeField] private Color[] ColorTeam;
    [SerializeField] private float DelayBetweenWaves = 30f;

    private bool StopSpawn = false;


    IEnumerator SpawnerMinions()
    {
        while (!StopSpawn)
        {
            for (int i = 0; i < nb; i++)
            {
                //Entity entity = PoolNetworkManager.Instance.PoolInstantiate(minion, SpawnPoints[Team - 1].position, Quaternion.identity);
                Entity BlueEntity = PhotonNetwork.Instantiate("Minion", SpawnPoints[0].position, Quaternion.identity)
                    .GetComponent<Entity>();
                Entity RedEntity = PhotonNetwork.Instantiate("Minion", SpawnPoints[1].position, Quaternion.identity)
                    .GetComponent<Entity>();
                yield return new WaitForSeconds(0.1f);
                photonView.RPC("SyncDataMinions", RpcTarget.All, BlueEntity.photonView.ViewID,
                    RedEntity.photonView.ViewID);
                yield return new WaitForSeconds(0.1f);
                BlueEntity.team = Enums.Team.Team1;
                RedEntity.team = Enums.Team.Team2;
                MyAIBT BlueBT = BlueEntity.GetComponent<MyAIBT>();
                MyAIBT RedBT = RedEntity.GetComponent<MyAIBT>();
                BlueBT.enabled = true;
                RedBT.enabled = true;
                BlueBT.waypoints = waypointsTeamBlue;
                RedBT.waypoints = waypointsTeamRed;
                BlueBT.OnStart();
                RedBT.OnStart();
                yield return new WaitForSeconds(delayBetweenSpawn);
            }

            yield return new WaitForSeconds(DelayBetweenWaves);
        }
    }

    IEnumerator SpawnerTowers()
    {
        Entity RedTower = PhotonNetwork.Instantiate("NewTower", SpawnerTower[0].position, Quaternion.identity)
            .GetComponent<Entity>();
        Entity BlueTower = PhotonNetwork.Instantiate("NewTower", SpawnerTower[1].position, Quaternion.identity)
            .GetComponent<Entity>();
        BlueTower.team = Enums.Team.Team1;
        RedTower.team = Enums.Team.Team2;
        yield return new WaitForSeconds(0.5f);
        RedTower.GetComponent<TowerBT>().OnStart();
        BlueTower.GetComponent<TowerBT>().OnStart();
        photonView.RPC("SyncDataTower", RpcTarget.All,
            new int[] { RedTower.photonView.ViewID, BlueTower.photonView.ViewID });
    }


    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SpawnTowers", RpcTarget.MasterClient);
            photonView.RPC("SpawnMinions", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    public void SpawnMinions()
    {
        StartCoroutine(SpawnerMinions());
    }

    [PunRPC]
    public void SpawnTowers()
    {
        StartCoroutine(SpawnerTowers());
    }


    [PunRPC]
    public void SyncDataMinions(int entityIDBlue, int entityIDRed)
    {
        Entity BlueEntity = EntityCollectionManager.GetEntityByIndex(entityIDBlue);
        Entity RedEntity = EntityCollectionManager.GetEntityByIndex(entityIDRed);

        BlueEntity.team = Enums.Team.Team1;
        RedEntity.team = Enums.Team.Team2;
        BlueEntity.ChangeColor();
        RedEntity.ChangeColor();
    }

    [PunRPC]
    public void SyncDataTower(int[] IDS)
    {
        Entity entity = EntityCollectionManager.GetEntityByIndex(IDS[0]);
        entity.team = Enums.Team.Team2;
        entity.ChangeColor();

        entity = EntityCollectionManager.GetEntityByIndex(IDS[1]);
        entity.team = Enums.Team.Team1;
        entity.ChangeColor();
    }
}