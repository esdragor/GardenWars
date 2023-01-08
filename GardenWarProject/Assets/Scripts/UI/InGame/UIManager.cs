using GameStates;
using Photon.Pun;
using TMPro;
using UnityEngine;

public partial class UIManager : MonoBehaviour
{
    public GameStateMachine gsm => GameStateMachine.Instance;
    private double currentTime => GameStateMachine.isOffline ? Time.timeAsDouble : PhotonNetwork.Time;
    public static UIManager Instance;

    [Header("Top Bar")]
    [SerializeField] private TextMeshProUGUI scoreTextBlue;
    [SerializeField] private TextMeshProUGUI scoreTextRed;
    [SerializeField] private TextMeshProUGUI timerText;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }

        Instance = this;
    }

    public void SetupTopBar()
    {
        UpdateScore(0);
        UpdateTimer();
        
        gsm.OnTeamIncreaseScoreFeedBack += UpdateScore;
        gsm.OnTickFeedback += UpdateTimer;
    }

    private void UpdateScore(byte _)
    {
        scoreTextBlue.text = $"<color=blue>{gsm.GetTeamScore(Enums.Team.Team1)}</color>";
        scoreTextRed.text = $"<color=red>{gsm.GetTeamScore(Enums.Team.Team2)}</color>";
    }

    private void UpdateTimer()
    {
        var time = currentTime - gsm.startTime;
        timerText.text = $"{((int)time/60):00}:{time%60:00}";
    }


}