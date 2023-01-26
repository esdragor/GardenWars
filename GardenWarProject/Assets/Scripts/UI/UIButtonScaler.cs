using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIComponents
{
    public class UIButtonScaler : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
    {
        private Button _button;
        private Button button => _button ??= gameObject.GetComponent<Button>();
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if(!button.interactable) return;
            transform.DOKill();
            if (transform.localScale.x < 0)
            {
                transform.DOScaleX(-1.05f, 0.1f);
                transform.DOScaleY(1.05f, 0.1f);
                transform.DOScaleZ(1.05f, 0.1f);
                return;
            }
            transform.DOScale(1.05f, 0.1f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.DOKill();
            if (transform.localScale.x < 0)
            {
                transform.DOScaleX(-1f, 0.1f);
                transform.DOScaleY(1f, 0.1f);
                transform.DOScaleZ(1f, 0.1f);
                return;
            }
            transform.DOScale(1, 0.1f);
        }
    }
}

