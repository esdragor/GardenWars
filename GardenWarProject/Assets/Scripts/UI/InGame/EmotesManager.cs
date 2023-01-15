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
    private Texture2D[] emotes = new Texture2D[6];

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        
        DontDestroyOnLoad(this);
    }

    // IEnumerator Load(int index, string url)
    // {
    //
    //     
    //     using (WWW www = new WWW(url))
    //     {
    //         yield return www;
    //         Texture2D tex = new Texture2D(100, 100, TextureFormat.DXT1, false);
    //         www.LoadImageIntoTexture(tex);
    //         emotes[index] = tex;
    //     }
    // }
    
    void Start()
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
                emotes[0] = tex;
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
            emotes[0] = tex;
            Debug.Log(tex.EncodeToPNG().Length);
        }
        else
        {
            Debug.LogError("File not found");
        }
    }

    public Texture2D[] GetEmotes()
    {
        return emotes;
    }
    
    public Texture2D GetEmoteAtLocation(int index)
    {
        return emotes[index];
    }
}