using UnityEngine;
using UIComponents;

public partial class UIManager
{
    [SerializeField] private UIChampionHUD[] championOverlays;
    
    public void InstantiateChampionHUD()
    {
        var champion = gsm.GetPlayerChampion();
        if (champion == null) return;
        
        var canvasChampion = Instantiate(championOverlays[0], transform);
        canvasChampion.InitHUD(champion);
    }
}