using System.Collections;
using System.Collections.Generic;
using Entities.Capacities;
using Entities.Champion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPassiveIcon : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI stackCount;

    private Champion linkedChampion;

    public void LinkWithChampion(Champion champion)
    {
        linkedChampion = champion;
    }

    public void LinkWithPassive(PassiveCapacity passiveCapacity)
    {
        image.sprite = passiveCapacity.AssociatedPassiveCapacitySO().icon;
        stackCount.text = passiveCapacity.stackable ? "1" : "";
    }
}
