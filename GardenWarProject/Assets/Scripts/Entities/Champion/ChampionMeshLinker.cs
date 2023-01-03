using GameStates;
using Photon.Pun;
using UnityEngine;

namespace Entities.Champion
{
    public class ChampionMeshLinker : MonoBehaviourPun
    {
        public Animator[] animators = new Animator[0];
        [SerializeField] private MeshRenderer[] teamColorfulParts = new MeshRenderer[0];
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        private GameStateMachine gsm;

        private void Start()
        {
            gsm = GameStateMachine.Instance;
        }

        public void LinkTeamColor(Enums.Team team)
        {
            var color = Color.white;
            if (gsm != null)
            {
                foreach (var tc in GameStateMachine.Instance.teamColors)
                {
                    if (team != tc.team) continue;
                    color = tc.color;
                    break;
                }
            }

            if (teamColorfulParts.Length <= 0) return;
            foreach (var rd in teamColorfulParts)
            {
                rd.material.SetColor(EmissionColor, color * 1f);
            }
        }
    }
}