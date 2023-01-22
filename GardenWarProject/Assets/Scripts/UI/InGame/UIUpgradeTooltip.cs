using System.Collections;
using System.Collections.Generic;
using GameStates;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UIComponents
{
    public class UIUpgradeTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [TextArea(6, 10), SerializeField] private string fighterTextNoUpgrade;
        [TextArea(6, 10), SerializeField] private string fighterTextUpgrade;
        [TextArea(6, 10), SerializeField] private string scavengerTextNoUpgrade;
        [TextArea(6, 10), SerializeField] private string scavengerTextUpgrade;

        public void OnPointerEnter(PointerEventData eventData)
        {
            var champion = GameStateMachine.Instance.GetPlayerChampion();
            
            var text = champion.isFighter ? fighterTextNoUpgrade : scavengerTextNoUpgrade;
            if (champion.upgrades > 0) text = champion.isFighter ? fighterTextUpgrade : scavengerTextUpgrade;
            
            ToolTipManager.Show(text,$"**Upgrade**");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ToolTipManager.Hide();
        }
    }
}

