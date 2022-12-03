using Photon.Pun;
using UnityEngine;

namespace GameStates.States
{
    public class InGameState : GameState
    {
        public InGameState(GameStateMachine sm) : base(sm) { }

        private double lastTickTime;
        private double timer;

        public override void StartState()
        {
            InputManager.EnablePlayerMap(true);
            lastTickTime = PhotonNetwork.Time;
        }

        public override void UpdateState()
        {
            if (IsWinConditionChecked() && PhotonNetwork.IsMasterClient)
            {
                sm.SendWinner(sm.winner);
                sm.SwitchState(3);
                return;
            }
            
            timer = PhotonNetwork.Time - lastTickTime;

            if (!(timer >= 1.0 / sm.tickRate)) return;
            
            if (PhotonNetwork.IsMasterClient) sm.Tick();
            sm.TickFeedback();
        }

        public override void ExitState() { }

        public override void OnAllPlayerReady() { }

        private bool IsWinConditionChecked()
        {
            // Check win condition for any team
            //sm.winner = Enums.Team.Neutral;
            return sm.winner != Enums.Team.Neutral;
        }
    }
}