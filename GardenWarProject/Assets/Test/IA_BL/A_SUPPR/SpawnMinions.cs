using System.Collections;
using System.Collections.Generic;
using Entities;
using GameStates;
using Photon.Pun;
using UnityEngine;

public class SpawnMinions : MonoBehaviour
{
    [SerializeField] private Entity minion;
    [SerializeField] private List<Transform> SpawnPoints;
    [SerializeField] private Transform[] waypointsTeamA;
    [SerializeField] private Transform[] waypointsTeamB;

    private Enums.Team myTeam;

    // Start is called before the first frame update
    void Start()
    {
        myTeam = GameStateMachine.Instance.GetPlayerTeam();
        for (int i = 0; i < 10; i++)
        {
            SpawnMinion();
        }

    }

    void SpawnMinion()
    {
        Entity entity = PoolNetworkManager.Instance.PoolInstantiate(minion, SpawnPoints[(int)myTeam - 1].position, Quaternion.identity);
        entity.GetComponent<MyAIBT>().waypoints = (myTeam == Enums.Team.Team1) ? waypointsTeamA : waypointsTeamB;
    }
}