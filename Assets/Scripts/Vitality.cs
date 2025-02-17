using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Vitality : MonoBehaviour {

    public event System.EventHandler OnDeath;
    public event System.EventHandler<EFFECTS> OnGainEffect;
    public event System.EventHandler<EFFECTS> OnLoseEffect;

    public event System.EventHandler<float> OnDamage;
    public event System.EventHandler<float> OnHeal;


    public float max_hp = 5;
    private float _hp; //don't ever use this version of hp

    [Header("Optional Variable")]
    public GearManager equipment;


    public List<ATTRIBUTE> baseAttributes;

    HashSet<ATTRIBUTE> currentAttributes;

    List<EFFECTS> statusEffects;

    Dictionary<EFFECTS, int> statusTimer;


    public float hp {
        get { return _hp; } 
        private set {
            //don't go above max hp
            value = Mathf.Min(value, max_hp);
            float dH = value - _hp;
            _hp = value;
            //fire an event if we take damage
            if (dH < 0) {
                OnDamage?.Invoke(this, dH);
            }
            if (dH > 0) {
                OnHeal?.Invoke(this, dH);
            }
            //fire another event if we die
            if (hp <= 0) { 
                OnDeath?.Invoke(this, System.EventArgs.Empty);
                SceneManager.instance.OnTick -= OnTick;
                Destroy(gameObject);
            }
        }
    }


    public List<EFFECTS> GetEffects() {
        return statusEffects;
    }

    public List<ATTRIBUTE> GetAttributes() {
        return new List<ATTRIBUTE>(currentAttributes);
    }

    //also take a duration, default is 1 tick
    public void AddStatusEffect(EFFECTS effect, int duration=2) {
        if (!statusEffects.Contains(effect)) {
            statusEffects.Add(effect);
            OnGainEffect?.Invoke(this, effect);
        }
        if (!statusTimer.ContainsKey(effect)) {
            statusTimer.Add(effect, duration);
        }
    }


    private void Awake() {
        hp = max_hp;
        statusEffects = new List<EFFECTS>();
        statusTimer = new Dictionary<EFFECTS, int>();
        if (equipment != null) {
            equipment.OnArmorChange += OnArmorChange;
        }
        currentAttributes = new HashSet<ATTRIBUTE>();
        currentAttributes.AddRange(baseAttributes);
    }

    private void Start() {
        SceneManager.instance.OnTick += OnTick;
    }

    public void OnArmorChange(System.Object src, (Weapon, Armor) previous) {
        //delete all attributes from the old armor
        foreach (var atb in previous.Item2.attributes) {
            if (currentAttributes.Contains(atb)) {
                currentAttributes.Remove(atb);
            }
        }
        //add attributes from the new armor + base in case we deleted them
        currentAttributes.AddRange(equipment.GetArmor().attributes);
        currentAttributes.AddRange(baseAttributes);
    }

    void OnTick(System.Object src, uint count) {
        foreach (EFFECTS effect in statusEffects) {
            int hpChange = SceneManager.instance.EffectHealthChange(effect);
            hp += hpChange;
            statusTimer[effect]--; //decrease our timer for this effect
        }
        //remove anything that is exactly 0
        var effects = new List<EFFECTS>(statusTimer.Keys);

        foreach(EFFECTS effect in effects) {
            if (statusTimer[effect] == 0) {
                statusEffects.Remove(effect);
                statusTimer.Remove(effect);
                OnLoseEffect?.Invoke(this, effect);
            }
        }

    }

    private void OnTriggerEnter(Collider other) {
        Weapon weap = null;

        if (other.TryGetComponent(out weap) && weap.owner != this) {
            int dmg = weap.OnHit(this);

            if (statusEffects.Contains(EFFECTS.harmless)) {
                dmg = 0;
                statusEffects.Remove(EFFECTS.harmless);
                statusTimer.Remove(EFFECTS.harmless);
                OnLoseEffect?.Invoke(this, EFFECTS.harmless);

            }

            hp -= dmg;
        }
    }

}
