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
    [Header("List of Spawn Points for Progression")]
    public Transform bossSpawn;
    public Transform centralSpawn;
    public Transform loganSpawn;
    public Transform adiSpawn;

    bool currentlyDying = false;

    private int current_room = 0;


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
    public Announcement SecondDeathP2;



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
                Announce(SecondDeathP2);
                checkpoint = 3;
                announcing = true;
            }
        }   
    }


    void OnPlayerDeath(System.Object source, System.EventArgs e) {
        if (currentlyDying) return;
        numDeaths++;

        if (numDeaths == 1) {
            float dist = (player.transform.position - bossSpawn.position).magnitude;
            Announce(firstDeath);
            UnlockTracker.instance.UnlockWeapon(basicWeapon);
            StartCoroutine(MoveToSpawn(dist / timeScale, bossSpawn));
            checkpoint = 1;
        }
        else if (checkpoint == 1 && redBarrel == null) {
            //play a death animation
            Announce(secondDeath);
            //load the next scene
            checkpoint = 2;
        }
        else {
            //generic revive based on where we were last
            Transform destination = centralSpawn;
            if (current_room == 0) {
                //central spawn
            }
            if (current_room == 1) {
                destination = loganSpawn;
            }
            if (current_room == 2) {
                destination = adiSpawn;
            }
            if (current_room == 3) {
                destination = bossSpawn;
            }

            float dist = (player.transform.position - destination.position).magnitude;
            StartCoroutine(MoveToSpawn(dist / timeScale, destination));

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

    IEnumerator MoveToSpawn(float dt, Transform spawn) {
        SetPlayerStatus(false);
        currentlyDying = true;
        Vector3 p0 = player.transform.position;
        Quaternion q0 = player.transform.rotation;
        float t0 = Time.time;
        while (Time.time - t0 < dt) {
            player.transform.position = Vector3.Lerp(p0, spawn.position, (Time.time - t0) / dt);
            player.transform.rotation = Quaternion.Lerp(q0, spawn.rotation, (Time.time - t0) / dt);

            yield return null;
        }
        player.transform.position = spawn.position;
        player.transform.rotation = spawn.rotation;
        SetPlayerStatus(true);
        player.Reset();
        currentlyDying = false;
        yield return null;
    }

}
