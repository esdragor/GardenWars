using GameStates;
using Photon.Pun;
using UnityEngine;

namespace Entities.Champion
{
    public class ChampionMeshLinker : MonoBehaviourPun
    {
        [SerializeField] private MeshRenderer[] teamColorfulParts;
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

            foreach (var rd in teamColorfulParts)
            {
                rd.material.SetColor(EmissionColor, color * 1f);
            }
        }
    }
}