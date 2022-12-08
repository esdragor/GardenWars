using System;
using UnityEngine;
     
namespace DebugStuff
{
    public class BuildConsole : MonoBehaviour
    {
        private bool show;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P)) show = !show;
        }

#if !UNITY_EDITOR
        static string myLog = "";
        private string output;
        private string stack;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void OnEnable()
        {
            Application.logMessageReceived += Log;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= Log;
        }
     
        public void Log(string logString, string stackTrace, LogType type)
        {
            output = logString;
            stack = stackTrace;
            myLog = output + "\n" + myLog;
            if (myLog.Length > 5000)
            {
                myLog = myLog.Substring(0, 4000);
            }
        }

        private void OnGUI()
        {
            if(!show) return;
            myLog =
 GUI.TextArea(new Rect(10, Screen.height * 0.75f - 10, Screen.width * 0.40f, Screen.height/4f), myLog);
        }
#endif
    }
}