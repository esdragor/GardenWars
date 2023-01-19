using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RebindKeys : MonoBehaviour
{
    [SerializeField] private Button[] listOfButtons;

    private void Start()
    {
        foreach (var btn in listOfButtons)
        {
            string nameOfKey = btn.transform.GetChild(0).GetComponent<Text>().text;
        }
    }
}