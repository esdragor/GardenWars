using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Entities;
using Entities.Champion;
using GameStates;
using LogicUI.FancyTextRendering;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public static ChatManager Instance;

    [SerializeField] private MarkdownRenderer chatPanel;
    [SerializeField] private GameObject chatPanelCanvas;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Color allyColor = Color.cyan;
    [SerializeField] private Color enemyColor = Color.red;
    [SerializeField] private Button QuitButton;

    private Champion champion = null;
    private float delayTimer = 0f;

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
        QuitButton.onClick.AddListener(ToggleChat);
        
        GameStateMachine.Instance.OnUpdateFeedback += DecreaseTimer;
    }

    private void OnAddMessage(string message)
    {
        if(!champion) champion = GameStateMachine.Instance.GetPlayerChampion();
        
        if(message.Length <= 0) return;
        
        champion.OnAddMessage($"[|@@@@|{GameSettingsManager.playerName}|@@@|]: {message}\n", champion.entityIndex);
        inputField.text = "";
        
        InputManager.EnableInput();
    }
    
    private void DecreaseTimer()
    {
        delayTimer -= Time.deltaTime;
        if (delayTimer < 0f)
        {
            delayTimer = -1.0f;
        }
    }

    public void UpdateMessageBox(string message, int entityIndex)
    {
        if (message.Length == 0) return;
        
        //chatPanelCanvas.SetActive(true);
        //QuitButton.gameObject.SetActive(true);
        
        
        if (delayTimer < 0f && chatPanelCanvas.activeSelf)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/" + GameStateMachine.Instance.SFXChatMessage);
            delayTimer = 3.5f;
        }
        string text;
        if (entityIndex <= -1)
        {
            text = $"<color=#{ColorUtility.ToHtmlStringRGB(GameStateMachine.messageColor)}>{GameStateMachine.messageAuthor} : {message}</color>";

            chatPanel.Source += $"[{((int)GameStateMachine.gameTime/60):00}:{GameStateMachine.gameTime%60:00}] {text}";
            
            GoToBot();
            return;
        }
        
        if(!champion) champion = GameStateMachine.Instance.GetPlayerChampion();
        
        var code = !champion.GetEnemyTeams().Contains(EntityCollectionManager.GetEntityByIndex(entityIndex).team)
            ? ColorUtility.ToHtmlStringRGB(allyColor)
            : ColorUtility.ToHtmlStringRGB(enemyColor);

        text = message.Replace("[|@@@@|", $"<color=#{code}>");
        text = text.Replace("|@@@|]:", $" :</color>");
        
        chatPanel.Source += $"[{((int)GameStateMachine.gameTime/60):00}:{GameStateMachine.gameTime%60:00}] {text}";
        GoToBot();
    }

    private async void GoToBot()
    {
        await Task.Delay(20);
        scrollRect.verticalNormalizedPosition = 0;
    }

    public void ToggleChat()
    {
        var value = !chatPanelCanvas.activeSelf;
        chatPanelCanvas.SetActive(value);
        QuitButton.gameObject.SetActive(value);
    }
}