using System.Collections;
using System.Collections.Generic;
using GameStates;
using UnityEngine;

public class SharkShotManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> sharkShot;

    public void EnableFXShot(Enums.Team team)
    {
        (team == GameStateMachine.Instance.GetPlayerTeam() ? sharkShot[0] : sharkShot[1]).SetActive(true);
    }
}