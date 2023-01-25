using Entities.Champion;
using GameStates;
using UIComponents;
using UnityEngine;
using UnityEngine.UI;

public partial class UIManager
{
    [Header("Emotes Elements")]
    [SerializeField] private UIEmoteWheel emoteWheel;

    public byte emoteIndex { get; private set; }

    [SerializeField] private GameObject emotesPanelPrefab;

    [SerializeField] private Transform emotesParent;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, 0);

    public void ShowWheel(Vector2 position)
    {
        emoteWheel.Show(position);
        emoteIndex = 6;
    }

    public void SetupEmoteWheel()
    {
        emoteWheel.InitWheel();
    }

    public void HideWheel()
    {
        emoteWheel.Hide();
    }

    public void SetEmoteIndex(byte indexOfEmote)
    {
        emoteIndex = indexOfEmote;
    }
    
    public void InstantiateEmoteForEntity(Champion entity)
    {
        if (entity == null) return;
        var panel = Instantiate(emotesPanelPrefab, entity.uiTransform.position + entity.uiOffset + Vector3.up * 2f,
            Quaternion.identity, emotesParent);
        entity.emotesImage = panel.GetComponent<RawImage>();
        panel.SetActive(false);
        var entityTr = entity.transform;
        var panelTransform = panel.transform;
        
        gsm.OnUpdateFeedback += UpdateEmotesPosition;
        
        void UpdateEmotesPosition()
        {
            if(panelTransform == null || entityTr == null) return;
            panelTransform.position = GameStateMachine.mainCam.WorldToScreenPoint(entityTr.position) + offset;
        }
    }


}