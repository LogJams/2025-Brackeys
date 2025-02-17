using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct EffectMap {
    public EFFECTS effect;
    public GameObject effectPrefab;
}

public class SceneManager : MonoBehaviour {

    public event EventHandler<uint> OnTick;
    uint tickCount = 0;

    //how often to update ticks (status effects, etc.)
    float tickTime = 1.0f;
    float tickTimer;

    List<Vitality> aliveStuff;

    public List<EffectMap> effectMap;
    Dictionary<EFFECTS, GameObject> effectPrefabs;

    public static SceneManager instance;


    Dictionary<(Vitality, EFFECTS), GameObject> activeEffects;


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
        activeEffects = new Dictionary<(Vitality, EFFECTS), GameObject>();

        //create a mapping from effects to prefabs
        effectPrefabs = new Dictionary<EFFECTS, GameObject>(effectMap.Count);
        foreach (var map in effectMap) {
            effectPrefabs.Add(map.effect, map.effectPrefab);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        aliveStuff.AddRange(FindObjectsByType<Vitality>(FindObjectsSortMode.None));

        foreach (Vitality v in aliveStuff) {
            v.OnGainEffect += OnGainEffect;
            v.OnLoseEffect += OnLoseEffect;
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
        var key = ((Vitality)source, effect);
        //we won't change the scene if this effect already exists
        if (activeEffects.ContainsKey(key)) {
            return;
        }
        //add an effect to this vitality if we have defined it
        if (effectPrefabs.ContainsKey(effect)) {
            GameObject fire = Instantiate(effectPrefabs[effect], ((Vitality)source).transform);
            activeEffects.Add(key, fire);
        }
    }

    public void OnLoseEffect(System.Object source, EFFECTS effect) {
        var key = ((Vitality)source, effect);
        if (!activeEffects.ContainsKey(key)) {
            Debug.LogWarning("Tried to remove " + effect + " from " + ((Vitality)source).gameObject.name + ", but none found.");
        } else {
            //destroy the active particle effect and remove it from the list
            GameObject.Destroy(activeEffects[key]);
            activeEffects.Remove(key);
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
        //clean up our event listeners
        vt.OnGainEffect -= OnGainEffect;
        vt.OnLoseEffect -= OnLoseEffect;
        vt.OnDeath -= OnDeath;
        //clear out the active effects for this vitality object
        foreach (EFFECTS ef in Enum.GetValues(typeof(EFFECTS))) {
            if (activeEffects.ContainsKey( (vt, ef) )) {
                activeEffects.Remove((vt, ef));
            }

        }
    }

}
