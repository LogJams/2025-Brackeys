using UnityEngine;

public class MusicZone : MonoBehaviour {

    public MainGameMusic parent;

    public AudioClip music;



    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            parent.PlayMusic(music);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            parent.ExitMusicZone();
        }
    }

}
