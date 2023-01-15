using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class LoaderClip : MonoBehaviour
{

    [SerializeField] private InputField fieldEmote1;
    private Texture2D[] emotes = new Texture2D[6];
    
    // Start is called before the first frame update
    void Start()
    {
        fieldEmote1.onEndEdit.AddListener(delegate
        {
            Texture2D tex = new Texture2D(100, 100, TextureFormat.RGB48, false);
            using (WWW www = new WWW(fieldEmote1.text))
            {
                www.LoadImageIntoTexture(tex);
                emotes[0] = tex;
            }
        });
    }
    
    public Texture2D[] GetEmotes()
    {
        return emotes;
    }
}
