using System;
using Photon.Pun;
using UnityEngine;

namespace Entities.Champion
{
    public class ChampionMeshLinker : MonoBehaviourPun
    {
        public Animator[] animators = Array.Empty<Animator>();
        [SerializeField] private SkinnedMeshRenderer[] teamColorfulParts = Array.Empty<SkinnedMeshRenderer>();
        public SkinnedMeshRenderer[] renderers => teamColorfulParts;
        
        public void LinkTeamColor(Material[] mats)
        {
            if (teamColorfulParts.Length <= 0) return;

            for (int i = 0; i < mats.Length; i++)
            {
                teamColorfulParts[i].material = mats[i];
            }
        }
        
    }
}