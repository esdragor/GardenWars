using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettingsManager : MonoBehaviour
{
    private static GameSettingsManager instance;
    
    private void Awake()
    {
        if(!instance) instance = this;
        else Destroy(gameObject);
        Application.targetFrameRate = 60;
        DontDestroyOnLoad(gameObject);
    }
}
