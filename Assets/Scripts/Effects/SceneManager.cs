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


        // exit and reload for testing
        if (Input.GetKeyUp(KeyCode.Escape)) {
            Application.Quit();
        }
        if (Input.GetKeyUp(KeyCode.F1)) {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
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
        // "harmless" is an effect that makes the current attack not damage the target
        // we don't create any effects, so we can just exit out instead
        if (effect == EFFECTS.harmless) {
            return;
        }

        var key = ((Vitality)source, effect);
        if (!activeEffects.ContainsKey(key)) {
            Debug.LogWarning("Tried to remove " + effect + " from " + ((Vitality)source).gameObject.name + ", but none found.");
        } else {
            //destroy the active particle effect and remove it from the list
            GameObject.Destroy(activeEffects[key]);
            activeEffects.Remove(key);
        }

    }

    //for combat related stuff
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


    public float HandleCombat(Weapon weap, Vitality target) {
        float damage = weap.baseDamage;

        ///// block of code to handle enchantments

        //first check for curses
        bool curseEffect = false;
        foreach (Enchantment ench in weap.enchantments) {
            foreach (Curse curse in ench.curses) {
                //check if the curse applies
                if (target.GetAttributes().Contains(curse.trigger)) {
                    curseEffect = true;
                    //apply (beneficial?) curses to targets
                    if (curse.target == TARGETS.target) {
                        target.AddStatusEffect(curse.effect);
                    }
                    //apply cuse to ourselves
                    else {
                        weap.GetOwner().AddStatusEffect(curse.effect);
                    }


                }
            }
        }


        //todo: check how our enchantment affects OnHit based on target properties
        if (!curseEffect) {
            foreach (Enchantment ench in weap.enchantments) {
                //do stuff to the target if it applies
                if (ench.target == TARGETS.target && HasValidAttribute(target, ench.attribute)) {
                    target.AddStatusEffect(ench.effect);
                } else if (ench.target == TARGETS.self && HasValidAttribute(weap.GetOwner(), ench.attribute)) {
                    weap.GetOwner().AddStatusEffect(ench.effect);
                }
            }
        }
        return damage;
    }


    private bool HasValidAttribute(Vitality vit, ATTRIBUTE atb) {
        if (atb == ATTRIBUTE.ANY || vit.GetAttributes().Contains(atb)) {
            return true;
        }

        return false;
    }


    //clean up references when something dies
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
