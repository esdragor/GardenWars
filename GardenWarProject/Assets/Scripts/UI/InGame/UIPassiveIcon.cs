using System;
using Entities;
using Entities.Capacities;
using Entities.Champion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPassiveIcon : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Image backGroundImage;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI stackCount;
    [Header("Config")]
    [SerializeField] private Color positiveColor;
    [SerializeField] private Color negativeColor;
    
    
    private PassiveCapacity linkedCapacity;
    private Champion linkedChampion;

    public void LinkWithChampion(Champion champion, PassiveCapacity capacity, Action<Entity> removedCallback)
    {
        linkedChampion = champion;
        linkedCapacity = capacity;

        UpdatePassive(linkedChampion);
        
        capacity.OnAddedEffectsFeedbackCallback += UpdatePassive;

        capacity.OnRemovedEffectsFeedbackCallback += removedCallback;
    }

    public bool RemovePassive(Entity _)
    {
        if (!linkedCapacity.stackable) return true;

        if (linkedCapacity.count <= 0) return true;
        
        UpdatePassive(_);
        
        return false;
    }

    private void UpdatePassive(Entity _)
    {
        backGroundImage.color =
            linkedCapacity.AssociatedPassiveCapacitySO().types.Contains(Enums.CapacityType.Positive) ? positiveColor : negativeColor;
        var icon = linkedCapacity.AssociatedPassiveCapacitySO().icon;

        if (icon != null) image.sprite = icon;
        image.color = icon != null ? Color.white : TransparentColor();
        
        stackCount.text = linkedCapacity.stackable ? $"{linkedCapacity.count}" : "";
    }

    private Color TransparentColor()
    {
        var color = Color.white;
        color.a = 0;
        return color;
    }
}
