using System;
using Entities.Champion;
using UnityEngine;
using UnityEngine.UI;
using UIComponents;

public partial class UIManager
{
    [Header("Minimap")]
    [SerializeField] private RawImage miniMapRenderImage;
    [SerializeField] private RawImage miniMapCoverImage;
    [SerializeField] private Transform playerIconsParent;
    [SerializeField] private MinimapPlayerIcon playerIconPrefab;
    [SerializeField] private Camera minimapCamera;
    private Vector2 minimapSize;

    private void Start()
    {
        minimapSize = miniMapRenderImage.GetComponent<RectTransform>().sizeDelta;
    }


    public void SetupMinimap()
    {
        
    }

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
    
    
}
