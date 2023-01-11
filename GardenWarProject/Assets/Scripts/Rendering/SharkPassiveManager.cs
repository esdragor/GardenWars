using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkPassiveManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> sharkSPassive;
    [SerializeField] private GameObject sharkHit;

    public void EnableFXShot(Enums.Team team)
    {
        (team == Enums.Team.Team1 ? sharkSPassive[0] : sharkSPassive[1]).SetActive(true);
    }

    public void HitFx()
    {
        sharkHit.SetActive(false);
        sharkHit.SetActive(true);
    }
}
