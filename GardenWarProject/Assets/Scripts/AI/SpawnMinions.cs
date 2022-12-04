using System.Collections;
using System.Collections.Generic;
using Entities;
using GameStates;
using Photon.Pun;
using UnityEngine;

public class SpawnMinions : MonoBehaviourPun
{
    public int nb = 1;
    public float delayBetweenSpawn = 0.3f;
    
    [SerializeField] private Entity minion;
    [SerializeField] private List<Transform> SpawnPoints;
    [SerializeField] private Transform[] waypointsTeamBlue;
    [SerializeField] private Transform[] waypointsTeamRed;

    private Enums.Team myTeam;

    IEnumerator Spawner(byte Team)
    {
        for (int i = 0; i < nb; i++)
        {
           
            //Entity entity = PoolNetworkManager.Instance.PoolInstantiate(minion, SpawnPoints[Team - 1].position, Quaternion.identity);
            Entity entity = PhotonNetwork.Instantiate("Minion", SpawnPoints[Team - 1].position, Quaternion.identity).GetComponent<Entity>();
            MyAIBT BT = entity.GetComponent<MyAIBT>();
            BT.enabled = true;
            BT.waypoints = ((Enums.Team)Team == Enums.Team.Team1) ? waypointsTeamBlue : waypointsTeamRed;
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
        
        photonView.RPC("Spawn", RpcTarget.MasterClient, (int)myTeam);

    }

    [PunRPC]
    public void Spawn(int team)
    {
        StartCoroutine(Spawner((byte)team));
    }
}