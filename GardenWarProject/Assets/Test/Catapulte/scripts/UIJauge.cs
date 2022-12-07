using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIJauge : MonoBehaviour
{
    [SerializeField] private Slider jaugeslider;

    public void UpdateJaugeSlider(float min, float max, double current)
    {
        jaugeslider.minValue = min;
        jaugeslider.maxValue = max;
        jaugeslider.value = (float)current;
    }
}