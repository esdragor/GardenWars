using System;
using Entities.Champion;
using UnityEngine;
using UnityEngine.UI;

namespace UIComponents
{
    public class MinimapPlayerIcon : MonoBehaviour
    {
        public Champion associatedChampion;
        [SerializeField] private Image playerIcon;
        [SerializeField] private Image borderImage;
        private GameObject iconObj;
        private GameObject borderObj;
        private RectTransform _rectTransform;
        public RectTransform rectTransform => _rectTransform;
        
        public void LinkToChampion(Champion champion)
        {
            _rectTransform = GetComponent<RectTransform>();
            iconObj = playerIcon.gameObject;
            borderObj = borderImage.gameObject;
            
            associatedChampion = champion;
            playerIcon.sprite = champion.currentSo.portrait;
        }

        public void ShowIcon(bool value)
        {
            iconObj.SetActive(value);
            borderObj.SetActive(value);
        }
    }

}
