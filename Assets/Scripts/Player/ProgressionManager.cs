using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public struct Announcement {
    public string message;
}

public class ProgressionManager : MonoBehaviour {

    public static ProgressionManager instance;

    public Vitality player;
    public Transform spawnPoint;

    public event System.EventHandler<Announcement> OnAnnouncement;
    public event System.EventHandler EndAnnouncement;
    bool announcing = false;


    [Range(1, 20)]
    public float timeScale = 10f;

    private uint numDeaths = 0;

    private int checkpoint = 0;

    [Header("Store all the announcements here.....")]
    public Announcement firstDeath;
    public Weapon basicWeapon;
    public GameObject redBarrel;
    public Announcement secondDeath;



    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        player.OnDeath += OnPlayerDeath;
    }


    private void Update() {
        if (announcing && Input.GetKeyUp(KeyCode.Space)) {
            EndAnnouncement?.Invoke(this, System.EventArgs.Empty);
            announcing = false;
            if (checkpoint == 2) {
                //load Level 2
                UnityEngine.SceneManagement.SceneManager.LoadScene(1);
            }
        }   
    }


    void OnPlayerDeath(System.Object source, System.EventArgs e) {
        float dist = (player.transform.position - spawnPoint.position).magnitude;
        numDeaths++;

        if (numDeaths == 1) {
            Announce(firstDeath);
            UnlockTracker.instance.UnlockWeapon(basicWeapon);
            StartCoroutine(MoveToSpawn(dist / timeScale));
            checkpoint = 1;
        }
        if (checkpoint == 1 && redBarrel == null) {
            //play a death animation
            Announce(secondDeath);
            //load the next scene
            checkpoint = 2;
            player.gameObject.SetActive(false);
        }
    }

    void Announce(Announcement anc) {
        announcing = true;
        OnAnnouncement?.Invoke(this, anc);
    }


    void SetPlayerStatus(bool enabled) {
        player.GetComponent<PlayerMovement>().enabled = enabled;
        player.GetComponent<PlayerInteractions>().enabled = enabled;
        player.enabled = enabled;
    }

    IEnumerator MoveToSpawn(float dt) {
        SetPlayerStatus(false);
        Vector3 p0 = player.transform.position;
        float t0 = Time.time;
        while (Time.time - t0 < dt) {
            player.transform.position = Vector3.Lerp(p0, spawnPoint.position, (Time.time - t0) / dt);
            yield return null;
        }
        player.transform.position = spawnPoint.position;
        player.transform.rotation = spawnPoint.rotation;
        SetPlayerStatus(true);
        player.Reset();
        yield return null;
    }

}
