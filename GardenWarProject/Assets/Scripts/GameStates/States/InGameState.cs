using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;

namespace GameStates.States
{
    public class InGameState : GameState
    {
        public InGameState(GameStateMachine sm) : base(sm) { }

        private double currentTime => GameStateMachine.isOffline ? Time.timeAsDouble : PhotonNetwork.Time;
        private double lastTickTime;
        private double timer;

        public override void StartState()
        {
            InputManager.EnablePlayerMap(true);
            
            lastTickTime = currentTime;

            sm.startTime = currentTime;
            
            sm.StartEntitySpawner();

            UIManager.Instance.ShowLoadingCanvas(false);
        }

        public override void UpdateState()
        {
            if (IsWinConditionChecked() && GameStateMachine.isMaster)
            {
                sm.SendWinner(sm.winner);
                sm.SwitchState(3);
                return;
            }
            
            if (GameStateMachine.isMaster) sm.UpdateEvent();
            sm.UpdateEventFeedback();
            
            timer = currentTime - lastTickTime;

            if (!(timer >= 1.0 / sm.tickRate)) return;
            lastTickTime = currentTime;
            
            if (GameStateMachine.isMaster) sm.Tick();
            sm.TickFeedback();
        }

        public override void ExitState() { }

        public override void OnAllPlayerReady() { }

        private bool IsWinConditionChecked()
        {
            return sm.winner != Enums.Team.Neutral;
        }
    }
}