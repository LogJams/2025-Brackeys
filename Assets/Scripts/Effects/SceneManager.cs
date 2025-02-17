using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour {

    public event EventHandler<uint> OnTick;
    uint tickCount = 0;

    float tickTime = 0.25f;
    float tickTimer;

    List<Vitality> aliveStuff;

    public GameObject firePrefab;

    public static SceneManager instance;

    private void Awake() {
        //singleton
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
            return;
        }
        else {
            instance = this;
        }
        //do initialization
        aliveStuff = new List<Vitality>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        aliveStuff.AddRange(FindObjectsByType<Vitality>(FindObjectsSortMode.None));

        foreach (Vitality v in aliveStuff) {
            v.OnGainEffect += OnGainEffect;
        }

        tickTimer = tickTime;

    }

    // Update is called once per frame
    void Update() {
        tickTimer -= Time.deltaTime;
        if (tickTimer <= 0) {
            tickTimer = tickTime;
            tickCount++;
            OnTick?.Invoke(this, tickCount);
        }
    }





    //////////// effect related code
    public void OnGainEffect(System.Object source, EFFECTS effect) {
        if (effect == EFFECTS.fire) {
            Instantiate(firePrefab, ((Vitality)source).transform);
        }
    }

    public int EffectHealthChange(EFFECTS effect) {
        int dh = 0;
        if (effect == EFFECTS.fire) {
            dh = -1;
        }
        if (effect == EFFECTS.heal) {
            dh = 1;
        }

        return dh;
    }

    public void OnDeath(System.Object source, EventArgs e) {
        Vitality vt = (Vitality)source;
        vt.OnGainEffect -= OnGainEffect;
        vt.OnDeath -= OnDeath;

    }

}
