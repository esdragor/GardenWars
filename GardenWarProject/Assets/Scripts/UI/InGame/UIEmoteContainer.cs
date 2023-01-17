using System;
using System.Collections;
using System.Collections.Generic;
using UIComponents;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIComponents
{
    public class UIEmoteContainer : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
    {
        private UIEmoteWheel wheel;
        
        private byte index;

        public RawImage image { get; private set; }
        public Texture texture => image.texture;

        private void Start()
        {
            image = GetComponent<RawImage>();
        }

        public void LinkToWheel(UIEmoteWheel wheel,int i)
        {
            this.wheel = wheel;
            index = (byte)i;
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


