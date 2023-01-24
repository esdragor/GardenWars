using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UIComponents
{
    public class UIButtonScaler : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            transform.DOKill();
            transform.DOScale(1.1f, 0.1f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.DOKill();
            transform.DOScale(1f, 0.1f);
        }
    }
}

