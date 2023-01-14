using FMOD.Studio;
using UnityEngine;
using UnityEngine.UI;

public class SoundVolumeManager : MonoBehaviour
{
    [SerializeField] private Slider sliderMaster;
    [SerializeField] private Slider sliderFX;
    [SerializeField] private Slider sliderAmbiant;
    private void Start()
    {
        SetupPlayerPref("Master", sliderMaster);
        SetupPlayerPref("FX", sliderFX);
        SetupPlayerPref("Ambiant", sliderAmbiant);
    }

    private void SetupPlayerPref(string prefName,Slider slider)
    {
        var bank = FMODUnity.RuntimeManager.GetVCA($"vca:/{prefName}");
        
        slider.onValueChanged.AddListener(UpdateVolume);
        
        if (PlayerPrefs.HasKey(prefName))
        {
            slider.value = PlayerPrefs.GetFloat(prefName);
            UpdateVolume(slider.value);
            return;
        }
        
        PlayerPrefs.SetFloat(prefName, 1f);
        slider.value = PlayerPrefs.GetFloat(prefName);
        UpdateVolume(slider.value);
        
        void UpdateVolume(float value)
        {
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName(prefName, value);
            bank.setVolume(value);
            PlayerPrefs.SetFloat(prefName, value);
        }
    }

    
}