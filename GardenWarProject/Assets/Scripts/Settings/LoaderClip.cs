using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class LoaderClip : MonoBehaviour
{
    public static string renderEmote1;
    
    [SerializeField] private InputField fieldEmote1;
    [SerializeField] private VideoPlayer test;
    
    // Start is called before the first frame update
    void Start()
    {
        fieldEmote1.onEndEdit.AddListener(delegate {test.url = fieldEmote1.text;});
    }
}
