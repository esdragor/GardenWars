using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEmoteFileExplorer : MonoBehaviour
{
    [SerializeField] private RawImage image;
    private EmotesManager manager => EmotesManager.instance;
    private byte index;

    public void Init(byte newIndex)
    {
        this.index = newIndex;
        image.texture = manager.EmotesTexture2Ds[index];
    }
}
