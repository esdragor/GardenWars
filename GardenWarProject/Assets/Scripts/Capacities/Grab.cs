using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour
{
    [SerializeField] private LineGenerate liner;
    [SerializeField] private ParticleSystem catchFX;
    
    public void StartGrab(Transform tr)
    {
        liner.startPoint = tr;
    }
    
    public void Catch()
    {
        catchFX.gameObject.SetActive(true);
    }
}