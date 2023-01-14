using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundVolumeManager : MonoBehaviour
{
    [SerializeField] private Slider sliderMaster;
    
    private FMOD.Studio.VCA masterBank;

    private void OnEnable()
    {
        masterBank = FMODUnity.RuntimeManager.GetVCA("vca:/Master");
        UpdateVolume("Master");
    }
    
    public void UpdateVolume(string parameterName)
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName(parameterName, sliderMaster.value);
        masterBank.setVolume(sliderMaster.value);
    }
}