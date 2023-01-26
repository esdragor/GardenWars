using System;
using GameStates;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PostGameUIManager : MonoBehaviour
{
    public static PostGameUIManager Instance;

    [SerializeField] private GameObject postGameCanvas;
    [SerializeField] private TextMeshProUGUI winningTeamText;
    [SerializeField] private TextMeshProUGUI resultText;

    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    
    [SerializeField] private Button rematchButton;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }

        Instance = this;
        
    }

    private void Start()
    {
        rematchButton.onClick.AddListener(Application.Quit);
        postGameCanvas.SetActive(false);
        winPanel.SetActive(false);
        losePanel.SetActive(!false);
    }

    public void DisplayPostGame(Enums.Team winner)
    {
        postGameCanvas.SetActive(true);
        winningTeamText.text = $"{winner} has won!";

        // var playerTeam = GameStateMachine.Instance.GetPlayerTeam();
        // resultText.text = playerTeam == winner ? "You won!" : "You lost!";

        var win = GameStateMachine.Instance.GetPlayerTeam() == winner;
        
        winPanel.SetActive(win);
        losePanel.SetActive(!win);
    }

    public void OnRematchClick()
    {
        Debug.Log("Does not work yet");
    }
}
