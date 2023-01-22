using System;
using System.Collections;
using System.Collections.Generic;
using Entities.Capacities;
using Entities.Champion;
using Photon.Pun;
using UnityEngine;

public class Fountain : MonoBehaviour
{
    [SerializeField] private FountainHealSO heal;
    [SerializeField] private Enums.Team team;

    private void OnTriggerEnter(Collider other)
    {
        if(!PhotonNetwork.IsMasterClient) return;
        
        var champion = other.gameObject.GetComponent<Champion>();
        if(champion == null) return;
        if (champion.team != team) return;
        
        champion.AddPassiveCapacityRPC(heal.indexInCollection);
    }

    private void OnTriggerExit(Collider other)
    {
        if(!PhotonNetwork.IsMasterClient) return;
        
        var champion = other.gameObject.GetComponent<Champion>();
        if(champion == null) return;
        if(champion.team != team) return;
        
        champion.RemovePassiveCapacityByIndexRPC(heal.indexInCollection);
    }
}
