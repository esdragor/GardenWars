using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public static PlayerInputs PlayerMap;
    
    public static PlayerInputs PlayerUIMap;
    
    
    private static Dictionary<string, string> allKeys = new Dictionary<string, string>();

    private void Awake()
    {
        if (PlayerMap != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        PlayerMap ??= new PlayerInputs();

    }
    
    public static void DisableInput()
    {
        PlayerMap.Disable();
    }
    
    public static void EnableInput()
    {
        PlayerMap.Enable();
    }

    /// <summary>
    /// Toggle PlayerMap
    /// </summary>
    /// <param name="value">State : true or false</param>
    public static void EnablePlayerMap(bool value)
    {
        if (PlayerMap == null) return;
        if(value) PlayerMap.Enable();
        else PlayerMap.Disable();
    }
    
    /// <summary>
    /// Toggle PlayerUIMap
    /// </summary>
    /// <param name="value">State : true or false</param>
    public static void EnablePlayerUIMap(bool value)
    {
        if (PlayerUIMap == null) return;
        if(value) PlayerUIMap.Enable();
        else PlayerUIMap.Disable();
    }

    public static event Action rebindComplete;
    public static event Action rebindCanceled;
    public static event Action<InputAction, int> rebindStarted;

    private static void RefreshDataDict()
    {
        allKeys.Clear();
        foreach (InputBinding bind in PlayerMap.bindings)
        {
            if (bind.effectivePath.Contains("Mouse")) continue;
            allKeys.Add(bind.effectivePath, bind.action);
        }
    }
    
    public static void StartRebind(string actionName, int bindingIndex, Text statusText, bool excludeMouse)
    {
        if (allKeys.Count == 0)
        {
            RefreshDataDict();
        }
        
        InputAction action = PlayerMap.asset.FindAction(actionName);
        if (action == null || action.bindings.Count <= bindingIndex)
        {
            Debug.Log("Couldn't find action or binding");
            return;
        }
        if (action.bindings[bindingIndex].isComposite)
        {
            var firstPartIndex = bindingIndex + 1;
            if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isComposite)
                DoRebind(action, bindingIndex, statusText, true, excludeMouse);
        }
        else
            DoRebind(action, bindingIndex, statusText, false, excludeMouse);
    }

    private static void DoRebind(InputAction actionToRebind, int bindingIndex, Text statusText, bool allCompositeParts, bool excludeMouse)
    {
        if (actionToRebind == null || bindingIndex < 0)
            return;

        statusText.text = $"Press a {actionToRebind.expectedControlType}";

        actionToRebind.Disable();
        var rebind = actionToRebind.PerformInteractiveRebinding(bindingIndex);

        rebind.OnComplete(operation =>
        {
            Debug.Log(actionToRebind.bindings[bindingIndex].action);
            if (allKeys.ContainsKey(actionToRebind.bindings[bindingIndex].effectivePath) ||actionToRebind.bindings[bindingIndex].effectivePath.Contains("Mouse"))
            {
                Debug.Log("ERROR");
                actionToRebind.RemoveBindingOverride(bindingIndex);
                return;
            }
            actionToRebind.Enable();
            operation.Dispose();
            if(allCompositeParts)
            {
                var nextBindingIndex = bindingIndex + 1;
                if (nextBindingIndex < actionToRebind.bindings.Count && actionToRebind.bindings[nextBindingIndex].isComposite)
                    DoRebind(actionToRebind, nextBindingIndex, statusText, allCompositeParts, excludeMouse);
            }
            RefreshDataDict();
            SaveBindingOverride(actionToRebind);
            rebindComplete?.Invoke();
        });

        rebind.OnCancel(operation =>
        {
            Debug.Log("Cancel");
            actionToRebind.Enable();
            operation.Dispose();

            rebindCanceled?.Invoke();
        });

        rebind.WithCancelingThrough("<Keyboard>/escape");

        if (excludeMouse)
            rebind.WithControlsExcluding("Mouse");

        rebindStarted?.Invoke(actionToRebind, bindingIndex);
        rebind.Start(); //actually starts the rebinding process
    }

    public static string GetBindingName(string actionName, int bindingIndex)
    {
        if (PlayerMap == null)
            PlayerMap = new PlayerInputs();

        InputAction action = PlayerMap.asset.FindAction(actionName);
        return action.GetBindingDisplayString(bindingIndex);
    }

    private static void SaveBindingOverride(InputAction action)
    {
        for (int i = 0; i < action.bindings.Count; i++)
        {
            PlayerPrefs.SetString(action.actionMap + action.name + i, action.bindings[i].overridePath);
        }
    }

    public static void LoadBindingOverride(string actionName)
    {
        PlayerMap ??= new PlayerInputs();

        var action = PlayerMap.asset.FindAction(actionName);

        for (int i = 0; i < action.bindings.Count; i++)
        {
            if (!string.IsNullOrEmpty(PlayerPrefs.GetString(action.actionMap + action.name + i)))
                action.ApplyBindingOverride(i, PlayerPrefs.GetString(action.actionMap + action.name + i));
        }
    }

    public static void ResetBinding(string actionName, int bindingIndex)
    {
        var action = PlayerMap.asset.FindAction(actionName);

        if(action == null || action.bindings.Count <= bindingIndex)
        {
            Debug.Log("Could not find action or binding");
            return;
        }

        if (action.bindings[bindingIndex].isComposite)
        {
            for (int i = bindingIndex; i < action.bindings.Count && action.bindings[i].isComposite; i++)
                action.RemoveBindingOverride(i);
        }
        else
            action.RemoveBindingOverride(bindingIndex);

        SaveBindingOverride(action);
    }
}
