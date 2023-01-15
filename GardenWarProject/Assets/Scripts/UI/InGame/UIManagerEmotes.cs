using System.Collections;
using System.Collections.Generic;
using Entities;
using Entities.Champion;
using UIComponents;
using UnityEngine;
using UnityEngine.UI;

public partial class UIManager
{
    [Header("Emotes Elements")] [SerializeField]
    private GameObject emotesPanelPrefab;

    [SerializeField] private Transform emotesParent;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, 0);
    

    public void InstantiateEmoteForEntity(Champion entity)
    {
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