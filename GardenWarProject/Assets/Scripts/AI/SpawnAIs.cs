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

    private Enums.Team myTeam;


    IEnumerator SpawnerMinions(byte Team)
    {
        for (int i = 0; i < nb; i++)
        {
            //Entity entity = PoolNetworkManager.Instance.PoolInstantiate(minion, SpawnPoints[Team - 1].position, Quaternion.identity);
            Entity entity = PhotonNetwork.Instantiate("Minion", SpawnPoints[Team - 1].position, Quaternion.identity)
                .GetComponent<Entity>();
            yield return new WaitForSeconds(0.1f);
            photonView.RPC("SyncDataMinions", RpcTarget.All, Team, entity.photonView.ViewID);
            yield return new WaitForSeconds(0.1f);
            MyAIBT BT = entity.GetComponent<MyAIBT>();
            BT.enabled = true;
            BT.waypoints = ((Enums.Team)Team == Enums.Team.Team1) ? waypointsTeamBlue : waypointsTeamRed;
            entity.team = (Enums.Team)Team;
            BT.OnStart();
            yield return new WaitForSeconds(delayBetweenSpawn);
        }
    }

    IEnumerator SpawnerTowers()
    {
        Entity RedTower = PhotonNetwork.Instantiate("Tower", SpawnerTower[0].position, Quaternion.identity)
            .GetComponent<Entity>();
        Entity BlueTower = PhotonNetwork.Instantiate("Tower", SpawnerTower[1].position, Quaternion.identity)
            .GetComponent<Entity>();
        yield return new WaitForSeconds(0.1f);
        RedTower.GetComponent<TowerBT>().OnStart();
        BlueTower.GetComponent<TowerBT>().OnStart();
        photonView.RPC("SyncDataTower", RpcTarget.All,
            new int[] { RedTower.photonView.ViewID, BlueTower.photonView.ViewID });
    }


    // Start is called before the first frame update
    void Start()
    {
        myTeam = GameStateMachine.Instance.GetPlayerTeam();
        if (!PhotonNetwork.LocalPlayer.IsLocal) return;
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC("SpawnTowers", RpcTarget.MasterClient, (int)myTeam);
        photonView.RPC("SpawnMinions", RpcTarget.MasterClient, (int)myTeam);
    }

    [PunRPC]
    public void SpawnMinions(int team)
    {
        StartCoroutine(SpawnerMinions((byte)team));
    }

    [PunRPC]
    public void SpawnTowers(int team)
    {
        StartCoroutine(SpawnerTowers());
    }

    public void SwitchColor(Renderer render, Color c)
    {
        render.material.color = c;
    }
    
    
    [PunRPC]
    public void SyncDataMinions(byte team, int entityID)
    {
        Entity entity = EntityCollectionManager.GetEntityByIndex(entityID);

        entity.team = (Enums.Team)team;
        SwitchColor(entity.GetComponent<Renderer>(), (entity.team == Enums.Team.Team2) ? ColorTeam[0] : ColorTeam[1]);
    }

    [PunRPC]
    public void SyncDataTower(int[] IDS)
    {
        Entity entity = EntityCollectionManager.GetEntityByIndex(IDS[0]);
        entity.team = Enums.Team.Team2;
        SwitchColor(entity.GetComponent<Renderer>(), ColorTeam[0]);
        
        entity = EntityCollectionManager.GetEntityByIndex(IDS[1]);
        entity.team = Enums.Team.Team1;
        SwitchColor(entity.GetComponent<Renderer>(), ColorTeam[1]);
    }
}