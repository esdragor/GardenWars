using Entities.Capacities;
using GameStates;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UIComponents
{
    public class UIActiveIconDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public ActiveCapacity capacity;
        private bool isPointerOver;

        public void DisplayText()
        {
            if(!isPointerOver) return;
            
            var so = capacity.AssociatedActiveCapacitySO();

            var text = capacity.level switch
            {
                1 => so.description,
                2 => so.description1,
                3 => so.description2,
                _ => so.description
            };


            var level = GameStateMachine.Instance.GetPlayerChampion().isFighter && capacity.AssociatedActiveCapacitySO().maxLevel != 1 ? $"[{capacity.level}]" : string.Empty;
            var header = $"{so.capacityName} {level}";
            
            ToolTipManager.Show(text,header);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            isPointerOver = true;
            
            DisplayText();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isPointerOver = false;
            
            ToolTipManager.Hide();
        }
    }
}