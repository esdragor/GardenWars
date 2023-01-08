using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIJauge : MonoBehaviour
{
    [SerializeField] private Slider jaugeslider;
    [SerializeField] private TMP_Text Textslider;

    public void UpdateJaugeSlider(float min, float max, double current)
    {
        if (!jaugeslider.gameObject.activeSelf) jaugeslider.gameObject.SetActive(true);
        jaugeslider.minValue = min;
        jaugeslider.maxValue = max;
        jaugeslider.value = (float)current;
    }
    
    public void UpdateTextSlider(int current)
    {
        if (!Textslider.gameObject.activeSelf) Textslider.gameObject.SetActive(true);
        Textslider.text = current.ToString();
    }
}