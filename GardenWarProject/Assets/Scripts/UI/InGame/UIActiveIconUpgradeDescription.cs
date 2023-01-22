using System;
using Entities.Capacities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UIComponents
{
    public class UIActiveIconUpgradeDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public ActiveCapacity capacity;
        private bool isPointerOver;

        public void DisplayText()
        {
            if(!isPointerOver || !gameObject.activeSelf) return;
            
            var so = capacity.AssociatedActiveCapacitySO();

            var text = (capacity.level+1) switch
            {
                1 => so.description,
                2 => so.description1,
                3 => so.description2,
                _ => string.Empty
            };
            
            var header = $"**Next Level**";
            
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

        private void OnDisable()
        {
            if(isPointerOver) ToolTipManager.Hide();
        }
    }
}