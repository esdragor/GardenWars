using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettingsManager : MonoBehaviour
{
    private static GameSettingsManager instance;

    public string playerName { get; private set; }
    public byte[][] emotes { get; private set; }
    
    private void Awake()
    {
        if(!instance) instance = this;
        else Destroy(gameObject);
        Application.targetFrameRate = 60;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("playerName")) playerName = PlayerPrefs.GetString("playerName");
        emotes = new byte[6][];
    }

    public static void SetPlayerName(string newName)
    {
        instance.playerName = newName;
        PlayerPrefs.SetString("playerName",newName);
    }

    public static void SetEmoteTexture(byte index, Texture2D tex)
    {
        if(index >= 6) return;
        instance.emotes[index] = tex.GetRawTextureData();
        Debug.Log($"Set textures for index {index} ({instance.emotes[index].Length})");
    }
    
    
}
