using Photon.Pun;
using UnityEngine;

public class GSMINIT : MonoBehaviour
{
    public void Start()
    {
        PhotonNetwork.Instantiate("GameStateMachine",Vector3.zero, Quaternion.identity);
    }
}
