using GameStates;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance;
    public string currentRoomName;

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene(1);
    }

    public void CreateRoom(string roomName)
    {
        GUIUtility.systemCopyBuffer = roomName;
        PhotonNetwork.CreateRoom(roomName);
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinOrCreateRoom(roomName,null,null);
    }

    public override void OnJoinedRoom()
    {
        currentRoomName = PhotonNetwork.CurrentRoom.Name;
        PhotonNetwork.LoadLevel(2);
    }

    public override void OnLeftRoom()
    {
        var sm = GameStateMachine.Instance;
        if(sm == null) return;
        sm.RequestRemovePlayer();
    }
}
