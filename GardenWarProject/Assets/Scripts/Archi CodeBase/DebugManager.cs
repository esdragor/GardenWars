using System;
using GameStates;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI clientDataText;
    [SerializeField] private TextMeshProUGUI frameDataText;
    private GameStateMachine gsm => GameStateMachine.Instance;
    
    private double timer;
    private int frameCount;
    private int tickCount;

    private void Start()
    {
        var id = PhotonNetwork.LocalPlayer.ActorNumber;
        clientDataText.text = $"Client {id} / {GameStateMachine.Instance.GetPlayerTeam()}";
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

    public void OnStartInGameState()
    {
        gsm.SwitchState(2);
    }
    
    public void OnTeamWins(int index)
    {
        gsm.winner = index == 0 ? Enums.Team.Team1 : Enums.Team.Team2;
    }

    public void OnDieButtonClick()
    {
        gsm.GetPlayerChampion().RequestDie(-1);
    }

    public void OnDamageButtonClick()
    {
        gsm.GetPlayerChampion().DecreaseCurrentHpRPC(2, -1);
    }
}
