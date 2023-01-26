using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class test : MonoBehaviour
{
    public void Killer()
    {
        Process.Start(Application.dataPath + @"\..\LittleHeroes.exe");
        Application.Quit();
    }
}