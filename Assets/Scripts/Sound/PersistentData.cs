using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class PersistentData : MonoBehaviour {

    public static PersistentData instance;

    [Range(0, 1)]
    public float MusicVolume;
    [Range(0, 1)]
    public float SfxVolume;

    public event System.EventHandler<float> OnMusicVolume;
    public event System.EventHandler<float> OnSFXVolume;

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }


    public void SetMusic(float val) {
        MusicVolume = Mathf.Clamp(val, 0, 1);
        OnMusicVolume?.Invoke(this, MusicVolume);
    }

    public void SetSFX(float val) {
        SfxVolume = Mathf.Clamp(val, 0, 1);
        OnSFXVolume?.Invoke(this, SfxVolume);
    }

}
