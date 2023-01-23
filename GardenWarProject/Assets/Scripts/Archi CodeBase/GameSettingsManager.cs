using UnityEngine;

public class GameSettingsManager : MonoBehaviour
{
    private static GameSettingsManager instance;

    [Header("Emotes")]
    [SerializeField] private bool ignoreEmotes;
    public static bool IgnoreEmotes => instance.ignoreEmotes;
    [SerializeField] private int maxImageSize = 512;
    public static int maxFileSize => instance.maxImageSize*instance.maxImageSize*4;

    [Header("Debug")]
    [SerializeField] private bool debugMode;
    [SerializeField] private int startingSweets;
    [SerializeField] private int startingUpgrades;
    public static bool isDebug => instance.debugMode;
    public static int candy => instance.startingSweets;
    public static int upgrades => instance.startingUpgrades;
    
    
    private string pName;
    private byte[][] emoteBytes;

    public static string playerName => instance.pName;
    public static byte[][] bytes => instance.emoteBytes;

    private void Awake()
    {
        if(!instance) instance = this;
        else Destroy(gameObject);
        Application.targetFrameRate = 60;
        DontDestroyOnLoad(gameObject);
        InputManager.PlayerMap ??= new PlayerInputs();
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("playerName"))
        {
            pName = PlayerPrefs.GetString("playerName");
        }
        else
        {
            SetPlayerName("Player");
        }
        
        emoteBytes = new byte[6][];
    }

    public static void SetPlayerName(string newName)
    {
        instance.pName = newName;
        PlayerPrefs.SetString("playerName",instance.pName );
        Debug.Log($"Player name set to : {instance.pName }");
    }

    public static void SetEmoteTexture(byte index,byte[] bytes)
    {
        if(index >= 6) return;
        instance.emoteBytes[index] = bytes;
    }


}
