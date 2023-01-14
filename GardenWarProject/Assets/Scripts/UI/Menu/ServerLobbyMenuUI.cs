using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ServerLobbyMenuUI : MonoBehaviour, ILobbyCallbacks
{
    [Header("UI Components")]
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button displayJoinLobbyPopUpButton;
    [SerializeField] private Button joinLobbyButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditButton;
    [SerializeField] private TMP_InputField joinRoomTMPInputField;
    [SerializeField] private Button quitButton;
    
    [SerializeField] private GameObject creditGO;
    [SerializeField] private GameObject joinLobbyPopUpGo;
    [SerializeField] private GameObject settingsCanvasGo;

    
    
    [Header("Debug")]
    [SerializeField] private TMP_InputField createRoomTMPInputField;
    [SerializeField] private TextMeshProUGUI connectionStatusText;

    private List<string> unavailableRooms = new List<string>();

    public void Start()
    {
        joinLobbyPopUpGo.SetActive(false);
        settingsCanvasGo.SetActive(false);
        
        createLobbyButton.onClick.AddListener(CreateRoom);
        joinLobbyButton.onClick.AddListener(JoinRoom);
        
        settingsButton.onClick.AddListener(() => settingsCanvasGo.SetActive(true));
        creditButton.onClick.AddListener(() => creditGO.SetActive(true));
        
        displayJoinLobbyPopUpButton.onClick.AddListener(() => joinLobbyPopUpGo.SetActive(!joinLobbyPopUpGo.activeSelf));
        
        quitButton.onClick.AddListener(QuitGame);
        
        quitButton.onClick.AddListener(QuitGame);
    }
    
    private void Update()
    {
        connectionStatusText.text = $"Is connected and ready : {PhotonNetwork.IsConnectedAndReady}" +
                                    $"\nStatus : {PhotonNetwork.NetworkClientState}";
    }

    private void TogglePopUp()
    {
    }

    public void CreateRoom()
    {
        if (NetworkManager.Instance == null) Debug.LogError("no NetworkManager");

        var roomNumber = Random.Range(0, 1_000_000); 
        var roomName = $"{roomNumber:000000}";

        while (unavailableRooms.Contains(roomName))
        {
            roomNumber = Random.Range(0, 1_000_000); 
            roomName = $"{roomNumber:000000}";
        }
        
        NetworkManager.Instance.CreateRoom(roomName);
    }

    public void JoinRoom()
    {
        if(NetworkManager.Instance == null) Debug.LogError("no NetworkManager");
        
        NetworkManager.Instance.JoinRoom(joinRoomTMPInputField.text);
    }


    private void QuitGame()
    {
        Application.Quit();
    }

    public void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
    }

    public void OnLeftLobby()
    {
        Debug.Log("Left Lobby");
    }

    public void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var info in roomList)
        {
            if (info.RemovedFromList)
            {
                Debug.Log($"Removed {info.Name}");
                unavailableRooms.Remove(info.Name);
            }
            else
            {
                Debug.Log($"Added {info.Name}");
                unavailableRooms.Add(info.Name);
            }
        }
    }

    public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
        
    }
}
