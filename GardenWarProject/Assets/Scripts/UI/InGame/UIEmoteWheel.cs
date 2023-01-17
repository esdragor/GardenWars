using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIComponents
{
    public class UIEmoteWheel : MonoBehaviour
    {
        [SerializeField] private UIEmoteContainer[] containers = new UIEmoteContainer[6];

        public void InitWheel()
        {
            gameObject.SetActive(false);

            for (int i = 0; i < 6; i++)
            {
                var container = containers[i];
                
                container.LinkToWheel(this,i);
            }
        }

        public void SelectContainer(byte index)
        {
            UIManager.Instance.SetEmoteIndex(index);
        }
        
        public void Show(Vector2 pos)
        {
            transform.position = pos;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    
   
    }
}


