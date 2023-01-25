using System.Collections.Generic;
using System.Linq;
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

        private Queue<GameStateMachine.InGameMessage> messages = new Queue<GameStateMachine.InGameMessage>();
        private GameStateMachine.InGameMessage currentMessage;

        public override void StartState()
        {
            InputManager.EnablePlayerMap(true);
            
            lastTickTime = currentTime;

            sm.startTime = currentTime;

            sm.StartEntitySpawner();

            sm.ShowLoadingCanvas(false);
            
            messages.Clear();

            foreach (var message in sm.messages)
            {
                messages.Enqueue(message);
            }

            currentMessage = null;
        }

        public override void UpdateState()
        {
            if (IsWinConditionChecked() && GameStateMachine.isMaster)
            {
                sm.SendWinner(sm.winner);
                sm.SwitchState(3);
                return;
            }

            if (GameStateMachine.isMaster)
            {
                SendMessages();
                sm.UpdateEvent();
            }
            sm.UpdateEventFeedback();
            
            timer = currentTime - lastTickTime;

            sm.currentTime = currentTime;
            
            if (!(timer >= 1.0 / sm.tickRate)) return;
            lastTickTime = currentTime;
            
            if (GameStateMachine.isMaster) sm.Tick();
            sm.TickFeedback();
        }

        private void SendMessages()
        {
            if(currentMessage == null && messages.Count > 0) currentMessage = messages.Dequeue();
            
            if(currentMessage == null) return;
            
            Debug.Log($"Messages left : {messages.Count}, next message plays at {currentMessage.timeToDisplay} (now {GameStateMachine.gameTime})and is {currentMessage.textToDisplay}");

            if(GameStateMachine.gameTime < currentMessage.timeToDisplay) return;

            sm.DisplayMessage(currentMessage.textToDisplay);

            currentMessage = null;
        }

        public override void ExitState() { }

        public override void OnAllPlayerReady() { }

        private bool IsWinConditionChecked()
        {
            return sm.winner != Enums.Team.Neutral;
        }
    }
}