using UnityEngine;
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
    private void Start()
    {
        emotesPanel.SetActive(false);
        keybinbindPanel.SetActive(false);
        
        emotesButton.onClick.AddListener(() => emotesPanel.SetActive(true));
        keybindButton.onClick.AddListener(() => keybinbindPanel.SetActive(true));
        emotesExitButton.onClick.AddListener(() => emotesPanel.SetActive(false));
        if(keybinbindPanel != null) keybindExitButton.onClick.AddListener(() => keybinbindPanel.SetActive(false));
        settingsExitButton.onClick.AddListener(() => settingsPanel.SetActive(false));
        PaypalButton.onClick.AddListener(() => Application.OpenURL("https://paypal.me/null?country.x=FR&locale.x=fr_FR"));
    }

}
