using UnityEngine;

public partial class UIManager
{
    [Header("Settings")]
    [SerializeField] private GameObject[] settingsToHideInGame;

    public void HideSettings()
    {
        foreach (var go in settingsToHideInGame)
        {
            go.SetActive(false);
        }
    }
}