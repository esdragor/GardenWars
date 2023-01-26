using GameStates;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI masterText;
    [SerializeField] private TextMeshProUGUI frameDataText;

    private GameStateMachine gsm => GameStateMachine.Instance;
    
    private double timer;
    private int frameCount;
    private int tickCount;

    private void Start()
    {
        if (!GameSettingsManager.isDebug)
        {
            masterText.gameObject.SetActive(false);
            frameDataText.gameObject.SetActive(false);
            return;
        }
        
        var id = PhotonNetwork.LocalPlayer.ActorNumber;
        masterText.text = ($"Client {id} / {GameStateMachine.Instance.GetPlayerTeam()}") + (PhotonNetwork.IsMasterClient ? " (Master)" : ""); 
        gsm.OnTick += (() => tickCount++);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        frameCount++;
        if(timer < 1) return;
        timer = 0;
        UpdateFrameData();
    }

    private void UpdateFrameData()
    {
        frameDataText.text = $"FPS : {frameCount} \nTPS : {tickCount}";
        frameCount = 0;
        tickCount = 0;
    }
}
