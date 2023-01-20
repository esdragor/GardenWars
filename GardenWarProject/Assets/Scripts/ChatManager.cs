using System;
using System.Collections;
using System.Collections.Generic;
using GameStates;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChatManager : MonoBehaviour
{
    public static ChatManager Instance;

    [SerializeField] private TMP_Text chatPanel;
    [SerializeField] private TMP_InputField inputField;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void OnStart()
    {
        inputField.onEndEdit.AddListener(delegate { OnAddMessage(); });
    }

    public void OnAddMessage()
    {
        Debug.Log("Chat Message: " + inputField.text);
        GameStateMachine.Instance.GetPlayerChampion().OnAddMessage($"[{GameSettingsManager.playerName}]: {inputField.text}" + string.Format("\n"));
        inputField.text = "";
    }
    
    public void UpdateMessageBox(string message)
    {
        chatPanel.text += message;
    }
}