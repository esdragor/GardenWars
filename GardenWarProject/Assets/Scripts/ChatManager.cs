using System;
using System.Collections;
using System.Collections.Generic;
using Entities;
using Entities.Champion;
using GameStates;
using TMPro;
using UnityEngine;

public class ChatManager : MonoBehaviour
{
    public static ChatManager Instance;

    [SerializeField] private TMP_Text chatPanel;
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

        champion.OnAddMessage($"[{GameSettingsManager.playerName}]: {message}", champion.entityIndex);
        inputField.text = "";
    }
    
    public void UpdateMessageBox(string message, int entityIndex)
    {
        if (message.Length == 0) return;
        if(!champion) champion = GameStateMachine.Instance.GetPlayerChampion();
        if (!champion.GetEnemyTeams().Contains(EntityCollectionManager.GetEntityByIndex(entityIndex).team)) 
            chatPanel.text += $"<color=blue>{message}</color>" + string.Format("\n");
        else
            chatPanel.text += $"<color=red>{message}</color>" + string.Format("\n");
    }
}