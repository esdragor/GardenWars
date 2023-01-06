using GameStates;
using Photon.Pun;
using UnityEngine;

namespace Entities.Champion
{
    public class ChampionMeshLinker : MonoBehaviourPun
    {
        public Animator[] animators = new Animator[0];
        [SerializeField] private SkinnedMeshRenderer[] teamColorfulParts = new SkinnedMeshRenderer[0];
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        private GameStateMachine gsm;

        private void Start()
        {
            gsm = GameStateMachine.Instance;
        }

        public void LinkTeamColor(Material[] mats)
        {
            if (teamColorfulParts.Length <= 0) return;

            for (int i = 0; i < teamColorfulParts.Length; i++)
            {
                teamColorfulParts[i].material = mats[i];
            }
        }
    }
}