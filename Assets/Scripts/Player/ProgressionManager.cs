using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public struct Announcement {
    public string message;
}

public class ProgressionManager : MonoBehaviour {

    public event System.EventHandler PlayerEntersBossArea;
    public event System.EventHandler PlayerLeavesBossArea;


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
    public GameObject doorToDelete;


    [Range(1, 20)]
    public float timeScale = 10f;

    private uint numDeaths = 0;

    private int checkpoint = 0;

    [Header("Store all the announcements here.....")]
    public Announcement gameStart;
    public Announcement firstDeath;
    public Announcement firstDeathAgain;
    public Weapon basicWeapon;
    public Announcement secondDeath;
    public Announcement SecondDeathP2;
    public Armor initialArmor;
    public Armor basicArmor;

    public Announcement diedWithConfidence;
    public Weapon confidenceBlade;

    public Announcement diedWithRed;
    public Armor redArmor;
    

    public Vitality bossHp;
    public Announcement BossDead;
    bool gameOver = false;


    bool playerInBossArea = false;

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
        bossHp.OnDeath += OnBossDead;
        Announce(gameStart);
    }


    public void OnBossDead(System.Object source, System.EventArgs e) {
        Announce(BossDead);
        gameOver = true;
    }

    private void Update() {
        if (currentlyDying || gameOver) return;

        if (announcing && Input.anyKeyDown) {
            EndAnnouncement?.Invoke(this, System.EventArgs.Empty);
            announcing = false;
            //follow-up to the first announcement
            if (checkpoint == 2) {
                Announce(SecondDeathP2);
                checkpoint = 3;
                announcing = true;
                UnlockTracker.instance.UnlockArmor(basicArmor);
                UnlockTracker.instance.LockArmor(initialArmor);
                Destroy(doorToDelete);
            }
        }   
    }


    void OnPlayerDeath(System.Object source, System.EventArgs e) {
        if (currentlyDying) return;
        numDeaths++;


  

        float dist = 0;
        if (numDeaths == 1) {
            dist = (player.transform.position - bossSpawn.position).magnitude;
            Announce(firstDeath);
            UnlockTracker.instance.UnlockWeapon(basicWeapon);
            StartCoroutine(MoveToSpawn(dist / timeScale, bossSpawn));
            checkpoint = 1;
        }
        else if (checkpoint == 1 && playerInBossArea) {
            //play a death animation
            Announce(secondDeath);
            //load the next scene
            checkpoint = 2;
        }
        else if (playerInBossArea && UnlockTracker.instance.IsWeaponUnlocked(confidenceBlade) && !UnlockTracker.instance.IsArmorUnlocked(redArmor)) {
            //we died to the boss again later
            Announce(diedWithConfidence);
        }
        else if (playerInBossArea && UnlockTracker.instance.IsArmorUnlocked(redArmor)) {
            Announce(diedWithRed);
        }

        //pick where to spawn
        Transform destination = centralSpawn;

        if (checkpoint < 2) {
            destination = bossSpawn;
            //remind the player....
            if (checkpoint == 1 && !announcing) {
                Announce(firstDeathAgain);
            }
        }


        dist = (player.transform.position - destination.position).magnitude;
        StartCoroutine(MoveToSpawn(dist / timeScale, destination));

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


    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            playerInBossArea = true;
            PlayerEntersBossArea?.Invoke(this, System.EventArgs.Empty);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            playerInBossArea = false;
            PlayerLeavesBossArea?.Invoke(this, System.EventArgs.Empty);
        }
    }

}
