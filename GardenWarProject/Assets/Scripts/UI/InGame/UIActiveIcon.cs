using Entities.Capacities;
using UnityEngine;
using UnityEngine.UI;

public class UIActiveIcon : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image cooldownOverlayImage;
    private ActiveCapacity capacity;

    private void Start()
    {
        cooldownOverlayImage.fillAmount = 0;
    }

    public void SetCapacity(ActiveCapacity active)
    {
        capacity = active;
        iconImage.sprite = capacity.AssociatedActiveCapacitySO().icon;
        cooldownOverlayImage.fillAmount = 0;
    }
}
