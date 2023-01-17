using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class EmotesManager : MonoBehaviour
{
    public static EmotesManager instance;

    [SerializeField] private InputField fieldEmote1;
    [SerializeField] private Texture2D[] defaultEmotes = new Texture2D[6];
    [SerializeField] private UIEmoteFileExplorer[] emoteFileExplorers = new UIEmoteFileExplorer[6];
    public Texture2D[] EmotesTexture2Ds => defaultEmotes;

    private void Awake()
    {
        if (instance == null && instance != this)
        {
            instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
            return;
        }
        
        DontDestroyOnLoad(this);
    }
    
    private void Start()
    {
        if(fieldEmote1 != null) AddListenerToField();
        
        LinkEmoteExplorers();
    }

    private void LinkEmoteExplorers()
    {
        for (byte i = 0; i < emoteFileExplorers.Length; i++)
        {
            emoteFileExplorers[i].Init(i);
            
            GameSettingsManager.SetEmoteTexture(i,defaultEmotes[i]);
        }
    }

    private void AddListenerToField()
    {
        fieldEmote1.onEndEdit.AddListener(delegate
        {
            //StartCoroutine(Load(0, fieldEmote1.text));
            Debug.Log(fieldEmote1.text);
            if (File.Exists(fieldEmote1.text))
            {
                byte[] fileData;
                fileData = File.ReadAllBytes(fieldEmote1.text);
                Debug.Log(fileData.Length);
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(fileData);
                defaultEmotes[0] = tex;
                Debug.Log(tex.EncodeToPNG().Length);
            }
            else
            {
                Debug.Log("File not found");
            }
        
        });
        fieldEmote1.text = $"C:/Users/badwolf/Desktop/try.jpg";
        Debug.Log(fieldEmote1.text);
        if (File.Exists(fieldEmote1.text))
        {
            byte[] fileData;
            fileData = File.ReadAllBytes(fieldEmote1.text);
            Debug.Log(fileData.Length);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
            defaultEmotes[0] = tex;
            Debug.Log(tex.EncodeToPNG().Length);
        }
        else
        {
            Debug.LogWarning("File not found");
        }
    }

    public Texture2D GetEmoteAtLocation(int index)
    {
        return defaultEmotes[index];
    }
}