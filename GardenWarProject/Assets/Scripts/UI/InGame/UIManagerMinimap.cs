using System;
using Entities.Champion;
using UnityEngine;
using UnityEngine.UI;
using UIComponents;

public partial class UIManager
{
    [Header("Minimap")]
    [SerializeField] private RawImage miniMapRenderImage;
    private RectTransform minimapRenderTransform => miniMapRenderImage.rectTransform;
    [SerializeField] private RawImage miniMapCoverImage;
    [SerializeField] private Transform playerIconsParent;
    [SerializeField] private MinimapPlayerIcon playerIconPrefab;
    [SerializeField] private Camera minimapCamera;
    private Vector2 minimapSize;
    
    public void InitPlayerIcon(Champion champion)
    {
        var icon = Instantiate(playerIconPrefab, playerIconsParent);
        icon.LinkToChampion(champion);
        
        icon.ShowIcon(champion.team == gsm.GetPlayerTeam());

        gsm.OnUpdateFeedback += UpdateIconPosition;

        champion.OnHideElementFeedback += () => icon.ShowIcon(false);
        champion.OnShowElementFeedback += () => icon.ShowIcon(true);
        
        void UpdateIconPosition()
        {
            icon.rectTransform.localPosition = (Vector2)minimapCamera.WorldToScreenPoint(icon.associatedChampion.position) - minimapSize/2;
        }
    }

    public RectTransform GetMinimapRect()
    {
        Debug.Log("Getting minimap render");
        return minimapRenderTransform;
    }

    public Ray MiniMapCamRay(Vector2 mousePos)
    {
        var sizeDelta = minimapRenderTransform.sizeDelta;
        return minimapCamera.ViewportPointToRay((mousePos - (Vector2)minimapRenderTransform.position + sizeDelta * 0.5f)/sizeDelta);
    }
    
    
}
