using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkShotManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> sharkShot;

    public void EnableFXShot(Enums.Team team)
    {
        (team == Enums.Team.Team1 ? sharkShot[0] : sharkShot[1]).SetActive(true);
    }
}