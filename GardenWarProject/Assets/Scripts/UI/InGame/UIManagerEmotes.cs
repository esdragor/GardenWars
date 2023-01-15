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
    private GameObject emotesPanel;

    [SerializeField] private Transform emotesParent;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, 0);

private Transform entity;
    public void InstantiateEmoteForEntity(Champion entity)
    {
        cam = Camera.main;
        if (entity == null) return;
        var canvasEmotes = Instantiate(emotesPanel, entity.uiTransform.position + entity.uiOffset + Vector3.up * 2f,
            Quaternion.identity, emotesParent);
        entity.elementsToShow.Add(canvasEmotes);
        entity.emotesImage = canvasEmotes.transform.GetChild(0).GetComponent<RawImage>();
        entity.emotesImage.gameObject.SetActive(false);
        this.entity = entity.transform;
        gsm.OnUpdateFeedback += UpdateEmotesPosition;
    }

    private void UpdateEmotesPosition()
    {
        emotesPanel.transform.position = cam.WorldToScreenPoint(entity.position) + offset;
    }
}