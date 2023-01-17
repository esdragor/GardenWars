using UnityEngine;

public class GameSettingsManager : MonoBehaviour
{
    private static GameSettingsManager instance;

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
