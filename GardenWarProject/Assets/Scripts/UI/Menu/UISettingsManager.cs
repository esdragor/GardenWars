using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISettingsManager : MonoBehaviour
{
    [SerializeField] private GameObject panelGo;

    [SerializeField] private Button closeButton;
    
    private void Start()
    {
        panelGo.SetActive(false);
        
        closeButton.onClick.AddListener(() =>
        {
            panelGo.SetActive(false);
        });
    }

    
}
