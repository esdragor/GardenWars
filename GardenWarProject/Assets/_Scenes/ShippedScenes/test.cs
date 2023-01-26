using UnityEngine;
using System.Diagnostics;

public class test : MonoBehaviour
{
    public void Killer()
    {
        if (Application.isEditor) return;
        Process.Start(Application.dataPath + @"\..\LittleHeroes.exe");
        Application.Quit();
    }
}