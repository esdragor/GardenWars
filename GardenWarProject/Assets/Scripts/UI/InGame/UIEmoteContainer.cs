using System;
using System.Collections;
using System.Collections.Generic;
using GameStates;
using UIComponents;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIComponents
{
    public class UIEmoteContainer : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
    {
        [SerializeField] private RawImage image;
        
        private UIEmoteWheel wheel;

        private byte index;

        public Texture texture => image.texture;

        public void LinkToWheel(UIEmoteWheel wheel,int i)
        {
            this.wheel = wheel;
            index = (byte)i;
            image.texture = GameStateMachine.Instance.GetPlayerEmotes()[i];
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            wheel.SelectContainer(index);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            wheel.SelectContainer(6);
        }
    }
}


