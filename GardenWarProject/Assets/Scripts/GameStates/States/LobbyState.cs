using Entities;

namespace GameStates.States
{
    public class LobbyState : GameState
    {
        public LobbyState(GameStateMachine sm) : base(sm) { }

        public override void StartState()
        {
            EntityCollectionManager.ClearDict();

            InputManager.EnablePlayerMap(false);
            InputManager.EnablePlayerUIMap(true);
        }

        public override void UpdateState() { }

        public override void ExitState()
        {
            sm.ResetPlayerReady();
            InputManager.EnablePlayerMap(false);
            InputManager.EnablePlayerUIMap(false);
        }

        public override void OnAllPlayerReady()
        {
            sm.StartLoadingMap();
        }
    }
}