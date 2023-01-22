using System;
using System.Collections;
using System.Collections.Generic;
using Entities;
using Entities.Champion;
using GameStates;
using LogicUI.FancyTextRendering;
using TMPro;
using UnityEngine;

public class ChatManager : MonoBehaviour
{
    public static ChatManager Instance;

    [SerializeField] private MarkdownRenderer chatPanel;
    [SerializeField] private TMP_InputField inputField;

    private Champion champion = null;
    private bool isChatting = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        inputField.onEndEdit.AddListener(OnAddMessage);
        inputField.onSelect.AddListener((string s) => { InputManager.DisableInput(); });
        inputField.onDeselect.AddListener((string s) => { InputManager.EnableInput(); });
    }

    private void OnAddMessage(string message)
    {
        if(!champion) champion = GameStateMachine.Instance.GetPlayerChampion();
        
        if(message.Length <= 0) return;
        
        champion.OnAddMessage($"[{GameSettingsManager.playerName}]: {message}", champion.entityIndex);
        inputField.text = "";
    }
    
    public void UpdateMessageBox(string message, int entityIndex)
    {
        if (message.Length == 0) return;
        if(!champion) champion = GameStateMachine.Instance.GetPlayerChampion();
        if (!champion.GetEnemyTeams().Contains(EntityCollectionManager.GetEntityByIndex(entityIndex).team)) 
            chatPanel.Source += $"<color=blue>{message}</color>" + string.Format("\n");
        else
            chatPanel.Source += $"<color=red>{message}</color>" + string.Format("\n");
    }
}