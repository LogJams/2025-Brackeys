using System.Collections.Generic;
using UnityEngine;

public class MainGameMusic : MonoBehaviour {

    public static MainGameMusic instance;

    public AudioSource musicSource;

    public AudioClip defaultClip;

    public AudioClip crateSmash;


    public List<Vitality> crates;

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        }
        else {
            instance = this;
        }
    }

    private void Start() {
        foreach (Vitality vitality in crates) {
            vitality.OnDeath += CRATESMASH;
        }
    }

    public void CRATESMASH(System.Object src, System.EventArgs e) {
        musicSource.PlayOneShot(crateSmash);
    }

    public void PlayMusic(AudioClip music) {
        musicSource.clip = music;
        musicSource.Play();
    }

    public void ExitMusicZone() {
        musicSource.clip = defaultClip;
        musicSource.Play();
    }

}
