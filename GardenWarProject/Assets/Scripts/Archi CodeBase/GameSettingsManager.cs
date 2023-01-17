using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettingsManager : MonoBehaviour
{
    private static GameSettingsManager instance;

    private string pName;
    private byte[][] emotes;

    public static string playerName => instance.pName;
    public static byte[][] emoteBytes => instance.emotes;
    
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
        emotes = new byte[6][];
    }

    public static void SetPlayerName(string newName)
    {
        instance.pName = newName;
        PlayerPrefs.SetString("playerName",instance.pName );
        Debug.Log($"Player name set to : {instance.pName }");
    }

    public static void SetEmoteTexture(byte index, Texture2D tex)
    {
        if(index >= 6) return;
        instance.emotes[index] = tex.GetRawTextureData();
    }
    
    
}
