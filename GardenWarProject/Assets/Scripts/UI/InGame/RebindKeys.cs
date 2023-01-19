using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RebindKeys : MonoBehaviour
{
    [SerializeField] private Button Button;
    [SerializeField] private string nameOfKey;
    [SerializeField] private Text textOfButton;
    [SerializeField] private GameObject defaultKey;
    [SerializeField] private GameObject WaitingForKey;
    [SerializeField] private PlayerController pc;

    private void Start()
    {
        Button.onClick.AddListener(StartRebinding);
    }

    public void StartRebinding()
    {
        Button.gameObject.SetActive(false);
        WaitingForKey.gameObject.SetActive(false);
        pc.PlayerInput.SwitchCurrentActionMap("UI");
    }
}