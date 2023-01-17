using Entities.Champion;
using UIComponents;
using UnityEngine;
using UnityEngine.UI;

public partial class UIManager
{
    [Header("Emotes Elements")]
    [SerializeField]
    private UIEmoteWheel emoteWheelGo;

    public byte emoteIndex { get; private set; }

    private GameObject emotesPanelPrefab;

    [SerializeField] private Transform emotesParent;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, 0);

    public void ShowWheel(Vector2 position)
    {
        emoteWheelGo.Show(position);
        emoteIndex = 6;
    }

    public void SetupEmoteWheel()
    {
        emoteWheelGo.InitWheel();
    }

    public void HideWheel()
    {
        emoteWheelGo.Hide();
    }

    public void SetEmoteIndex(byte indexOfEmote)
    {
        Debug.Log($"Index Set to {indexOfEmote}");
        emoteIndex = indexOfEmote;
    }
    
    
    

    public void InstantiateEmoteForEntity(Champion entity)
    {
        return;
        cam = Camera.main;
        if (entity == null) return;
        var panel = Instantiate(emotesPanelPrefab, entity.uiTransform.position + entity.uiOffset + Vector3.up * 2f,
            Quaternion.identity, emotesParent);
        entity.emotesImage = panel.GetComponent<RawImage>();
        Debug.Log(entity.name);
        panel.SetActive(false);
        var entityTr = entity.transform;
        var panelTransform = panel.transform;
        gsm.OnUpdateFeedback += UpdateEmotesPosition;
        
        void UpdateEmotesPosition()
        {
            panelTransform.position = cam.WorldToScreenPoint(entityTr.position) + offset;
        }
    }


}