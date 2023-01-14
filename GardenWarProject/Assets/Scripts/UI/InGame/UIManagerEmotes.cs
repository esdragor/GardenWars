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
    private GameObject EmotesPanel;

    public void InstantiateEmoteForEntity(Champion entity)
    {
        if (entity == null) return;
        var canvasEmotes = Instantiate(EmotesPanel, entity.uiTransform.position + entity.uiOffset + Vector3.up * 2f,
            Quaternion.identity, entity.uiTransform);
        entity.elementsToShow.Add(canvasEmotes);
        entity.emotesImage = canvasEmotes.transform.GetChild(0).GetComponent<RawImage>();
        entity.emotesImage.gameObject.SetActive(false);
        gsm.OnUpdate += () =>{ entity.emotesImage.transform.LookAt(Camera.main.transform); };
    }
}