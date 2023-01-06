using UnityEngine;
using UIComponents;

public partial class UIManager
{
    [SerializeField] private UIChampionHUD[] championOverlays;
    
    public void InstantiateChampionHUD()
    {
        var champion = gsm.GetPlayerChampion();
        if (champion == null) return;
        
        var canvasIndex = champion.currentSo.canvasIndex;
        if (canvasIndex >= championOverlays.Length) canvasIndex = 0;
        
        var canvasChampion = Instantiate(championOverlays[canvasIndex], transform);
        canvasChampion.InitHUD(champion);
    }
}