using UnityEngine;

public class SoundVolumeManager : MonoBehaviour {

    [Header("Check for music source, uncheck for SFX source")]
    public bool isMusic = true;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        if (isMusic) {
            PersistentData.instance.OnMusicVolume += OnUpdateVolume;
            GetComponent<AudioSource>().volume = PersistentData.instance.MusicVolume;
        } else {
            PersistentData.instance.OnSFXVolume += OnUpdateVolume;
            GetComponent<AudioSource>().volume = PersistentData.instance.SfxVolume;
        }
    }


    void OnUpdateVolume(System.Object src, float val) {
        GetComponent<AudioSource>().volume = val;
    }

}
