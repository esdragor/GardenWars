using GameStates;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UIComponents
{
    public class UICandyTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [TextArea(6, 10), SerializeField] private string fighterText;
        [TextArea(6, 10), SerializeField] private string scavengerText;

        public void OnPointerEnter(PointerEventData eventData)
        {
            var champion = GameStateMachine.Instance.GetPlayerChampion();
            
            var text = champion.isFighter ? fighterText : scavengerText;
            text = text.Replace("Candy_Count", $"{Minion.level+5}"); //TODO - wesh c hardcodé

            ToolTipManager.Show(text,"**Candies**");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ToolTipManager.Hide();
        }
    }
}