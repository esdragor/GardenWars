using System.Collections;
using System.Collections.Generic;
using Entities;
using GameStates;
using Photon.Pun;
using UnityEngine;

public class SpawnMinions : MonoBehaviour
{
    public int nb = 1;
    public float delayBetweenSpawn = 0.3f;
    
    [SerializeField] private Entity minion;
    [SerializeField] private List<Transform> SpawnPoints;
    [SerializeField] private Transform[] waypointsTeamBlue;
    [SerializeField] private Transform[] waypointsTeamRed;

    private Enums.Team myTeam;

    IEnumerator Spawner()
    {
        for (int i = 0; i < nb; i++)
        {
            Entity entity = PoolNetworkManager.Instance.PoolInstantiate(minion, SpawnPoints[(int)myTeam - 1].position, Quaternion.identity);
            MyAIBT BT = entity.GetComponent<MyAIBT>();
            BT.enabled = true;
            BT.waypoints = (myTeam == Enums.Team.Team1) ? waypointsTeamBlue : waypointsTeamRed;
            yield return new WaitForSeconds(0.1f);
            BT.OnStart();
            yield return new WaitForSeconds(delayBetweenSpawn);
        }
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        myTeam = GameStateMachine.Instance.GetPlayerTeam();
        if (!PhotonNetwork.LocalPlayer.IsLocal) return;
        StartCoroutine(Spawner());

    }

    void SpawnMinion()
    {

    }
}