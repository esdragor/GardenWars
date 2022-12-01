using System.Collections;
using System.Collections.Generic;
using Entities;
using UnityEngine;

public class Minion : Entity
{
    protected override void OnStart()
    {
        
    }

    public override void OnInstantiated()
    {
        gameObject.SetActive(true);
    }
}
