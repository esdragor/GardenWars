using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject emotesPanel;
    [SerializeField] private GameObject keybinbindPanel;
    [SerializeField] private Button emotesButton;
    [SerializeField] private Button keybindButton;
    [SerializeField] private Button emotesExitButton;
    [SerializeField] private Button keybindExitButton;
    [SerializeField] private Button settingsExitButton;
    [SerializeField] private Button PaypalButton;
    void Start()
    {
        emotesButton.onClick.AddListener(() => emotesPanel.SetActive(true));
        keybindButton.onClick.AddListener(() => keybinbindPanel.SetActive(true));
        emotesExitButton.onClick.AddListener(() => emotesPanel.SetActive(false));
        keybindExitButton.onClick.AddListener(() => keybinbindPanel.SetActive(false));
        settingsExitButton.onClick.AddListener(() => settingsPanel.SetActive(false));
        PaypalButton.onClick.AddListener(() => Application.OpenURL("https://paypal.me/null?country.x=FR&locale.x=fr_FR"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
