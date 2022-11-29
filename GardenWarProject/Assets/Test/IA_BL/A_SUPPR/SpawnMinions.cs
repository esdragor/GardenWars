using System.Collections;
using System.Collections.Generic;
using GameStates;
using Photon.Pun;
using UnityEngine;

public class SpawnMinions : MonoBehaviour
{
    [SerializeField] private GameObject minion;
    [SerializeField] private List<Transform> SpawnPoints;
    
    private Enums.Team myTeam;
    
    // Start is called before the first frame update
    void Start()
    {
        myTeam = GameStateMachine.Instance.GetPlayerTeam();
        
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnMinion();
        }
    }

    void SpawnMinion()
    {
        Instantiate(minion, SpawnPoints[(int)myTeam - 1]);
    }
}
