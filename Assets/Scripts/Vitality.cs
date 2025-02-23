using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Vitality : MonoBehaviour {

    public event System.EventHandler OnDeath;
    public event System.EventHandler<EFFECTS> OnGainEffect;
    public event System.EventHandler<EFFECTS> OnLoseEffect;

    public event System.EventHandler OnAttributeChange;


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
                if (!gameObject.CompareTag("Player")) {
                    Destroy(gameObject);
                    SceneManager.instance.OnTick -= OnTick;
                }
            }
            if (hp >= max_hp/2 && !baseAttributes.Contains(ATTRIBUTE.Healthy)) {
                baseAttributes.Add(ATTRIBUTE.Healthy);
                currentAttributes.Add(ATTRIBUTE.Healthy);
                if (GetComponent<GearManager>() != null) {
                    UpdateWeaponBenefits(GetComponent<GearManager>().GetWeapon());
                }
                OnAttributeChange?.Invoke(this, System.EventArgs.Empty);
            }
            if (hp < max_hp/2 && baseAttributes.Contains(ATTRIBUTE.Healthy)) {
                baseAttributes.Remove(ATTRIBUTE.Healthy);
                currentAttributes.Remove(ATTRIBUTE.Healthy);
                if (GetComponent<GearManager>() != null) {
                    UpdateWeaponBenefits(GetComponent<GearManager>().GetWeapon());
                }
                OnAttributeChange?.Invoke(this, System.EventArgs.Empty);
            }
        }
    }

    public void Reset() {
        List<EFFECTS> stats = new List<EFFECTS>(statusEffects);
        foreach (var st in stats) {
            RemoveStatusEffect(st);
        }
        currentAttributes.Clear();
        hp = max_hp;
        AddAttributes(equipment.GetWeapon(), equipment.GetArmor());
        UpdateWeaponBenefits(equipment.GetWeapon());
    }

    public List<EFFECTS> GetEffects() {
        return statusEffects;
    }

    public bool QueryStatusEffect(EFFECTS eff) {
        return statusEffects.Contains(eff);
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

    public void RemoveStatusEffect(EFFECTS effect) {
        if (statusEffects.Contains(effect)) {
            statusEffects.Remove(effect);
            OnLoseEffect?.Invoke(this, effect);
        }
        if (statusTimer.ContainsKey(effect)) {
            statusTimer.Remove(effect);
        }
    }


    private void Awake() {
        currentAttributes = new HashSet<ATTRIBUTE>();
        currentAttributes.AddRange(baseAttributes);
        statusEffects = new List<EFFECTS>();
        statusTimer = new Dictionary<EFFECTS, int>();
        if (equipment != null) {
            equipment.OnArmorChange += OnArmorChange;
            equipment.OnWeaponChange += OnWeaponChange;
        }
    }

    private void Start() {
        SceneManager.instance.OnTick += OnTick;
        hp = max_hp; // this sets the HEALTHY attribute & calls the event
    }

    public void OnArmorChange(System.Object src, (Weapon, Armor) previous) {
        //delete all attributes from the old armor
        foreach (var atb in previous.Item2.attributes) {
            if (currentAttributes.Contains(atb)) {
                currentAttributes.Remove(atb);
            }
        }
        UpdateWeaponBenefits(previous.Item1);
        //add attributes from the new armor + base in case we deleted them
        AddAttributes(equipment.GetWeapon(), equipment.GetArmor());
        //update base health bonus
        int dh = equipment.GetArmor().bonus_hp - previous.Item2.bonus_hp;
        if (dh != 0) {
            max_hp += dh;
            hp += dh;
        }
    }

    public void OnWeaponChange(System.Object src, (Weapon, Armor) previous) {
        //delete all attributes from the old armor
        foreach (var atb in previous.Item2.attributes) {
            if (currentAttributes.Contains(atb)) {
                currentAttributes.Remove(atb);
            }
        }
        RemoveWeaponBenefits(previous.Item1);
        //add attributes from the new armor + base in case we deleted them
        AddAttributes(equipment.GetWeapon(), equipment.GetArmor());
    }

    void UpdateWeaponBenefits(Weapon weapon) {
        foreach (var Ench in weapon.enchantments) {
            foreach (var bens in Ench.benefits) {
                //add any status effects that we have the attribute for
                 if (currentAttributes.Contains(bens.trigger) && !statusEffects.Contains(bens.effect)) {
                    statusEffects.Add(bens.effect);
                    statusTimer.Add(bens.effect,-1);
                    OnGainEffect?.Invoke(this, bens.effect);
                }
                //update this to remove the status effect if we've lost the required attribute
                if (!currentAttributes.Contains(bens.trigger) && statusEffects.Contains(bens.effect)) {
                    statusEffects.Remove(bens.effect);
                    statusTimer.Remove(bens.effect);
                    OnLoseEffect?.Invoke(this, bens.effect);
                }
            }
        }
    }


    void RemoveWeaponBenefits(Weapon weapon) {
        foreach (var Ench in weapon.enchantments) {
            foreach (var bens in Ench.benefits) {
                if (statusEffects.Contains(bens.effect)) {
                    statusEffects.Remove(bens.effect);
                    statusTimer.Remove(bens.effect);
                    OnLoseEffect?.Invoke(this, bens.effect);
                }
            }
        }
    }


    void AddAttributes(Weapon weapon, Armor armor) {
        currentAttributes.AddRange(armor.attributes);
        currentAttributes.AddRange(baseAttributes);
        foreach (Enchantment ench in weapon.enchantments) {
            foreach (var bens in ench.benefits) {
                if (currentAttributes.Contains(bens.trigger)) {
                    AddStatusEffect(bens.effect, -1); //add a permenant effect
                }
            }
        }
        OnAttributeChange?.Invoke(this, System.EventArgs.Empty);
    }



    void OnTick(System.Object src, uint count) {
        int hpChange = 0;
        foreach (EFFECTS effect in statusEffects) {
            hpChange += SceneManager.instance.EffectHealthChange(effect);
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
        hp += hpChange; //this modifies the effect list
    }

    public void Attacked(Weapon weap) {
       
       float dmg = SceneManager.instance.HandleCombat(weap, this);


       if (statusEffects.Contains(EFFECTS.harmless)) {
           dmg = 0;
           statusEffects.Remove(EFFECTS.harmless);
           statusTimer.Remove(EFFECTS.harmless);
           OnLoseEffect?.Invoke(this, EFFECTS.harmless);
       }
       if (statusEffects.Contains(EFFECTS.crit)) {
            dmg *= 2;
            statusEffects.Remove(EFFECTS.crit);
            statusTimer.Remove(EFFECTS.crit);
            OnLoseEffect?.Invoke(this, EFFECTS.crit);
       }
        if (statusEffects.Contains(EFFECTS.ethereal)) {
            dmg = 0;
            Debug.Log("Spooky ghost!!");
        }

       hp -= dmg;
    }

}
