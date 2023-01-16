using System.Collections;
using System.Collections.Generic;
using GameStates;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Destroyer : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("ZA WARUDO");

        GameStateMachine.Instance.photonView.ViewID = 69;
        
        Destroy(GameStateMachine.Instance.gameObject);
        
                
        NetworkManager.Destroy();

        PhotonNetwork.Disconnect();

        SceneManager.LoadSceneAsync(0);
    }

    
}
